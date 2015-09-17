using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.Models
{
    class EditItemPageParams : PageNavigationParamsBase
    {
        public Category Category { get; set; }
        public ItemBase Item { get; set; }
    }
}
