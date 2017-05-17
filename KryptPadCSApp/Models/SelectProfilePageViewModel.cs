using KryptPad.Api;
using KryptPad.Api.Models;
using KryptPad.Api.Responses;
using KryptPadCSApp.Classes;
using KryptPadCSApp.Collections;
using KryptPadCSApp.Dialogs;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Security.Credentials.UI;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
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

        private string _passphrase;

        public string Passphrase
        {
            get { return _passphrase; }
            set
            {
                _passphrase = value;

                // Changed
                OnPropertyChanged(nameof(Passphrase));

                // Enable login button
                EnterProfileCommand.OnCanExecuteChanged();

            }
        }

        private ApiProfile _selectedProfile;
        /// <summary>
        /// Gets or sets the selected profile
        /// </summary>
        public ApiProfile SelectedProfile
        {
            get { return _selectedProfile; }
            set
            {
                _selectedProfile = value;

                // Clear passphrase
                Passphrase = null;

                // Notify
                OnPropertyChanged(nameof(SelectedProfile));
                // Enable login button}
                EnterProfileCommand.OnCanExecuteChanged();

                // Verify identity through Windows Hello. If the user is authenticated, then
                // release the saved passphrase and automatically enter the profile.

                // If Windows Hello is not available, then the user must enter the passphrase
                // manually.
                if (value != null)
                {
                    // Introduce windows hello
                    PromptForPin(value.Id.ToString());
                    
                }
            }
        }

        private Visibility _profileSelectionVisible;
        /// <summary>
        /// Gets or sets whether the profile selection is visible
        /// </summary>
        public Visibility ProfileSelectionVisible
        {
            get { return _profileSelectionVisible; }
            set
            {
                _profileSelectionVisible = value;
                // Changed
                OnPropertyChanged(nameof(ProfileSelectionVisible));
            }
        }

        private Visibility _passphrasePromptVisibility;
        /// <summary>
        /// Gets or sets the visibility of the passphrase box
        /// </summary>
        public Visibility PassphrasePromptVisibility
        {
            get { return _passphrasePromptVisibility; }
            set
            {
                _passphrasePromptVisibility = value;
                // Changed
                OnPropertyChanged(nameof(PassphrasePromptVisibility));
            }
        }


        public Command CreateProfileCommand { get; protected set; }

        public Command EnterProfileCommand { get; protected set; }

        public Command RestoreBackupCommand { get; protected set; }

        #endregion

        public SelectProfilePageViewModel()
        {
            RegisterCommands();

            // Set the profile selection visibility
            ProfileSelectionVisible = Visibility.Collapsed;
            PassphrasePromptVisibility = Visibility.Collapsed;

            // Ensure the profile is closed and stored passphrase is cleared
            KryptPadApi.CloseProfile();

            // Success, tell the app we are not signed in
            (App.Current as App).IsSignedIn = false;
        }

        /// <summary>
        /// Gets the profiles for the user
        /// </summary>
        /// <returns></returns>
        public async Task GetProfilesAsync()
        {

            IsBusy = true;

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

                // Set the selected profile.
                // TODO: Make this restore last selected profile... somehow
                //SelectedProfile = Profiles.FirstOrDefault();

                // If we don't have any profiles, hide the selection
                ProfileSelectionVisible = Profiles.Any() ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (WebException ex)
            {
                // Something went wrong in the api
                await DialogHelper.ShowMessageDialogAsync(ex.Message);
            }
            catch (Exception)
            {
                // Failed
                await DialogHelper.ShowConnectionErrorMessageDialog();
            }

            IsBusy = false;
        }

        /// <summary>
        /// Registers commands
        /// </summary>
        private void RegisterCommands()
        {
            CreateProfileCommand = new Command(async (p) =>
            {
                // Prompt the user for profile info
                var dialog = new ProfileDetailsDialog();

                var result = await dialog.ShowAsync();
            });

            EnterProfileCommand = new Command(EnterProfileCommandHandler, CanLogIn);

            RestoreBackupCommand = new Command(async (p) =>
            {
                try
                {
                    var fop = new FileOpenPicker();
                    // Add supported file types
                    fop.FileTypeFilter.Add(".kdf");

                    // Pick file to open and read
                    var result = await fop.PickSingleFileAsync();

                    if (result != null)
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

                        await GetProfilesAsync();

                    }
                }
                catch (WebException ex)
                {
                    // Something went wrong in the api
                    await DialogHelper.ShowMessageDialogAsync(ex.Message);
                }
                catch (Exception)
                {
                    // Failed
                    await DialogHelper.ShowConnectionErrorMessageDialog();
                }


            });
        }

        /// <summary>
        /// Decrypts the user's profile and logs in
        /// </summary>
        /// <returns></returns>
        private async Task EnterProfile(bool promptToSave)
        {
            try
            {
                // Check the profile and determine if the passphrase is correct
                await KryptPadApi.LoadProfileAsync(SelectedProfile, Passphrase);

                // Success, tell the app we are signed in
                (App.Current as App).IsSignedIn = true;

                if (promptToSave)
                {
                    // Prompt to save profile passphrase if Windows Hello is enabled
                    await StorePassphrase(SelectedProfile.Id.ToString());
                }

                // When a profile is selected, navigate to main page
                NavigationHelper.Navigate(typeof(ItemsPage), null);

            }
            catch (WebException)
            {
                // Something went wrong in the api
                await DialogHelper.ShowMessageDialogAsync("The passphrase you entered is incorrect.");

                // Clear out the passphrase
                Passphrase = null;
            }
            catch (Exception)
            {
                // Failed
                await DialogHelper.ShowConnectionErrorMessageDialog();
            }
        }

        #region Command handlers
        private async void EnterProfileCommandHandler(object p)
        {
            await EnterProfile(true);
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Verifies the user's identity and retrieves the selected profile's passphrase
        /// </summary>
        /// <param name="profileId"></param>
        private async void PromptForPin(string profileId)
        {
            // Check to see if we have a stored passphrase for this profile
            // Create instance to credential locker
            var locker = new PasswordVault();

            try
            {
                // Clear out the saved credential for the resource
                var login = locker.FindAllByUserName($"Profile_{profileId}").FirstOrDefault();
                if (login != null)
                {
                    // We have a stored passphrase, verify the user's identity before releasing it.
                    UserConsentVerificationResult consentResult = await UserConsentVerifier.RequestVerificationAsync("userMessage");
                    if (consentResult == UserConsentVerificationResult.Verified)
                    {
                        // Verified. Get the passphrase.
                        login.RetrievePassword();
                        // Set the passphrase field
                        Passphrase = login.Password;
                        // Enter the profile with the saved passphrase
                        await EnterProfile(false);

                        return;
                    }
                    else if (consentResult == UserConsentVerificationResult.DeviceNotPresent)
                    {
                        // Windows Hello isn't available. Perhaps we should inform the user to set a PIN
                    }
                    
                }
                else
                {
                    // No credentials found
                }
            }
            catch { /* Nothing to see here */ }

            // Show passphrase box
            PassphrasePromptVisibility = Visibility.Visible;

        }

        /// <summary>
        /// Stores the passphrase of the profile
        /// </summary>
        /// <param name="profileId"></param>
        private async Task StorePassphrase(string profileId)
        {
            var keyCredentialAvailable = await KeyCredentialManager.IsSupportedAsync();
            if (!keyCredentialAvailable)
            {
                // The user didn't set up a PIN yet
                return;
            }

            // If the user wants to save password, store in credential locker, otherwise, exit.
            var res = await DialogHelper.Confirm("Would you like Krypt Pad to remember your passphrase (requires unlocking with PIN)?", (a) =>
            {
                // The user has Windows Hello set up, so let's store the passphrase
                // Create instance to credential locker
                var locker = new PasswordVault();
                var profile = $"Profile_{profileId}";

                try
                {
                    // Clear out the saved credential for the resource
                    var login = locker.FindAllByUserName(profile).FirstOrDefault();
                    if (login != null)
                    {
                        // Remove only the credentials for the given resource
                        locker.Remove(login);
                    }
                }
                catch { /* Nothing to see here */ }

                // Create new credential
                var credential = new PasswordCredential()
                {
                    Resource = "Profiles",
                    UserName = profile,
                    Password = Passphrase
                };

                // Store the credentials
                locker.Add(credential);
            });

        }
        #endregion

        private bool CanLogIn(object p) => !string.IsNullOrWhiteSpace(Passphrase) && SelectedProfile != null;

    }
}
