using KryptPad.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.Classes
{
    /// <summary>
    /// Represents a pre-build field for an item template
    /// </summary>
    public class ItemTemplateField
    {
        /// <summary>
        /// Gets or sets the field type for this field
        /// </summary>
        public FieldType FieldType { get; set; }

        /// <summary>
        /// Gets or sets the name for this field
        /// </summary>
        public string Name { get; set; }
    }
}
