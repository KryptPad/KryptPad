using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.API.Responses
{
    class ApiResponse
    {

        #region Properties
        public HttpStatusCode StatusCode { get; set; }
        #endregion

        #region Helper Methods
        
        public static async Task<ApiResponse> Ok()
        {
            return await Task.Factory.StartNew(() =>
            {
                return new ApiResponse()
                {
                    StatusCode = HttpStatusCode.OK
                };
            });
        }

        /// <summary>
        /// Creates a WebException response from a WebException
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static async Task<ApiResponse> CreateWebExceptionResponse(WebException exception)
        {
            return await CreateApiWebResponse<WebExceptionResponse>(exception.Response as HttpWebResponse);
        }

        /// <summary>
        /// Creates an OAuthTokenErrorResponse from a WebException
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static async Task<ApiResponse> CreateOAuthTokenErrorResponse(WebException exception)
        {
            return await CreateApiWebResponse<OAuthTokenErrorResponse>(exception.Response as HttpWebResponse);   
        }

        /// <summary>
        /// Creates an OAuthTokenResponse from a HttpWebResponse
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static async Task<ApiResponse> CreateOAuthTokenResponse(HttpWebResponse response)
        {
            return await CreateApiWebResponse<OAuthTokenResponse>(response);

        }

        /// <summary>
        /// Creates a generic error response
        /// </summary>
        /// <returns></returns>
        public static ApiResponse CreateGenericErrorResponse()
        {
            return new WebExceptionResponse()
            {
                Message = "Unable to connect to the server."
            };
        }
        
        /// <summary>
        /// Creates an ApiResponse object from an HttpWebResponse
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        private static async Task<ApiResponse> CreateApiWebResponse<T>(HttpWebResponse response) where T : ApiResponse
        {
            if (response != null)
            {
                //get the data from the response
                var data = await KryptPadApi.GetStringDataAsync(response);
                //deserialize data
                var apiResponse = JsonConvert.DeserializeObject<T>(data);
                //set status code
                apiResponse.StatusCode = response.StatusCode;
                //return the re
                return apiResponse;
            }
            else
            {
                return CreateGenericErrorResponse();
            }
        }
        #endregion

    }
}
