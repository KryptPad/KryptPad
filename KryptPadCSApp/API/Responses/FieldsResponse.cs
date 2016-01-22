using KryptPadCSApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.API.Responses
{
    class FieldsResponse : ApiResponse
    {
        public ApiField[] Fields { get; set; }
    }
}
