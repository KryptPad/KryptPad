using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.API.Models
{
    class ApiItem
    {
        public int Id { get; set; }
        public int ItemType { get; set; }
        public string Name { get; set; }

        public ApiCategory Category { get; set; }
    }
}
