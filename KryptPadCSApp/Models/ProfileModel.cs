
using KryptPad.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.Models
{
    class ProfileModel : BaseModel
    {
        #region Properties
        private ApiProfile _profile;

        /// <summary>
        /// Gets the underlying field that backs this model
        /// </summary>
        public ApiProfile Profile
        {
            get { return _profile; }
        }

        /// <summary>
        /// Gets or sets the ID of the field
        /// </summary>
        public int Id
        {
            get { return _profile.Id; }
            set { _profile.Id = value; }
        }

        /// <summary>
        /// Gets or sets the name of the field. e.g Password
        /// </summary>
        public string Name
        {
            get { return _profile.Name; }
            set
            {
                _profile.Name = value;
                // Notify change
                OnPropertyChanged(nameof(Name));
            }
        }

        private bool _windowsHelloEnabled;
        /// <summary>
        /// Gets whether Windows Hello is turned on for this profile
        /// </summary>
        public bool WindowsHelloEnabled
        {
            get { return _windowsHelloEnabled; }
            set
            {
                _windowsHelloEnabled = value;
                // Notify change
                OnPropertyChanged(nameof(WindowsHelloEnabled));
            }
        }

        #endregion

        #region Constructor
        public ProfileModel(ApiProfile profile)
        {
            _profile = profile ?? throw new ArgumentNullException(nameof(profile));

        }
        #endregion
    }
}
