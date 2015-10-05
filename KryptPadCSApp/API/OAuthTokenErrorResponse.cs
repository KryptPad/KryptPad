using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KryptPadCSApp.API
{
    class OAuthTokenErrorResponse : ApiResponse
    {

        public string Error { get; set; }

        [JsonProperty(PropertyName ="error_description")]
        public string ErrorDescription { get; set; }
    }
}
