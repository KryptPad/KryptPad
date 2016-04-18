using KryptPadCSApp.API;
using KryptPadCSApp.API.Models;
using KryptPadCSApp.API.Responses;
using KryptPadCSApp.Classes;
using KryptPadCSApp.Collections;
using KryptPadCSApp.Dialogs;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace KryptPadCSApp.Models
{
    class SelectProfilePageViewModel : BasePageModel
    {
        #region Properties

        /// <summary>
        /// Gets the list of profiles for a user
        /// </summary>
        public ProfileCollection Profiles { get; protected set; } = new ProfileCollection();

        public Command CreateProfileCommand { get; protected set; }

        public Command SelectProfileCommand { get; protected set; }

        public Command SignOutCommand { get; protected set; }

        public Command RestoreBackupCommand { get; protected set; }

        #endregion

        public SelectProfilePageViewModel()
        {
            RegisterCommands();

            // Ensure the passphrase is cleared
            KryptPadApi.CloseProfile();

#if DEBUG
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) { return; }
#endif

            // Get the list of profiles
            var t = GetProfiles();

        }

        /// <summary>
        /// Gets the profiles for the user
        /// </summary>
        /// <returns></returns>
        private async Task GetProfiles()
        {
            // Call the api and get some data!
            try
            {
                var resp = await KryptPadApi.GetProfilesAsync();

                // Clear the profiles list
                Profiles.Clear();
                // Add the profiles to the list
                foreach (var profile in resp.Profiles)
                {
                    // Add profile to list
                    Profiles.Add(profile);
                }


            }
            catch (Exception ex)
            {
                // Operation failed
                await DialogHelper.ShowMessageDialogAsync(ex.Message);
            }
        }

        private void RegisterCommands()
        {
            CreateProfileCommand = new Command(async (p) =>
            {
                // Prompt the user for profile info
                var dialog = new ProfileDetailsDialog();

                var result = await dialog.ShowAsync();
            });

            SelectProfileCommand = new Command(async (p) =>
            {
                // Prompt for passphrase
                await DialogHelper.ShowDialog<PassphrasePrompt>(async (d) =>
                {

                    try
                    {
                        // Check the profile and determine if the passphrase is correct
                        await KryptPadApi.LoadProfileAsync(p as ApiProfile, d.Passphrase);

                        // When a profile is selected, navigate to main page
                        NavigationHelper.Navigate(typeof(ItemsPage), null, NavigationHelper.NavigationType.Frame);
                        // Clear the back stack
                        NavigationHelper.ClearBackStack();

                    }
                    catch (Exception ex)
                    {
                        // Operation failed
                        await DialogHelper.ShowMessageDialogAsync(ex.Message);
                    }


                });

            });

            SignOutCommand = new Command((p) =>
            {
                // Sign out
                NavigationHelper.Navigate(typeof(LoginPage), null, NavigationHelper.NavigationType.Window);
            });

            RestoreBackupCommand = new Command(async (p) => {
                var fop = new FileOpenPicker();
                // Add supported file types
                fop.FileTypeFilter.Add(".kdf");

                // Pick file to open and read
                var result = await fop.PickSingleFileAsync();

                if (result != null )
                {
                    var fs = await result.OpenReadAsync();
                    string profileData;
                    // Create a stream reader
                    using (var sr = new StreamReader(fs.AsStreamForRead()))
                    {
                        profileData = await sr.ReadToEndAsync();

                    }

                    // Upload the profile data
                    var resp = await KryptPadApi.UploadProfile(profileData);

                    await DialogHelper.ShowMessageDialogAsync("Profile restored successfully");

                    await GetProfiles();

                }

            });
        }
        
    }
}
