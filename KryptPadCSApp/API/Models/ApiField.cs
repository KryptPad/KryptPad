using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.API.Models
{
    public class ApiField
    {
        /// <summary>
        /// Gets or sets the ID of the field
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the field. e.g Password
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Base64 encoded cipher text
        /// </summary>
        public string Value { get; set; }
    }
}
