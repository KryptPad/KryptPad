using KryptPadCSApp.API;
using KryptPadCSApp.API.Models;
using KryptPadCSApp.API.Responses;
using KryptPadCSApp.Classes;
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
        public ObservableCollection<ApiProfile> Profiles { get; protected set; } = new ObservableCollection<ApiProfile>();

        private ApiProfile _selectedProfile;

        public ApiProfile SelectedProfile
        {
            get { return _selectedProfile; }
            set
            {
                _selectedProfile = value;

                // Set the selected profile
                CurrentProfile = _selectedProfile;

                //// Notify changes
                //OnPropertyChanged(nameof(SelectedProfile));
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


        public Command CreateProfileCommand { get; protected set; }

        public Command DeleteProfileCommand { get; protected set; }

        #endregion

        public SelectProfilePageViewModel()
        {
            RegisterCommands();

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
                var response = await KryptPadApi.GetProfilesAsync(AccessToken, "12345678");

                // Check response
                if (response is ProfileResponse)
                {
                    var profiles = (response as ProfileResponse).Profiles;

                    // Clear the profiles list
                    Profiles.Clear();
                    // Add the profiles to the list
                    foreach (var profile in profiles)
                    {
                        // Add profile to list
                        Profiles.Add(profile);
                    }
                }
                else
                {
                    throw new Exception();
                }

            }
            catch (Exception)
            {
                await DialogHelper.ShowConnectionErrorMessageDialog();

            }
        }

        private void RegisterCommands()
        {
            CreateProfileCommand = new Command(async (p) =>
            {
                // Prompt the user for profile info
                await PromptForProfileInfo();
            });

            DeleteProfileCommand = new Command(async (p) =>
            {

                var msg = new MessageDialog("All of your data in this profile will be deleted permanently. This action CANNOT be undone. Are you sure you want to delete this profile?", "CONFIRM DELETE");
                msg.Commands.Add(new UICommand("Yes", async (ap) =>
                {

                    var profile = p as ApiProfile;
                    if (profile != null)
                    {
                        // Delete the selected profile
                        var response = await KryptPadApi.DeleteProfile(profile.Id, AccessToken);

                        if (response)
                        {
                            // Remove the deleted profile from the list
                            Profiles.Remove(profile);
                        }
                    }

                }));
                msg.Commands.Add(new UICommand("No"));
                msg.DefaultCommandIndex = 1;

                await msg.ShowAsync();

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
