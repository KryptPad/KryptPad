using KryptPad.Api.Exceptions;
using KryptPad.Api.Models;
using KryptPad.Api.Requests;
using KryptPad.Api.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KryptPad.Api
{

    /// <summary>
    /// Accesses the KryptPad API
    /// </summary>
    public class KryptPadApi
    {

        private const int SESSION_TIME_MINUTES = 5;
        private const int SESSION_WARNING_MINUTES = 1;
        // How much time is taken away from the token's ttl to account for network latency
        private const int EXPIRATION_TIME_THRESHOLD = 10;

        // Use a semaphore to prevent a task from executing simultaniously
        private static SemaphoreSlim ReauthenticateSemaphore = new SemaphoreSlim(1, 1);


#if LOCAL
        /// <summary>
        /// Gets the host address of the API service.
        /// </summary>
        public static string ServiceHost { get; set; } = "http://localhost:50821/";
        //public static string ServiceHost { get; set; } = "https://www.kryptpad.com/";
#elif DEBUG
        /// <summary>
        /// Gets or sets the host address of the API service.
        /// </summary>
        public static string ServiceHost { get; set; } = "http://test.kryptpad.com/";
#else
        /// <summary>
        /// Gets the host address of the API service.
        /// </summary>
        public static string ServiceHost { get; } = "https://www.kryptpad.com/";
#endif

        #region Delegates
        public delegate void SessionEndingHandler(DateTime expiration);
        public delegate void SessionEndedHandler();
        #endregion

        #region Events
        public static event SessionEndingHandler SessionEnding;
        public static event SessionEndedHandler SessionEnded;

        //public static event AccessTokenExpirationTimerHandler AccessTokenExpirationTimer;
        #endregion

        #region Properties
        private static Guid _appId;

        /// <summary>
        /// Gets the app id of the current instance of the app
        /// </summary>
        private static Guid AppId
        {
            get
            {
                if (_appId == Guid.Empty)
                {
                    // Generate a new app id for the lifetime of this app's
                    // instance. It will be appended to the client id.
                    _appId = Guid.NewGuid();
                }

                return _appId;
            }
        }

        /// <summary>
        /// Gets or sets the API OAuth access token to authorize API calls
        /// </summary>
        private static OAuthTokenResponse TokenResponse { get; set; }

        /// <summary>
        /// Gets or sets when the token is supposed to expire
        /// </summary>
        private static DateTime TokenExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets the passphrase for the profile
        /// </summary>
        private static string Passphrase { get; set; }

        /// <summary>
        /// Gets or sets the current profile Id
        /// </summary>
        public static ApiProfile CurrentProfile { get; private set; }

        /// <summary>
        /// Gets or sets the session end time
        /// </summary>
        private static DateTime SessionEndDate { get; set; }

        /// <summary>
        /// Gets or sets the CancellationToken for the task
        /// </summary>
        private static CancellationTokenSource ExpirationTaskCancelTokenSource { get; set; }

        /// <summary>
        /// Gets the api token endpoint to use for authentication
        /// </summary>
        private static string ApiTokenEndpoint { get; set; }

        #endregion


        /// <summary>
        /// Starts the expiration task
        /// </summary>
        private static void StartExpirationTask()
        {
            // Initialize new cancellation token
            ExpirationTaskCancelTokenSource = new CancellationTokenSource();

            // Start a task that will check the expiration date of the access token, when
            // the current time has passed token expiration, an event will be raised.
            Task.Factory.StartNew(
                async () => await ExpirationTaskWork(ExpirationTaskCancelTokenSource.Token),
                ExpirationTaskCancelTokenSource.Token);
        }

        /// <summary>
        /// Performs the work of the expiration task
        /// </summary>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
        private static async Task ExpirationTaskWork(CancellationToken cancelToken)
        {

            // Get local date from expiration
            var expiration = SessionEndDate;

            // Store handle to events
            var sessionEndingHandler = SessionEnding;
            var sessionEndedHandler = SessionEnded;

            // Enter a while loop and check that the expiration date hasn't passed
            while (DateTime.Now < expiration)
            {
                // Wait a bit, then check again
                cancelToken.ThrowIfCancellationRequested();

                // Set date to check
                expiration = SessionEndDate;

                // Calculate the time when the warning should show
                //var warningTime = expiration.AddMinutes(-SESSION_WARNING_MINUTES);

                // Fire event
                sessionEndingHandler?.Invoke(expiration);

                // Wait a bit, then check again
                await Task.Delay(100);

            }

            // If the loop exits, then we have expired
            sessionEndedHandler?.Invoke();
        }

        /// <summary>
        /// Gets the endpoint information from the server
        /// </summary>
        /// <returns></returns>
        private static async Task SetEndpointConfigurationAsync()
        {
            var tokenEndpoint = GetUrl("token");
            try
            {
                using (var client = new HttpClient())
                {
                    var resp = await client.GetStringAsync(GetUrl(".well-known/openid-configuration"));
                    if (!string.IsNullOrWhiteSpace(resp)) {
                        // Deserialize the configuration and set the endpoints
                        var config = JsonConvert.DeserializeObject<EndpointConfigurationResponse>(resp);
                        tokenEndpoint = config?.TokenEndpoint;
                    }

                }
            }
            catch (Exception ex) {

            }

            // Set the token endpoint
            ApiTokenEndpoint = tokenEndpoint;
        }

        /// <summary>
        /// Creates an authorization request
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static async Task AuthenticateAsync(string username, string password)
        {
            // Check if we have a token endpoint configured
            if (string.IsNullOrWhiteSpace(ApiTokenEndpoint))
            {
                // Get the token endpoint
                await SetEndpointConfigurationAsync();
            }


            using (var client = new HttpClient())
            {
                // Prepare form values
                var values = new Dictionary<string, string>
                {
                    { "client_id", "KryptPadUniversal_" + AppId.ToString() },
                    { "client_secret", "secret" },
                    { "grant_type", "password" },
                    { "username", username },
                    { "password", password }
                };

                // Create the content to send
                var content = new FormUrlEncodedContent(values);
                // Send the post request
                var response = await client.PostAsync(ApiTokenEndpoint, content);

                // Get the data if the response is what we want
                if (response.IsSuccessStatusCode)
                {
                    // Get the response as a string
                    var data = await response.Content.ReadAsStringAsync();

                    // Deserialize the data and get the access token
                    TokenResponse = JsonConvert.DeserializeObject<OAuthTokenResponse>(data);

                    // Set the expiration date based on the ttl of the access token
                    TokenExpirationDate = DateTime.Now.AddSeconds(TokenResponse.ExpiresIn - EXPIRATION_TIME_THRESHOLD);

                    // Set the session end time
                    ExtendSessionTime();

                    // Start session expiration task
                    StartExpirationTask();
                }
                else
                {
                    throw await CreateException(response);
                }
            }

        }

        /// <summary>
        /// Refreshes the access token using the refresh token
        /// </summary>
        /// <returns></returns>
        public static async Task ReauthenticateAsync()
        {
            // Check if we have a token endpoint configured
            if (string.IsNullOrWhiteSpace(ApiTokenEndpoint))
            {
                // Get the token endpoint
                await SetEndpointConfigurationAsync();
            }

            using (var client = new HttpClient())
            {
                // Prepare form values
                // TODO: Ideally, we want to pass a client secret here, but this is 
                // not something we can store in the source code (open-source... d'oh).
                var values = new Dictionary<string, string>
                {
                    { "client_id", "KryptPadUniversal_" + AppId.ToString() },
                    { "grant_type", "refresh_token" },
                    { "refresh_token", TokenResponse.RefreshToken }
                };

                // Create the content to send
                var content = new FormUrlEncodedContent(values);
                // Send the post request
                var response = await client.PostAsync(ApiTokenEndpoint, content);

                // Get the data if the response is what we want
                if (response.IsSuccessStatusCode)
                {
                    // Get the response as a string
                    var data = await response.Content.ReadAsStringAsync();

                    // Deserialize the data and get the access token
                    TokenResponse = JsonConvert.DeserializeObject<OAuthTokenResponse>(data);

                    // Set the expiration date based on the ttl of the access token
                    TokenExpirationDate = DateTime.Now.AddSeconds(TokenResponse.ExpiresIn - EXPIRATION_TIME_THRESHOLD);

                }
                else
                {
                    throw await CreateException(response);
                }
            }

        }

        /// <summary>
        /// Creates an account in the system
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static async Task<SuccessResponse> CreateAccountAsync(string username, string password, string confirmPassword)
        {
            using (var client = new HttpClient())
            {
                //create object to pass
                var values = new
                {
                    email = username,
                    password = password,
                    confirmPassword = confirmPassword
                };

                //create content
                var content = JsonContent(values);

                //execute request
                var response = await client.PostAsync(GetUrl("api/account/register"), content);

                //check if the response is a success code
                if (response.IsSuccessStatusCode)
                {
                    //get the response content
                    var data = await response.Content.ReadAsStringAsync();

                    return new SuccessResponse();
                }
                else
                {
                    throw await CreateException(response);
                }
            }

        }

        #region App
        /// <summary>
        /// Gets the system broadcast message
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetBroadcastMessage()
        {
            using (var client = new HttpClient())
            {
                
                //send request and get a response
                var response = await client.GetAsync(GetUrl("api/app/broadcast-message"));

                //deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // Read the response
                        var data = await response.Content.ReadAsStringAsync();
                        return data;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    throw await CreateException(response);
                }
            }

        }
        #endregion

        #region Profiles

        /// <summary>
        /// Gets all profiles for the authenticated user
        /// </summary>
        /// <returns></returns>
        public static async Task<ProfileResponse> GetProfilesAsync()
        {
            using (var client = new HttpClient())
            {
                //authorize the request
                await AuthorizeRequest(client);

                //send request and get a response
                var response = await client.GetAsync(GetUrl("api/profiles"));

                //deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    // Read the data
                    var data = await response.Content.ReadAsStringAsync();
                    // Deserialize the response as an ApiResponse object
                    return JsonConvert.DeserializeObject<ProfileResponse>(data);
                }
                else
                {
                    throw await CreateException(response);
                }
            }

        }

        /// <summary>
        /// Gets a specific profile for the authenticated user
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="passphrase"></param>
        /// <returns></returns>
        public static async Task LoadProfileAsync(ApiProfile profile, string passphrase)
        {
            using (var client = new HttpClient())
            {
                //authorize the request
                await AuthorizeRequest(client);
                // Add passphrase to message
                AddPassphraseHeader(client, passphrase);
                //send request and get a response
                var response = await client.GetAsync(GetUrl($"api/profiles/{profile.Id}"));

                //deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    //read the data
                    var data = await response.Content.ReadAsStringAsync();
                    // Deserialize the response as an ApiResponse object
                    var profileResp = JsonConvert.DeserializeObject<ProfileResponse>(data);

                    // Set current profile and passphrase
                    if (profileResp != null && profileResp.Profiles.Length > 0)
                    {
                        CurrentProfile = profileResp.Profiles.SingleOrDefault();
                        Passphrase = passphrase;
                    }

                }
                else
                {
                    throw await CreateException(response);
                }
            }

        }

        /// <summary>
        /// Gets a specific profile for the authenticated user
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="passphrase"></param>
        /// <returns></returns>
        public static async Task<string> DownloadCurrentProfileAsync()
        {
            using (var client = new HttpClient())
            {
                //authorize the request
                await AuthorizeRequest(client);
                // Add passphrase to message
                AddPassphraseHeader(client);
                //send request and get a response
                var response = await client.GetAsync(GetUrl($"api/profiles/{CurrentProfile.Id}/download"));

                //deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    //read the data
                    var data = await response.Content.ReadAsStringAsync();

                    return data;

                }
                else
                {
                    throw await CreateException(response);
                }
            }

        }

        /// <summary>
        /// Uploads a profile
        /// </summary>
        /// <param name="profileData"></param>
        /// <returns></returns>
        public static async Task<SuccessResponse> UploadProfile(string profileData)
        {
            using (var client = new HttpClient())
            {
                // Authorize the request
                await AuthorizeRequest(client);

                var content = new StringContent(profileData, Encoding.UTF8, "application/json");
                // Send request and get a response
                var response = await client.PostAsync(GetUrl($"api/profiles/upload"), content);

                //deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    // Read the data
                    var data = await response.Content.ReadAsStringAsync();
                    // Deserialize the response as an ApiResponse object
                    return new SuccessResponse(Convert.ToInt32(data));

                }
                else
                {
                    throw await CreateException(response);
                }
            }
        }

        /// <summary>
        /// Creates a new profile
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="passphrase"></param>
        /// <returns></returns>
        public static async Task<ApiProfile> CreateProfileAsync(CreateProfileRequest profile)
        {
            using (var client = new HttpClient())
            {
                // Authorize the request.
                await AuthorizeRequest(client);

                // Create JSON content.
                var content = JsonContent(profile);

                // Send request and get a response
                var response = await client.PostAsync(GetUrl($"api/profiles"), content);


                // Deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    // Read the data
                    var data = await response.Content.ReadAsStringAsync();

                    // Create an ApiProfile object
                    var p = new ApiProfile()
                    {
                        Name = profile.Name,
                        Id = Convert.ToInt32(data)
                    };

                    // Return the profile object
                    return p;
                }
                else
                {
                    throw await CreateException(response);
                }
            }

        }

        /// <summary>
        /// Creates a new profile
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="passphrase"></param>
        /// <returns></returns>
        public static async Task<SuccessResponse> SaveProfileAsync(ApiProfile profile)
        {
            using (var client = new HttpClient())
            {
                // Authorize the request.
                await AuthorizeRequest(client);

                // Create JSON content.
                var content = JsonContent(profile);

                // Send request and get a response
                var response = await client.PutAsync(GetUrl($"api/profiles/{profile.Id}"), content);

                // Deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    // Read the data
                    var data = await response.Content.ReadAsStringAsync();
                    // Deserialize the response as an ApiResponse object
                    return new SuccessResponse(Convert.ToInt32(data));
                }
                else
                {
                    throw await CreateException(response);
                }
            }

        }

        /// <summary>
        /// Changes the profile passphrase
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="newPassphrase"></param>
        /// <returns></returns>
        public static async Task ChangePassphraseAsync(string oldPassphrase, string newPassphrase)
        {
            using (var client = new HttpClient())
            {
                // Authorize the request.
                await AuthorizeRequest(client);
                // Add passphrase to message
                AddPassphraseHeader(client, oldPassphrase);

                // Create object to send to API
                var values = new
                {
                    NewPassphrase = newPassphrase
                };

                // Create JSON content.
                var content = JsonContent(values);

                // Create
                var response = await client.PostAsync(GetUrl($"api/profiles/{CurrentProfile.Id}/change-passphrase"), content);

                // Deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    // Store the new passphrase
                    Passphrase = newPassphrase;
                }
                else
                {
                    throw await CreateException(response);
                }
            }

        }

        /// <summary>
        /// Deletes the profile
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static async Task DeleteProfileAsync(ApiProfile profile)
        {
            using (var client = new HttpClient())
            {
                // Authorize the request.
                await AuthorizeRequest(client);

                // Send request and get a response
                var response = await client.DeleteAsync(GetUrl($"api/profiles/{profile.Id}"));

                // Deserialize the object based on the result
                if (!response.IsSuccessStatusCode)
                {
                    throw await CreateException(response);
                }
            }

        }

        #endregion

        #region Categories

        /// <summary>
        /// Gets all categories for the authenticated user
        /// </summary>
        /// <returns></returns>
        public static async Task<CategoryResponse> GetCategoriesWithItemsAsync()
        {

            using (var client = new HttpClient())
            {
                // Authorize the request
                await AuthorizeRequest(client);
                // Add passphrase to message
                AddPassphraseHeader(client);
                // Send request and get a response
                var response = await client.GetAsync(GetUrl($"api/profiles/{CurrentProfile.Id}/categories/with-items"));

                // Deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    // Read the data
                    var data = await response.Content.ReadAsStringAsync();
                    // Deserialize the response as an ApiResponse object
                    return JsonConvert.DeserializeObject<CategoryResponse>(data);
                }
                else
                {
                    throw await CreateException(response);
                }
            }

        }

        /// <summary>
        /// Gets all categories for the authenticated user
        /// </summary>
        /// <returns></returns>
        public static async Task<CategoryResponse> GetCategoriesAsync()
        {

            using (var client = new HttpClient())
            {
                // Authorize the request
                await AuthorizeRequest(client);
                // Add passphrase to message
                AddPassphraseHeader(client);
                // Send request and get a response
                var response = await client.GetAsync(GetUrl($"api/profiles/{CurrentProfile.Id}/categories"));

                // Deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    // Read the data
                    var data = await response.Content.ReadAsStringAsync();
                    // Deserialize the response as an ApiResponse object
                    return JsonConvert.DeserializeObject<CategoryResponse>(data);
                }
                else
                {
                    throw await CreateException(response);
                }
            }

        }

        /// <summary>
        /// Saves a category to the database
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="category"></param>
        /// <param name="passphrase"></param>
        /// <returns></returns>
        public static async Task<SuccessResponse> SaveCategoryAsync(ApiCategory category)
        {
            using (var client = new HttpClient())
            {
                // Authorize the request.
                await AuthorizeRequest(client);
                // Add passphrase to message
                AddPassphraseHeader(client);
                // Create JSON content.
                var content = JsonContent(category);
                // Send request and get a response
                HttpResponseMessage response;

                if (category.Id == 0)
                {
                    // Create
                    response = await client.PostAsync(GetUrl($"api/profiles/{CurrentProfile.Id}/categories"), content);
                }
                else
                {
                    // Update
                    response = await client.PutAsync(GetUrl($"api/profiles/{CurrentProfile.Id}/categories/{category.Id}"), content);
                }

                // Read the data
                var data = await response.Content.ReadAsStringAsync();

                // Deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response as an ApiResponse object
                    return new SuccessResponse(Convert.ToInt32(data));
                }
                else
                {
                    throw await CreateException(response);
                }
            }

        }

        /// <summary>
        /// Deletes a category from the database
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteCategoryAsync(int categoryId)
        {

            using (var client = new HttpClient())
            {
                // Authorize the request.
                await AuthorizeRequest(client);

                // Send request and get a response
                var response = await client.DeleteAsync(GetUrl($"api/profiles/{CurrentProfile.Id}/categories/{categoryId}"));

                // Deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response as an ApiResponse object
                    return true;
                }
                else
                {
                    throw await CreateException(response);
                }
            }
        }
        #endregion

        #region Items

        /// <summary>
        /// Gets an item by its id, including all the details
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static async Task<ItemsResponse> GetItemAsync(int categoryId, int itemId)
        {
            using (var client = new HttpClient())
            {
                // Authorize the request
                await AuthorizeRequest(client);
                // Add passphrase to message
                AddPassphraseHeader(client);
                // Send request and get a response
                var response = await client.GetAsync(GetUrl($"api/profiles/{CurrentProfile.Id}/categories/{categoryId}/items/{itemId}"));

                // Deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    // Read the data
                    var data = await response.Content.ReadAsStringAsync();
                    // Deserialize the response as an ApiResponse object
                    return JsonConvert.DeserializeObject<ItemsResponse>(data);
                }
                else
                {
                    throw await CreateException(response);
                }
            }

        }

        /// <summary>
        /// Creates a new item in the specified category
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="categoryId"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<SuccessResponse> SaveItemAsync(int categoryId, ApiItem item)
        {
            using (var client = new HttpClient())
            {

                // Authorize the request.
                await AuthorizeRequest(client);
                // Add passphrase to message
                AddPassphraseHeader(client);
                // Create content to send
                var content = JsonContent(item);

                // Execute request
                HttpResponseMessage response;

                if (item.Id == 0)
                {
                    response = await client.PostAsync(GetUrl($"api/profiles/{CurrentProfile.Id}/categories/{categoryId}/items"), content);
                }
                else
                {
                    response = await client.PutAsync(GetUrl($"api/profiles/{CurrentProfile.Id}/categories/{categoryId}/items/{item.Id}"), content);
                }

                // Check if the response is a success code
                if (response.IsSuccessStatusCode)
                {
                    // Get the response content
                    var data = await response.Content.ReadAsStringAsync();
                    // Create SuccessResponse object
                    return new SuccessResponse(Convert.ToInt32(data));
                }
                else
                {
                    throw await CreateException(response);
                }


            }

        }

        /// <summary>
        /// Deletes an item and all its fields from the database
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="categoryId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static async Task DeleteItemAsync(int categoryId, int itemId)
        {
            using (var client = new HttpClient())
            {

                // Authorize the request.
                await AuthorizeRequest(client);

                // Execute request
                var response = await client.DeleteAsync(GetUrl($"api/profiles/{CurrentProfile.Id}/categories/{categoryId}/items/{itemId}"));

                // Check if the response is a success code
                if (!response.IsSuccessStatusCode)
                {
                    throw await CreateException(response);
                }


            }

        }

        #endregion

        #region Fields
        /// <summary>
        /// Creates a new field
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="categoryId"></param>
        /// <param name="itemId"></param>
        /// <param name="field"></param>
        /// <param name="passphrase"></param>
        /// <returns></returns>
        public static async Task<SuccessResponse> SaveFieldAsync(int categoryId, int itemId, ApiField field)
        {
            using (var client = new HttpClient())
            {

                // Authorize the request.
                await AuthorizeRequest(client);
                // Add passphrase to message
                AddPassphraseHeader(client);
                // Create content to send
                var content = JsonContent(field);

                // Execute request
                HttpResponseMessage response;

                if (field.Id == 0)
                {
                    // Create
                    response = await client.PostAsync(GetUrl($"api/profiles/{CurrentProfile.Id}/categories/{categoryId}/items/{itemId}/fields"), content);
                }
                else
                {
                    // Update
                    response = await client.PutAsync(GetUrl($"api/profiles/{CurrentProfile.Id}/categories/{categoryId}/items/{itemId}/fields/{field.Id}"), content);
                }

                // Check if the response is a success code
                if (response.IsSuccessStatusCode)
                {
                    // Get the response content
                    var data = await response.Content.ReadAsStringAsync();
                    // Create SuccessResponse object
                    return new SuccessResponse(Convert.ToInt32(data));
                }
                else
                {
                    throw await CreateException(response);
                }


            }


        }

        /// <summary>
        /// Gets the fields from an item
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="categoryId"></param>
        /// <param name="itemId"></param>
        /// <param name="passphrase"></param>
        /// <returns></returns>
        public static async Task<FieldsResponse> GetFieldsAsync(int categoryId, int itemId)
        {
            using (var client = new HttpClient())
            {
                // Authorize the request
                await AuthorizeRequest(client);
                // Add passphrase to message
                AddPassphraseHeader(client);
                // Send request and get a response
                var response = await client.GetAsync(GetUrl($"api/profiles/{CurrentProfile.Id}/categories/{categoryId}/items/{itemId}/fields"));

                // Deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    // Read the data
                    var data = await response.Content.ReadAsStringAsync();
                    // Deserialize the response as an ApiResponse object
                    return JsonConvert.DeserializeObject<FieldsResponse>(data);
                }
                else
                {
                    throw await CreateException(response);
                }
            }

        }

        /// <summary>
        /// Deletes a field
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="categoryId"></param>
        /// <param name="itemId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task DeleteFieldAsync(int categoryId, int itemId, int id)
        {
            using (var client = new HttpClient())
            {

                // Authorize the request.
                await AuthorizeRequest(client);

                // Execute request
                var response = await client.DeleteAsync(GetUrl($"api/profiles/{CurrentProfile.Id}/categories/{categoryId}/items/{itemId}/fields/{id}"));

                // Check if the response is a success code
                if (!response.IsSuccessStatusCode)
                {
                    throw await CreateException(response);
                }


            }
        }
        #endregion

        #region Helper methods

        /// <summary>
        /// Process a failed request
        /// </summary>
        /// <param name="response"></param>
        private static async Task<WebException> CreateException(HttpResponseMessage response)
        {
            WebException exception;

            // If a bad request is received, try and figure out why
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {

                try
                {
                    ApiWebExceptionResponse r = null;

                    // Read the data
                    var data = await response.Content.ReadAsStringAsync();

                    // Check is this is an oauth error
                    var msg = JsonConvert.DeserializeObject(data) as JObject;

                    // Does this have what we want?
                    //if ((string)msg["Message"] != null)
                    //{
                    //    // Use the string directly
                    //    exception = new WebException((string)msg["Message"]);
                    //}
                    if ((string)msg["error"] == "invalid_grant")
                    {
                        // Deserialize the data
                        r = JsonConvert.DeserializeObject<OAuthTokenErrorResponse>(data);

                    }
                    else
                    {
                        // Deserialize the data
                        r = JsonConvert.DeserializeObject<WebExceptionResponse>(data);

                    }

                    // If we have a WebExceptionResponse object, then use that to create an exception
                    exception = r.ToException();


                }
                catch (Exception ex)
                {
                    // Epic fail
                    exception = new WebException("The server responded with a Bad Request status, but gave no reason.", ex);
                }


            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                exception = new UnauthorizedWebException();
            }
            else
            {
                exception = new WebException("An error occurred while trying to process your request.");
            }



            // Return the exception
            return exception;

        }

        /// <summary>
        /// Authorizes an http client request
        /// </summary>
        /// <param name="client"></param>
        private static async Task AuthorizeRequest(HttpClient client)
        {
            await ReauthenticateSemaphore.WaitAsync();
            try
            {
                // If we have a token, use it to authorize the request
                if (TokenResponse != null && !string.IsNullOrWhiteSpace(TokenResponse.AccessToken))
                {

                    // Check the expiration time
                    if (TokenExpirationDate <= DateTime.Now)
                    {
                        // Attempt to get a new access token
                        await ReauthenticateAsync();
                    }

                    //add the authorize header to the request
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenResponse.AccessToken);

                    // Set the session end time
                    ExtendSessionTime();
                }
                else
                {
                    // No token? Hmm.
                }
            }
            catch (Exception)
            {
                //ReauthenticateSemaphore.Release();
            }
            finally
            {
                ReauthenticateSemaphore.Release();

            }

        }

        /// <summary>
        /// Adds a passphrase header to the outgoing message
        /// </summary>
        /// <param name="client"></param>
        private static void AddPassphraseHeader(HttpClient client, string passphrase = null)
        {
            // Get the current passphrase
            passphrase = passphrase ?? Passphrase;

            // Add passphrase to the header
            client.DefaultRequestHeaders.Add("Passphrase", passphrase);
        }

        /// <summary>
        /// Creates a StringContent object from an object serialized to JSON
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private static HttpContent JsonContent(object values)
        {
            var data = JsonConvert.SerializeObject(values);

            return new StringContent(data, Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// Gets the url for the api request
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static string GetUrl(string uri) => $"{ServiceHost}{uri}";

        /// <summary>
        /// Cancels the expiration task
        /// </summary>
        private static void CancelExpirationTask()
        {
            if (ExpirationTaskCancelTokenSource != null)
            {
                // Cancel task
                ExpirationTaskCancelTokenSource.Cancel();

            }
        }

        /// <summary>
        /// Closes the current profile
        /// </summary>
        public static void CloseProfile()
        {
            CurrentProfile = null;
            Passphrase = null;

        }

        /// <summary>
        /// Signs out of the api
        /// </summary>
        public static void SignOutAsync()
        {
            // Cancel task
            CancelExpirationTask();

            // Clean up
            TokenResponse = null;
            CurrentProfile = null;
            Passphrase = null;
            //Username = null;
            //Password = null;

        }

        /// <summary>
        /// Extends the session time
        /// </summary>
        public static void ExtendSessionTime()
        {
            // Set the session end time
            SessionEndDate = DateTime.Now.AddMinutes(SESSION_TIME_MINUTES);
        }
        #endregion

    }
}
