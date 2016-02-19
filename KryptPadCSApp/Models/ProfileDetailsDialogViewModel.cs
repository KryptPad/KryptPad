using KryptPadCSApp.API;
using KryptPadCSApp.API.Models;
using KryptPadCSApp.Classes;
using KryptPadCSApp.Dialogs;
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
                try
                {
                    var errors = new List<string>();
                    // Check password
                    if (ProfilePassphrase.Length < 8)
                    {
                        // Opps, passphrase doesn't meet criteria
                        errors.Add("Passphrase must be at least eight characters long.");

                    }
                    else if (!ProfilePassphrase.Equals(ConfirmProfilePassphrase))
                    {
                        // Opps, passphrase doesn't meet criteria
                        errors.Add("Passphrase and Confirm Passphrase must match.");
                    }

                    if (errors.Count > 0)
                    {
                        var dialog = p as ProfileDetailsDialog;

                        dialog.Cancel = true;

                        var errorMsg = string.Join("\n", errors);
                        // Show message if we have any errors
                        await DialogHelper.ShowMessageDialogAsync(errorMsg);

                        return;
                    }

                    //create a new profile
                    var profile = new ApiProfile()
                    {
                        Name = Name
                    };

                    // Call api to create the profile.
                    var response = await KryptPadApi.SaveProfileAsync(profile, AccessToken, ProfilePassphrase);
                }
                catch (Exception ex)
                {
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
