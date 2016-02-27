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

        public Command RenameProfileCommand { get; protected set; }

        public Command DeleteProfileCommand { get; protected set; }

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
                            
                            var frame = Window.Current.Content as Frame;

                            Window.Current.Content = new MainPage(frame);

                            // When a profile is selected, navigate to main page
                            Navigate(typeof(ItemsPage));
                            //clear stack
                            //frame.SetNavigationState("1,0");
                            frame.BackStack.Clear();
                            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;


                        }
                    }
                    catch(Exception ex)
                    {
                        // Operation failed
                        await DialogHelper.ShowMessageDialogAsync(ex.Message);
                    }
                    
                    
                });

            });

            RenameProfileCommand = new Command(async (p) => {

                // Prompt for name
                await DialogHelper.ShowDialog<NamePromptDialog>(async (d) =>
                {
                    try
                    {
                        //create new category
                        var profile = p as ApiProfile;

                        // Set new name
                        profile.Name = d.Value;

                        // Send the category to the api
                        var resp = await KryptPadApi.SaveProfileAsync(profile);

                        // Refresh the view
                        Profiles.RefreshItem(profile);
                    }
                    catch (Exception ex)
                    {
                        // Operation failed
                        await DialogHelper.ShowMessageDialogAsync(ex.Message);
                    }
                }, "Rename Profile");
            });

            DeleteProfileCommand = new Command(async (p) =>
            {

                var res = await DialogHelper.Confirm(
                    "All of your data in this profile will be deleted permanently. This action CANNOT be undone. Are you sure you want to delete this profile?",
                    async (ap) =>
                        {

                            var profile = p as ApiProfile;
                            if (profile != null)
                            {
                                // Delete the selected profile
                                var response = await KryptPadApi.DeleteProfileAsync(profile);

                                if (response)
                                {
                                    // Remove the deleted profile from the list
                                    Profiles.Remove(profile);
                                }
                            }

                        }
                );


            });

            SignOutCommand = new Command((p) => {
                // Sign out
                NavigationHelper.Navigate(typeof(LoginPage), null, true);
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
