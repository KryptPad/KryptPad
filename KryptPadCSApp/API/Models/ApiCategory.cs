using KryptPadCSApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.API.Models
{
    class ApiCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ItemCollection Items { get; protected set; } = new ItemCollection();

        public ApiProfile Profile { get; set; }
    }
}
