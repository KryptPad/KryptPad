using KryptPadCSApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.Models
{
    class EditItemPageParams : PageNavigationParamsBase
    {
        public ApiCategory Category { get; set; }
        public ApiItem Item { get; set; }
    }
}
