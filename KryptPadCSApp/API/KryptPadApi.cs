using KryptPadCSApp.API.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
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

            try
            {
                //create a request
                var request = CreatePostRequest("token");
                //create the values to send
                var values = new NameValueCollection();
                values.Add("grant_type", "password");
                values.Add("username", username);
                values.Add("password", password);
                
                //write these values to the request
                await WritePostValues(request, values);
                //execute the request
                var response = await request.GetResponseAsync() as HttpWebResponse;
                
                //create OAuth token response
                return await ApiResponse.CreateOAuthTokenResponse(response);
            }
            catch (WebException ex)
            {
                return await ApiResponse.CreateOAuthTokenErrorResponse(ex);
            }
            catch (Exception)
            {
                return ApiResponse.CreateGenericErrorResponse();
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
            try
            {
                //create a request
                var request = CreatePostRequest("api/account/register");

                var values = new
                {
                    email = username,
                    password = password
                };


                //write these values to the request
                await WritePostValues(request, values);

                //execute the request
                var response = await request.GetResponseAsync();

                //check the response. if OK then the operation succeeded
                return await ApiResponse.Ok();

            }
            catch (WebException ex)
            {
                return await ApiResponse.CreateWebExceptionResponse(ex);

            }
            catch (Exception)
            {
                return ApiResponse.CreateGenericErrorResponse();
            }
        }

        public static async Task<ApiResponse> GetProfilesAsync(string token)
        {
            try
            {
                //create a request
                var request = CreateGetRequest("api/profiles", token);

                //execute the request
                var response = await request.GetResponseAsync();

                //get the returned response
                var data = await GetStringDataAsync(response);

                return await ApiResponse.Ok();
            }
            catch (WebException ex)
            {
                return await ApiResponse.CreateWebExceptionResponse(ex);
            }
            catch(Exception)
            {
                return ApiResponse.CreateGenericErrorResponse();
            }
            
        }

        #region Helper methods

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
