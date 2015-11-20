using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.API.Responses
{
    class WebExceptionResponse : ApiResponse
    {
        public string Message { get; set; }
        
        public IDictionary<string, string[]> ModelState { get; set; }
    }
}
