using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.Models
{
    /// <summary>
    /// Represents the working document
    /// </summary>
    class Document : BaseModel
    {
        /// <summary>
        /// Gets the collection of categories
        /// </summary>
        public CategoryCollection Categories { get; private set; } = new CategoryCollection();
    }
}
