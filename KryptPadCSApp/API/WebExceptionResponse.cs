using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.API
{
    class WebExceptionResponse : ApiResponse
    {
        public string Message { get; set; }
        
        public ModelState ModelState { get; set; }
    }
}
