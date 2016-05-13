using KryptPadCSApp.API.Models;
using KryptPadCSApp.API.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KryptPadCSApp.API
{

    /// <summary>
    /// Accesses the KryptPad API
    /// </summary>
    class KryptPadApi
    {
#if LOCAL
        /// <summary>
        /// Gets the host address of the API service.
        /// </summary>
        private static string ServiceHost { get; } = "http://localhost:50821/"; //
#elif DEBUG
        /// <summary>
        /// Gets the host address of the API service.
        /// </summary>
        private static string ServiceHost { get; } = "http://test.kryptpad.com/";
#else
        /// <summary>
        /// Gets the host address of the API service.
        /// </summary>
        private static string ServiceHost { get; } = "https://www.kryptpad.com/";
#endif

        #region Events
        public static event EventHandler AccessTokenExpired;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the API OAuth access token to authorize API calls
        /// </summary>
        private static OAuthTokenResponse TokenResponse { get; set; }

        /// <summary>
        /// Gets or sets the username of the user logged in
        /// </summary>
        private static string Username { get; set; }

        /// <summary>
        /// Gets or sets the password of the user logged in
        /// </summary>
        private static string Password { get; set; }

        /// <summary>
        /// Gets or sets the passphrase for the profile
        /// </summary>
        private static string Passphrase { get; set; }

        /// <summary>
        /// Gets or sets the current profile Id
        /// </summary>
        public static ApiProfile CurrentProfile { get; private set; }

        /// <summary>
        /// Gets whether the user is signed in
        /// </summary>
        public static bool IsSignedIn
        {
            get { return !string.IsNullOrWhiteSpace(TokenResponse?.AccessToken); }
        }

        /// <summary>
        /// Gets or sets the CancellationToken for the task
        /// </summary>
        private static CancellationTokenSource ExpirationTaskCancelTokenSource { get; set; }

        /// <summary>
        /// Gets or sets the task used to check if the expiration date has passed
        /// </summary>
        private static Task ExpirationTask { get; set; }
        #endregion


        /// <summary>
        /// Creates an authorization request
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static async Task AuthenticateAsync(string username, string password)
        {
            // Clear token response
            TokenResponse = null;

            using (var client = new HttpClient())
            {
                // Prepare form values
                var values = new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "username", username },
                    { "password", password }
                };

                // Create the content to send
                var content = new FormUrlEncodedContent(values);
                // Send the post request
                var response = await client.PostAsync(GetUrl("token"), content);

                // Get the data if the response is what we want
                if (response.IsSuccessStatusCode)
                {
                    // Get the response as a string
                    var data = await response.Content.ReadAsStringAsync();

                    // Before updating the token, kill the current task
                    await CancelExpirationTask();

                    // Deserialize the data and get the access token
                    TokenResponse = JsonConvert.DeserializeObject<OAuthTokenResponse>(data);

                    // Store the username and password for future use
                    Username = username;
                    Password = password;

                    // Initialize new cancellation token
                    ExpirationTaskCancelTokenSource = new CancellationTokenSource();

                    // Start a task that will check the expiration date of the access token, when
                    // the current time has passed token expiration, an event will be raised.
                    ExpirationTask = Task.Factory.StartNew(async () =>
                    {
                        // Get local date from expiration
                        var expiration = TimeZoneInfo.ConvertTime(TokenResponse.Expiration, TimeZoneInfo.Local);

                        // Store handle to event
                        var handle = AccessTokenExpired;

                        // Enter a while loop and check that the expiration date hasn't passed
                        while (DateTime.Now < expiration && handle != null && !ExpirationTaskCancelTokenSource.IsCancellationRequested)
                        {
                            // Wait a bit, then check again
                            //ExpirationTaskCancelTokenSource.Token.ThrowIfCancellationRequested();

                            // Wait a bit, then check again
                            await Task.Delay(1000);
                        }

                        // If the loop exits, then we have expired
                        if (!ExpirationTaskCancelTokenSource.IsCancellationRequested)
                        {
                            handle?.Invoke(null, EventArgs.Empty);
                        }

                    }, ExpirationTaskCancelTokenSource.Token);
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
        public static async Task<SuccessResponse> CreateAccountAsync(string username, string password)
        {
            using (var client = new HttpClient())
            {
                //create object to pass
                var values = new
                {
                    email = username,
                    password = password
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
        public static async Task<SuccessResponse> SaveProfileAsync(ApiProfile profile, string passphrase = null)
        {
            using (var client = new HttpClient())
            {
                // Authorize the request.
                await AuthorizeRequest(client);
                // Add passphrase to message
                AddPassphraseHeader(client, passphrase);
                // Create JSON content.
                var content = JsonContent(profile);

                // Send request and get a response
                HttpResponseMessage response;

                if (profile.Id == 0)
                {
                    // Create
                    response = await client.PostAsync(GetUrl($"api/profiles"), content);
                }
                else
                {
                    // Update
                    response = await client.PutAsync(GetUrl($"api/profiles/{profile.Id}"), content);
                }


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
                // Create JSON content.
                var content = JsonContent(newPassphrase);

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
        /// <param name="id"></param>
        /// <param name="token"></param>
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
                //authorize the request
                await AuthorizeRequest(client);
                // Add passphrase to message
                AddPassphraseHeader(client);
                //send request and get a response
                var response = await client.GetAsync(GetUrl($"api/profiles/{CurrentProfile.Id}/categories/with-items"));
                //read the data
                var data = await response.Content.ReadAsStringAsync();

                //deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    //deserialize the response as an ApiResponse object
                    return JsonConvert.DeserializeObject<CategoryResponse>(data);
                }
                else
                {
                    var wer = JsonConvert.DeserializeObject<WebExceptionResponse>(data);
                    // Throw exception with the WebExceptionResponse
                    throw wer.ToException();
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
                //authorize the request
                await AuthorizeRequest(client);
                // Add passphrase to message
                AddPassphraseHeader(client);
                //send request and get a response
                var response = await client.GetAsync(GetUrl($"api/profiles/{CurrentProfile.Id}/categories"));
                //read the data
                var data = await response.Content.ReadAsStringAsync();

                //deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    //deserialize the response as an ApiResponse object
                    return JsonConvert.DeserializeObject<CategoryResponse>(data);
                }
                else
                {
                    var wer = JsonConvert.DeserializeObject<WebExceptionResponse>(data);
                    // Throw exception with the WebExceptionResponse
                    throw wer.ToException();
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
                    // Read the data
                    var data = await response.Content.ReadAsStringAsync();

                    var wer = JsonConvert.DeserializeObject<WebExceptionResponse>(data);
                    // Throw exception with the WebExceptionResponse
                    throw wer.ToException();
                }
            }
        }
        #endregion

        #region Items

        ///// <summary>
        ///// Gets all categories for the authenticated user
        ///// </summary>
        ///// <returns></returns>
        //public static async Task<ItemsResponse> GetItemsAsync(int categoryId)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        //authorize the request
        //        await AuthorizeRequest(client);
        //        // Add passphrase to message
        //        AddPassphraseHeader(client);
        //        //send request and get a response
        //        var response = await client.GetAsync(GetUrl($"api/profiles/{CurrentProfile.Id}/categories/{categoryId}/items"));
        //        //read the data
        //        var data = await response.Content.ReadAsStringAsync();

        //        //deserialize the object based on the result
        //        if (response.IsSuccessStatusCode)
        //        {
        //            //deserialize the response as an ApiResponse object
        //            return JsonConvert.DeserializeObject<ItemsResponse>(data);
        //        }
        //        else
        //        {
        //            var wer = JsonConvert.DeserializeObject<WebExceptionResponse>(data);
        //            // Throw exception with the WebExceptionResponse
        //            throw wer.ToException();
        //        }
        //    }

        //}

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
                //authorize the request
                await AuthorizeRequest(client);
                // Add passphrase to message
                AddPassphraseHeader(client);
                //send request and get a response
                var response = await client.GetAsync(GetUrl($"api/profiles/{CurrentProfile.Id}/categories/{categoryId}/items/{itemId}"));
                //read the data
                var data = await response.Content.ReadAsStringAsync();

                //deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    //deserialize the response as an ApiResponse object
                    return JsonConvert.DeserializeObject<ItemsResponse>(data);
                }
                else
                {
                    var wer = JsonConvert.DeserializeObject<WebExceptionResponse>(data);
                    // Throw exception with the WebExceptionResponse
                    throw wer.ToException();
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

                // Get the response content
                var data = await response.Content.ReadAsStringAsync();

                // Check if the response is a success code
                if (response.IsSuccessStatusCode)
                {
                    return new SuccessResponse(Convert.ToInt32(data));
                }
                else
                {
                    var wer = JsonConvert.DeserializeObject<WebExceptionResponse>(data);
                    // Throw exception with the WebExceptionResponse
                    throw wer.ToException();
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

                // Get the response content
                var data = await response.Content.ReadAsStringAsync();

                // Check if the response is a success code
                if (!response.IsSuccessStatusCode)
                {
                    var wer = JsonConvert.DeserializeObject<WebExceptionResponse>(data);
                    // Throw exception with the WebExceptionResponse
                    throw wer.ToException();
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

                // Get the response content
                var data = await response.Content.ReadAsStringAsync();

                // Check if the response is a success code
                if (response.IsSuccessStatusCode)
                {
                    return new SuccessResponse(Convert.ToInt32(data));
                }
                else
                {
                    var wer = JsonConvert.DeserializeObject<WebExceptionResponse>(data);
                    // Throw exception with the WebExceptionResponse
                    throw wer.ToException();
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
                //authorize the request
                await AuthorizeRequest(client);
                // Add passphrase to message
                AddPassphraseHeader(client);
                //send request and get a response
                var response = await client.GetAsync(GetUrl($"api/profiles/{CurrentProfile.Id}/categories/{categoryId}/items/{itemId}/fields"));
                //read the data
                var data = await response.Content.ReadAsStringAsync();

                //deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    //deserialize the response as an ApiResponse object
                    return JsonConvert.DeserializeObject<FieldsResponse>(data);
                }
                else
                {
                    var wer = JsonConvert.DeserializeObject<WebExceptionResponse>(data);
                    // Throw exception with the WebExceptionResponse
                    throw wer.ToException();
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

                // Get the response content
                var data = await response.Content.ReadAsStringAsync();

                // Check if the response is a success code
                if (!response.IsSuccessStatusCode)
                {
                    var wer = JsonConvert.DeserializeObject<WebExceptionResponse>(data);
                    // Throw exception with the WebExceptionResponse
                    throw wer.ToException();
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
                exception = new WebException("Access denied due to invalid credentials.");
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
            if (TokenResponse != null && !string.IsNullOrWhiteSpace(TokenResponse.AccessToken))
            {

                var expiration = TimeZoneInfo.ConvertTime(TokenResponse.Expiration, TimeZoneInfo.Local);
                // Before we execute the request, make sure the token isn't about to expire
                if (DateTime.Now >= expiration.AddSeconds(-30) && DateTime.Now < expiration)
                {
                    // We are about to lose our access, reauthenticate with saved credentials
                    await AuthenticateAsync(Username, Password);
                    // Make sure we have a token
                    if (TokenResponse == null)
                    {
                        // Authentication failed
                        throw new WebException("Authentication failed");
                    }
                }

                //add the authorize header to the request
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenResponse.AccessToken);
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
        private static async Task CancelExpirationTask()
        {
            if (ExpirationTaskCancelTokenSource != null)
            {
                // Cancel task
                ExpirationTaskCancelTokenSource.Cancel();
                // Wait for the task to complete
                await ExpirationTask;
            }
        }

        /// <summary>
        /// Closes the current profile
        /// </summary>
        public static void CloseProfile()
        {
            CurrentProfile = null;
            Passphrase = null;

            // TODO: Should we raise an event that the user can handle? Such as going to another page?
        }

        /// <summary>
        /// Signs out of the api
        /// </summary>
        public static async Task SignOutAsync()
        {
            // Cancel task
            await CancelExpirationTask();

            // Clean up
            TokenResponse = null;
            CurrentProfile = null;
            Passphrase = null;
            Username = null;
            Password = null;

            // TODO: Should we raise an event that the user can handle? Such as going to another page?

        }
        #endregion

    }
}
