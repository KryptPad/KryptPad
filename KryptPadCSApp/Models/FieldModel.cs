using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.Models
{
    class FieldModel : BaseModel
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
        /// Gets or sets the value
        /// </summary>
        private string _value;

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                // Notify change
                OnPropertyChanged(nameof(Value));
            }
        }

    }
}
