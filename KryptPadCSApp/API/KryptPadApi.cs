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
using System.Threading.Tasks;

namespace KryptPadCSApp.API
{

    /// <summary>
    /// Accesses the KryptPad API
    /// </summary>
    class KryptPadApi
    {
        /// <summary>
        /// Gets the host address of the API service. Must be changed before going live
        /// </summary>
        private static string ServiceHost { get; } = "http://localhost:50821/"; // "https://www.kryptpad.com/"; //

        /// <summary>
        /// Creates an authorization request
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static async Task<ApiResponse> AuthenticateAsync(string username, string password)
        {
            using (var client = new HttpClient())
            {
                //prepare form values
                var values = new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "username", username },
                    { "password", password }
                };

                //create the content to send
                var content = new FormUrlEncodedContent(values);
                //send the post request
                var response = await client.PostAsync(GetUrl("token"), content);

                //get the response as a string
                var data = await response.Content.ReadAsStringAsync();

                //get the data if the response is what we want
                if (response.IsSuccessStatusCode)
                {
                    //deserialize the data and get the access token
                    return JsonConvert.DeserializeObject<OAuthTokenResponse>(data);

                }
                else
                {
                    //deserialize the data and get the access token
                    return JsonConvert.DeserializeObject<OAuthTokenErrorResponse>(data);
                }
            }

        }

        /// <summary>
        /// Creates an account in the system
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static async Task<ApiResponse> CreateAccountAsync(string username, string password)
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

                //get the response content
                var data = await response.Content.ReadAsStringAsync();

                //check if the response is a success code
                if (response.IsSuccessStatusCode)
                {
                    return new SuccessResponse();
                }
                else
                {
                    return JsonConvert.DeserializeObject<WebExceptionResponse>(data);
                }
            }

        }

        /// <summary>
        /// Gets all profiles for the authenticated user
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<ApiResponse> GetProfilesAsync(string token, string passphrase)
        {
            using (var client = new HttpClient())
            {
                //authorize the request
                AuthorizeRequest(client, token);
                
                // TODO: TEST
                client.DefaultRequestHeaders.Add("Passphrase", passphrase);

                //send request and get a response
                var response = await client.GetAsync(GetUrl("api/profiles"));
                //read the data
                var data = await response.Content.ReadAsStringAsync();

                //deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    //deserialize the response as an ApiResponse object
                    return JsonConvert.DeserializeObject<ProfileResponse>(data);
                }
                else
                {
                    return JsonConvert.DeserializeObject<WebExceptionResponse>(data);
                }
            }

        }

        /// <summary>
        /// Gets a specific profile for the authenticated user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<ApiResponse> GetProfileAsync(int id, string token, string passphrase)
        {
            using (var client = new HttpClient())
            {
                //authorize the request
                AuthorizeRequest(client, token);

                // TODO: TEST
                client.DefaultRequestHeaders.Add("Passphrase", passphrase);

                //send request and get a response
                var response = await client.GetAsync(GetUrl($"api/profiles/{id}"));

                //read the data
                var data = await response.Content.ReadAsStringAsync();

                //deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    //deserialize the response as an ApiResponse object
                    return JsonConvert.DeserializeObject<ProfileResponse>(data);
                }
                else
                {
                    return JsonConvert.DeserializeObject<WebExceptionResponse>(data);
                }
            }

        }

        public static async Task<ApiResponse> CreateProfile(ApiProfile profile, string token, string passphrase)
        {
            using (var client = new HttpClient())
            {
                // Authorize the request.
                AuthorizeRequest(client, token);
                // Create JSON content.
                var content = JsonContent(profile);

                // TODO: TEST
                client.DefaultRequestHeaders.Add("Passphrase", passphrase);

                // Send request and get a response
                var response = await client.PostAsync(GetUrl($"api/profiles"), content);

                // Read the data
                var data = await response.Content.ReadAsStringAsync();

                // Deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response as an ApiResponse object
                    return JsonConvert.DeserializeObject<ProfileResponse>(data);
                }
                else
                {
                    return JsonConvert.DeserializeObject<WebExceptionResponse>(data);
                }
            }

        }

        /// <summary>
        /// Deletes the profile
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteProfile(int id, string token)
        {
            using (var client = new HttpClient())
            {
                // Authorize the request.
                AuthorizeRequest(client, token);

                // Send request and get a response
                var response = await client.DeleteAsync(GetUrl($"api/profiles/{id}"));

                // Deserialize the object based on the result
                return response.IsSuccessStatusCode;
            }

        }

        #region Categories
        /// <summary>
        /// Gets all categories for the authenticated user
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<ApiResponse> GetCategoriesAsync(int profileId, string token)
        {
            using (var client = new HttpClient())
            {
                //authorize the request
                AuthorizeRequest(client, token);
                //send request and get a response
                var response = await client.GetAsync(GetUrl($"api/profiles/{profileId}/categories"));
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
                    return JsonConvert.DeserializeObject<WebExceptionResponse>(data);
                }
            }

        }

        public static async Task<ApiResponse> CreateCategoryAsync(ApiProfile profile, ApiCategory category, string token)
        {
            using (var client = new HttpClient())
            {
                // Authorize the request.
                AuthorizeRequest(client, token);
                // Create JSON content.
                var content = JsonContent(category);
                // Send request and get a response
                var response = await client.PostAsync(GetUrl($"api/profiles/{profile.Id}/categories"), content);

                // Read the data
                var data = await response.Content.ReadAsStringAsync();

                // Deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response as an ApiResponse object
                    return JsonConvert.DeserializeObject<CategoryResponse>(data);
                }
                else
                {
                    return JsonConvert.DeserializeObject<WebExceptionResponse>(data);
                }
            }

        }
        #endregion


        #region Items
        /// <summary>
        /// Gets all categories for the authenticated user
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<ApiResponse> GetItemsAsync(int profileId, int categoryId, string token)
        {
            using (var client = new HttpClient())
            {
                //authorize the request
                AuthorizeRequest(client, token);
                //send request and get a response
                var response = await client.GetAsync(GetUrl($"api/profiles/{profileId}/categories/{categoryId}/items"));
                //read the data
                var data = await response.Content.ReadAsStringAsync();

                //deserialize the object based on the result
                if (response.IsSuccessStatusCode)
                {
                    //deserialize the response as an ApiResponse object
                    return JsonConvert.DeserializeObject<ItemResponse>(data);
                }
                else
                {
                    return JsonConvert.DeserializeObject<WebExceptionResponse>(data);
                }
            }

        }

        /// <summary>
        /// Creates a new item in the specified category
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="categoryId"></param>
        /// <param name="item"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<ApiResponse> CreateItemAsync(int profileId, int categoryId, ApiItem item, string token)
        {
            using (var client = new HttpClient())
            {

                // Authorize the request.
                AuthorizeRequest(client, token);

                // Create content to send
                var content = JsonContent(item);

                // Execute request
                var response = await client.PostAsync(GetUrl($"api/profiles/{profileId}/categories/{categoryId}/items"), content);

                // Get the response content
                var data = await response.Content.ReadAsStringAsync();

                // Check if the response is a success code
                if (response.IsSuccessStatusCode)
                {
                    return new SuccessResponse();
                }
                else
                {
                    return JsonConvert.DeserializeObject<WebExceptionResponse>(data);
                }


            }

        }

        #endregion
        #region Helper methods

        /// <summary>
        /// Authorizes an http client request
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        private static void AuthorizeRequest(HttpClient client, string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                //add the authorize header to the request
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
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

        //public static bool TestResponse<T>(ApiResponse response)
        //{
        //    if (response is T)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// Takes a NameValueCollection and creates a query string
        ///// </summary>
        ///// <param name="values"></param>
        ///// <returns></returns>
        //private static string CreateQueryString(NameValueCollection values)
        //{
        //    var list = new List<string>();

        //    foreach (var k in values.AllKeys)
        //    {
        //        list.Add($"{k}={WebUtility.UrlEncode(values[k])}");
        //    }

        //    return string.Join("&", list);
        //}

        ///// <summary>
        ///// Takes an object and creates a query string from the public properties
        ///// </summary>
        ///// <param name="values"></param>
        ///// <returns></returns>
        //private static string CreateQueryString(object values)
        //{
        //    var list = new List<string>();

        //    foreach (var k in values.AllKeys)
        //    {
        //        list.Add($"{k}={WebUtility.UrlEncode(values[k])}");
        //    }

        //    return string.Join("&", list);
        //}
        #endregion


    }
}
