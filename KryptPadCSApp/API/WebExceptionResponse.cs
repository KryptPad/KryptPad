using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.API
{
    class WebExceptionResponse : ApiResponse
    {
        public ModelState ModelState { get; set; }
    }
}
