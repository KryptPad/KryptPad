using KryptPadCSApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.API.Responses
{
    class ItemResponse : ApiResponse
    {
        public ApiItem[] Items { get; set; }
    }
}
