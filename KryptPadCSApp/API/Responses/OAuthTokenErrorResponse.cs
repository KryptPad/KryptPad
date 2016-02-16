using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;

namespace KryptPadCSApp.API.Responses
{
    class OAuthTokenErrorResponse : ApiWebExceptionResponse
    {

        public string Error { get; set; }

        [JsonProperty(PropertyName ="error_description")]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Converts this WebExceptionResponse to an Exception object
        /// </summary>
        /// <returns></returns>
        public override WebException ToException()
        {
            return new WebException(ErrorDescription);
        }
    }
}
