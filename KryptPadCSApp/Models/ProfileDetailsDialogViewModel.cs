using KryptPadCSApp.API;
using KryptPadCSApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.Models
{
    class ProfileDetailsDialogViewModel : BasePageModel
    {
        #region Properties
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                // Notify change.
                OnPropertyChanged(nameof(Name));
                // Can we execute command?
                SaveCommand.CommandCanExecute = !string.IsNullOrWhiteSpace(_name);
            }
        }

        public Command SaveCommand { get; protected set; }

        #endregion

        public ProfileDetailsDialogViewModel()
        {
            // Register commands
            RegisterCommands();
        }

        #region Helper Methods

        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            SaveCommand = new Command(async (p) =>
            {
                try
                {

                    //create a new profile
                    var profile = new ApiProfile()
                    {
                        Name = Name
                    };

                    // Call api to create the profile.
                    var response = await KryptPadApi.SaveProfile(profile, AccessToken, Passphrase);
                }
                catch (Exception)
                {

                }

            }, false);


        }


        #endregion

    }
}
