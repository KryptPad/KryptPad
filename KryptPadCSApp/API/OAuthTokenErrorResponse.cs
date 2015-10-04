using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.API
{
    class OAuthTokenErrorResponse : ApiResponse
    {
        public string error { get; set; }
        public string error_description { get; set; }
    }
}
