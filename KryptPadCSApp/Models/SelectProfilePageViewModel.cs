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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                await PromptForProfileInfo();
            });

            SelectProfileCommand = new Command(async (p) => {
                // Prompt for passphrase
                await DialogHelper.ShowDialog<PassphrasePrompt>(async (d) =>
                {

                    try
                    {
                        // Check the profile and determine if the passphrase is correct
                        var success = await KryptPadApi.LoadProfileAsync(p as ApiProfile, d.Passphrase);

                        if (success)
                        {
                            // When a profile is selected, navigate to main page
                            NavigationHelper.Navigate(typeof(ItemsPage), null, NavigationHelper.NavigationType.Frame);
                            // Clear the back stack
                            NavigationHelper.ClearBackStack();
                       
                        }
                    }
                    catch(Exception ex)
                    {
                        // Operation failed
                        await DialogHelper.ShowMessageDialogAsync(ex.Message);
                    }
                    
                    
                });

            });
            
            SignOutCommand = new Command((p) => {
                // Sign out
                NavigationHelper.Navigate(typeof(LoginPage), null, NavigationHelper.NavigationType.Window);
            });
        }

        private async Task PromptForProfileInfo()
        {
            var dialog = new ProfileDetailsDialog();

            var result = await dialog.ShowAsync();

            // If the user clicked the primary button, try to get the profile
            if (result == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                // Get profiles
                await GetProfiles();
            }
        }

    }
}
