using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KryptPad.Api.Responses
{
    class EndpointConfigurationResponse
    {
        [JsonProperty(PropertyName = "token_endpoint")]
        public string TokenEndpoint { get; set; }
    }
}
