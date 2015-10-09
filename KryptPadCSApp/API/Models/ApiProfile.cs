using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.API.Models
{
    class ApiProfile //: KryptPadCSApp.Models.BaseModel
    {
        /// <summary>
        /// Gets or sets the Id of the profile
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name of the profile
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets whether this profile is encrypted with a password other than the user's
        /// account password.
        /// </summary>
        public bool HasPassword { get; set; }
    }
}
