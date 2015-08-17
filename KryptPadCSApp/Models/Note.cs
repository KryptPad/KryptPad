using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.Models
{
    class Note : BaseModel, IItem
    {
        #region Properties

        private string _name;
        /// <summary>
        /// Gets or sets the name of the profile
        /// </summary>
        public string Name
        {
            get
            { return _name; }
            set
            {
                _name = value;
                //raise change event
                OnPropertyChanged(nameof(Name));
            }
        }

        #endregion
    }
}
