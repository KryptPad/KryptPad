using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.API.Models
{
    class ApiFieldType
    {
        public FieldType Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the field type
        /// </summary>
        public string Name { get; set; }
    }
}
