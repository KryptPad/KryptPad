using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.Models
{
    class Field : BaseModel
    {
        #region Properties
        private string _name;
        /// <summary>
        /// Gets or sets the name of the field
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                //notify change
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _value;
        /// <summary>
        /// Gets or sets the name of the field
        /// </summary>
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                //notify change
                OnPropertyChanged(nameof(Value));
            }
        }

        #endregion
    }
}
