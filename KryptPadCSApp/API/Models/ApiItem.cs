using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.API.Models
{
    public class ApiItem
    {
        /// <summary>
        /// Gets or sets the ID of the item
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the category
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the name of the item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the notes of the item
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or set the fields for the item
        /// </summary>
        public ApiField[] Fields { get; set; }
    }
}
