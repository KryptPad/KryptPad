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
        private static string ServiceHost { get; } = "http://localhost:50821/";

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

                }else
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
        public static async Task<ApiResponse> GetProfilesAsync(string token)
        {
            using (var client = new HttpClient())
            {
                //authorize the request
                AuthorizeRequest(client, token);
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
        public static async Task<ApiResponse> GetProfile(int id, string token)
        {
            using (var client = new HttpClient())
            {
                //authorize the request
                AuthorizeRequest(client, token);
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

        /// <summary>
        /// Creates a POST request
        /// </summary>
        /// <returns></returns>
        private static HttpWebRequest CreatePostRequest(string uri)
        {
            var request = WebRequest.CreateHttp($"{ServiceHost}{uri}");
            //set method to POST
            request.Method = "POST";

            return request;
        }

        /// <summary>
        /// Creates a GET request
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static HttpWebRequest CreateGetRequest(string uri, string token = null)
        {
            var request = WebRequest.CreateHttp($"{ServiceHost}{uri}");
            //set method to GET
            request.Method = "GET";

            //add the token as an authorization header
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers["Authorization"] = $"Bearer {token}";
            }

            return request;
        }

        /// <summary>
        /// Writes the values in a NameValueCollection to the request stream
        /// </summary>
        /// <param name="request"></param>
        /// <param name="values"></param>
        private static async Task WritePostValues(HttpWebRequest request, NameValueCollection values)
        {
            //write the values to the request
            var stream = await request.GetRequestStreamAsync();
            //write the values to the stream
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(CreateQueryString(values));
            }
        }

        /// <summary>
        /// Writes the serialized values of an object
        /// </summary>
        /// <param name="request"></param>
        /// <param name="values"></param>
        private static async Task WritePostValues(HttpWebRequest request, object values)
        {
            request.ContentType = "application/json";
            //write the values to the request
            var stream = await request.GetRequestStreamAsync();
            //write the values to the stream
            using (var sw = new StreamWriter(stream))
            {

                //serialize object
                var data = JsonConvert.SerializeObject(values);

                sw.Write(data);
            }
        }

        private static async Task<string> GetStringData(WebException exception)
        {
            var response = exception.Response;

            return await GetStringDataAsync(response);
        }

        /// <summary>
        /// Executes the webrequest
        /// </summary>
        /// <param name="request"></param>
        public static async Task<string> GetStringDataAsync(WebResponse response)
        {
            //get the data from the response
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                var data = await sr.ReadToEndAsync();
                //return the data
                return data;
            }
        }

        /// <summary>
        /// Takes a NameValueCollection and creates a query string
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private static string CreateQueryString(NameValueCollection values)
        {
            var list = new List<string>();

            foreach (var k in values.AllKeys)
            {
                list.Add($"{k}={WebUtility.UrlEncode(values[k])}");
            }

            return string.Join("&", list);
        }

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
