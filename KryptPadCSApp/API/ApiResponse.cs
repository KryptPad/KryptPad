using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.API
{
    class ApiResponse
    {
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public static async Task<ApiResponse> Ok()
        {
            return await Task.Factory.StartNew(() =>
            {
                return new ApiResponse()
                {
                    Message = "Ok",
                    StatusCode = HttpStatusCode.OK
                };
            });
        }


        public static async Task<WebExceptionResponse> CreateWebExceptionResponse(WebException exception)
        {
            var response = exception.Response as HttpWebResponse;
            var data = await KryptPadApi.GetStringDataAsync(response);
            var resp = JsonConvert.DeserializeObject<WebExceptionResponse>(data);
            resp.StatusCode = response.StatusCode;
            return resp;
        }

        public static async Task<OAuthTokenErrorResponse> CreateOAuthTokenErrorResponse(WebException exception)
        {
            var response = exception.Response as HttpWebResponse;
            var data = await KryptPadApi.GetStringDataAsync(response);
            var resp = JsonConvert.DeserializeObject<OAuthTokenErrorResponse>(data);
            resp.StatusCode = response.StatusCode;
            return resp;
        }

    }
}
