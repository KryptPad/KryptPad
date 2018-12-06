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
        const int MAX_SAVED_PROFILES = 5;
        const string LOCKER_RESOURCE = "Profiles";

        #region Properties

        /// <summary>
        /// Gets the list of profiles for a user
        /// </summary>
        public ProfileCollection Profiles { get; protected set; } = new ProfileCollection();

        private bool _savePassphraseEnabled;

        /// <summary>
        /// Gets or sets whether the save passphrase feature is enabled.
        /// </summary>
        public bool SavePassphraseEnabled
        {
            get { return _savePassphraseEnabled; }
            set
            {
                _savePassphraseEnabled = value;

                // Save setting
                // Create a simple setting
                var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
                roamingSettings.Values["SavePassphraseEnabled"] = value;

                // Changed
                OnPropertyChanged(nameof(SavePassphraseEnabled));

            }
        }

        private Visibility _windowsHelloVisibility;
        /// <summary>
        /// Gets or sets the Windows Hello checkbox visibility. Shows up only if Windows Hello is enabled for the device.
        /// </summary>
        public Visibility WindowsHelloVisibility
        {
            get { return _windowsHelloVisibility; }
            set
            {
                _windowsHelloVisibility = value;
                // Changed
                OnPropertyChanged(nameof(WindowsHelloVisibility));
            }
        }


        private Visibility _profileSelectionVisible;
        /// <summary>
        /// Gets or sets whether the profile selection is visible.
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

        public Command ProfileSelectedCommand { get; protected set; }

        public Command CreateProfileCommand { get; protected set; }

        public Command RestoreBackupCommand { get; protected set; }

        public Command SavePassphraseCheckedCommand { get; protected set; }

        public Command DeleteSavedPassphraseCommand { get; protected set; }

        #endregion

        #region Methods

        public SelectProfilePageViewModel()
        {
            RegisterCommands();

            // Set the profile selection visibility
            ProfileSelectionVisible = Visibility.Collapsed;
            WindowsHelloVisibility = Visibility.Collapsed;

            // Ensure the profile is closed and stored passphrase is cleared
            KryptPadApi.CloseProfile();

            // Success, tell the app we are not signed in with a profile
            (App.Current as App).SignInStatus = SignInStatus.SignedIn;
        }

        /// <summary>
        /// Checks if Windows Hello is supported and enabled the checkbox
        /// </summary>
        /// <returns></returns>
        public async Task CheckIfWindowsHelloSupported()
        {
            var supported = await KeyCredentialManager.IsSupportedAsync();
            // Enable or disable the checkbox
            WindowsHelloVisibility = supported ? Visibility.Visible : Visibility.Collapsed;

            // Look up saved settings for option
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;

            // Read setting
            var settingValue = roamingSettings.Values["SavePassphraseEnabled"];
            if (settingValue != null)
            {
                // Override checkbox checked value
                SavePassphraseEnabled = (bool)settingValue;
            }
            else if (supported)
            {
                // Preselect the checkbox
                SavePassphraseEnabled = true;
            }


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
                // Create instance to credential locker
                var locker = new PasswordVault();

                // Clear the profiles list
                Profiles.Clear();
                // Add the profiles to the list
                foreach (var profile in resp.Profiles)
                {

                    var profileModel = new ProfileModel(profile)
                    {
                        WindowsHelloEnabled = HasSavedPassphrase(locker, profile.Id.ToString())
                    };

                    // Add profile to list
                    Profiles.Add(profileModel);
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
            catch (Exception ex)
            {
                // Failed
                await DialogHelper.ShowGenericErrorDialogAsync(ex);
            }

            IsBusy = false;
        }

        #endregion

        #region Command handlers

        /// <summary>
        /// Registers commands
        /// </summary>
        private void RegisterCommands()
        {
            ProfileSelectedCommand = new Command(ProfileSelectedCommandHandler);

            CreateProfileCommand = new Command(CreateProfileCommandHandler);

            SavePassphraseCheckedCommand = new Command(SavePassphraseCheckedCommandHandler);

            RestoreBackupCommand = new Command(RestoreBackupCommandHandler);

            DeleteSavedPassphraseCommand = new Command(DeleteSavedPassphraseCommandHandler);
        }

        private async void ProfileSelectedCommandHandler(object obj)
        {
            var profile = obj as ProfileModel;
            // Get whether credential manager is supported
            var credentialManagerSupported = await KeyCredentialManager.IsSupportedAsync();

            // If Windows Hello is enabled, ask for verification of consent
            if (profile != null 
                && credentialManagerSupported 
                && SavePassphraseEnabled 
                && HasSavedPassphrase(new PasswordVault(), profile?.Id.ToString()))
            {
                // Introduce windows hello
                await PromptForConsent(profile);
            }
            else
            {
                // Prompt the user for the passphrase
                await PromptForPassphrase(profile);
            }
        }

        private async void CreateProfileCommandHandler(object obj)
        {
            // Prompt the user for profile info
            var dialog = new ProfileDetailsDialog();

            var result = await dialog.ShowAsync();
        }

        private void SavePassphraseCheckedCommandHandler(object obj)
        {
            // If the user unchecks this checkbox, clear any saved passphrases
            if (!SavePassphraseEnabled)
            {
                // Wipe out any passphrases
                ClearAllSavedPassphrases();

            }
        }

        private async void RestoreBackupCommandHandler(object obj)
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

                    await DialogHelper.ShowMessageDialogAsync(ResourceHelper.GetString("ProfileRestored"));

                    await GetProfilesAsync();

                }
            }
            catch (WebException ex)
            {
                // Something went wrong in the api
                await DialogHelper.ShowMessageDialogAsync(ex.Message);
            }
            catch (Exception ex)
            {
                // Failed
                await DialogHelper.ShowGenericErrorDialogAsync(ex);
            }
        }

        private void DeleteSavedPassphraseCommandHandler(object obj)
        {
            var profile = obj as ProfileModel;

            // Delete the saved passphrase
            DeletePassphrase(profile.Id.ToString());

            // Tell profile we deleted the saved passphrase
            profile.WindowsHelloEnabled = false;

        }

        #endregion


        #region Helper methods

        /// <summary>
        /// Decrypts the user's profile and logs in
        /// </summary>
        /// <returns></returns>
        private async Task EnterProfile(ProfileModel profile, string passphrase)
        {
            try
            {
                // Check the profile and determine if the passphrase is correct
                await KryptPadApi.LoadProfileAsync(profile.Profile, passphrase);


                // Success, tell the app we are signed in
                (App.Current as App).SignInStatus = SignInStatus.SignedInWithProfile;

                // Check if Windows hellow is supported and save the passphrase
                var supported = await KeyCredentialManager.IsSupportedAsync();
                if (supported && SavePassphraseEnabled)
                {
                    // Prompt to save profile passphrase if Windows Hello is enabled
                    StorePassphrase(profile.Id.ToString(), passphrase);
                }

                // When a profile is selected, navigate to main page
                NavigationHelper.Navigate(typeof(ItemsPage), null);

            }
            catch (WebException)
            {
                // Something went wrong in the api
                await DialogHelper.ShowMessageDialogAsync(ResourceHelper.GetString("UnlockProfileFailed"));

            }
            catch (Exception ex)
            {
                // Failed
                await DialogHelper.ShowGenericErrorDialogAsync(ex);
            }
        }

        /// <summary>
        /// Verifies the user's identity and retrieves the selected profile's passphrase
        /// </summary>
        /// <param name="profileId"></param>
        private async Task PromptForConsent(ProfileModel profile)
        {
            // Create instance to credential locker
            var locker = new PasswordVault();
            var profileId = profile.Id.ToString();

            try
            {
                // Find the saved passphrase for the selected profile
                var login = GetSavedCredential(locker, profileId);
                if (login != null)
                {
                    // We have a stored passphrase, verify the user's identity before releasing it.
                    var consentResult = await UserConsentVerifier.RequestVerificationAsync(ResourceHelper.GetString("WindowsHelloConsentMessage"));
                    if (consentResult == UserConsentVerificationResult.Verified)
                    {
                        // Verified. Get the passphrase.
                        login.RetrievePassword();

                        // Enter the profile with the saved passphrase
                        await EnterProfile(profile, login.Password);

                        return;
                    }

                }

            }
            catch { /* Nothing to see here */ }

        }

        /// <summary>
        /// Prompts the user to enter a passphrase
        /// </summary>
        /// <param name="profile"></param>
        private async Task PromptForPassphrase(ProfileModel profile)
        {
            await DialogHelper.ShowClosableDialog<PassphrasePrompt>(async (d) =>
            {
                // Enter the profile with the saved passphrase
                await EnterProfile(profile, d.Passphrase);
            });

        }

        #region PasswordVault methods

        /// <summary>
        /// Gets the saved credential for the profile
        /// </summary>
        /// <param name="locker"></param>
        /// <param name="profileId"></param>
        /// <returns></returns>
        private PasswordCredential GetSavedCredential(PasswordVault locker, string profileId)
        {
            try
            {
                // Find the saved passphrase for the selected profile
                var login = locker.FindAllByUserName($"Profile_{profileId}").FirstOrDefault();
                if (login != null)
                {
                    return login;
                }

            }
            catch { /* Nothing to see here */ }

            return null;
        }

        /// <summary>
        /// Determines if the profile has a saved passphrase
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns></returns>
        private bool HasSavedPassphrase(PasswordVault locker, string profileId)
        {

            try
            {
                // Find the saved passphrase for the selected profile
                var login = GetSavedCredential(locker, profileId);
                if (login != null)
                {
                    return true;
                }

            }
            catch { /* Nothing to see here */ }

            return false;
        }

        /// <summary>
        /// Clears any saved passphrases
        /// </summary>
        private async void ClearAllSavedPassphrases()
        {
            var command = await DialogHelper.Confirm(
                ResourceHelper.GetString("ConfirmClearAllPassphrases"),
                ResourceHelper.GetString("Confirm"),
                (c) =>
                {
                    try
                    {
                        // Create instance to credential locker
                        var locker = new PasswordVault();
                        // Clear out the saved credential for the resource
                        var logins = locker.FindAllByResource(LOCKER_RESOURCE);
                        foreach (var login in logins)
                        {
                            locker.Remove(login);
                        }

                        // Reset flag on all profiles
                        foreach (var profile in Profiles)
                        {
                            profile.WindowsHelloEnabled = false;
                        }


                    }
                    catch { }

                });

            if ((int)command.Id == 2)
            {
                // User chose to cancel, so re-enable
                SavePassphraseEnabled = true;
            }

        }

        /// <summary>
        /// Stores the passphrase of the profile
        /// </summary>
        /// <param name="profileId"></param>
        private void StorePassphrase(string profileId, string passphrase)
        {
            
            // The user has Windows Hello set up, so let's store the passphrase
            // Create instance to credential locker
            var locker = new PasswordVault();
            var profile = $"Profile_{profileId}";

            try
            {

                // Check how many profiles we have saved already
                var allLogins = locker.FindAllByResource(LOCKER_RESOURCE);
                if (allLogins.Count == MAX_SAVED_PROFILES)
                {
                    return;
                }

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
                Resource = LOCKER_RESOURCE,
                UserName = profile,
                Password = passphrase
            };

            // Store the credentials
            locker.Add(credential);


        }

        /// <summary>
        /// Delete saved profile
        /// </summary>
        /// <param name="profileId"></param>
        private void DeletePassphrase(string profileId)
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
        }

        #endregion

        #endregion

    }
}
