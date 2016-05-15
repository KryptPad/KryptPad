using KryptPadCSApp.API;
using KryptPadCSApp.API.Models;
using KryptPadCSApp.Classes;
using KryptPadCSApp.Dialogs;
using KryptPadCSApp.Views;
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
                SaveCommand.CommandCanExecute = CanSaveProfile;
            }
        }

        private string _profilePassphrase;

        public string ProfilePassphrase
        {
            get { return _profilePassphrase; }
            set
            {
                _profilePassphrase = value;
                // Notify change
                OnPropertyChanged(nameof(ProfilePassphrase));
                // Can we execute command?
                SaveCommand.CommandCanExecute = CanSaveProfile;
            }
        }
        private string _confirmProfilePassphrase;

        public string ConfirmProfilePassphrase
        {
            get { return _confirmProfilePassphrase; }
            set
            {
                _confirmProfilePassphrase = value;
                // Notify change
                OnPropertyChanged(nameof(ConfirmProfilePassphrase));
                // Can we execute command?
                SaveCommand.CommandCanExecute = CanSaveProfile;

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
                // Get reference to dialog
                var dialog = p as ClosableContentDialog;

                try
                {
                    
                    //create a new profile
                    var profile = new ApiProfile()
                    {
                        Name = Name
                    };

                    // Call api to create the profile.
                    var response = await KryptPadApi.SaveProfileAsync(profile, ProfilePassphrase);

                    profile.Id = response.Id;
                    
                    // Go to profile
                    await KryptPadApi.LoadProfileAsync(profile, ProfilePassphrase);

                    // Redirect to the main item list page
                    NavigationHelper.Navigate(typeof(ItemsPage), null);
                    // Clear the back stack
                    NavigationHelper.ClearBackStack();
                }
                catch (Exception ex)
                {
                    // Cancel closing
                    dialog.Cancel = true;
                    
                    // Operation failed
                    await DialogHelper.ShowMessageDialogAsync(ex.Message);
                                        
                }

            }, false);


        }

        private bool CanSaveProfile => !string.IsNullOrWhiteSpace(Name)
            && !string.IsNullOrWhiteSpace(ProfilePassphrase)
            && !string.IsNullOrWhiteSpace(ConfirmProfilePassphrase);

        #endregion

    }
}
