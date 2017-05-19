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

        private bool _savePassphraseEnabled;

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
                if (value != null && SavePassphraseEnabled)
                {
                    // Introduce windows hello
                    PromptForPin(value.Id.ToString());

                }

                // Show the passphrase box if an item is selected
                PassphrasePromptVisibility = value != null ? Visibility.Visible : Visibility.Collapsed;
                
            }
        }

        private Visibility _windowsHelloVisibility;
        /// <summary>
        /// Gets or sets the Windows Hello checkbox visibility
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

        public Command SavePassphraseCheckedCommand { get; protected set; }

        #endregion

        #region Methods


        public SelectProfilePageViewModel()
        {
            RegisterCommands();

            // Set the profile selection visibility
            ProfileSelectionVisible = Visibility.Collapsed;
            PassphrasePromptVisibility = Visibility.Collapsed;
            WindowsHelloVisibility = Visibility.Collapsed;

            // Ensure the profile is closed and stored passphrase is cleared
            KryptPadApi.CloseProfile();

            // Success, tell the app we are not signed in
            (App.Current as App).IsSignedIn = false;
        }

        /// <summary>
        /// Checks if Windows Hello is supported and enabled the checkbox
        /// </summary>
        /// <returns></returns>
        public async Task CheckIfWindowsHelloSupported()
        {
            var supported = await IsWindowsHelloEnabled();
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
            } else if(supported)
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

        #endregion

        #region Command handlers
        private async void EnterProfileCommandHandler(object obj)
        {
            await EnterProfile();
        }

        private void SavePassphraseCheckedCommandHandler(object obj)
        {
            // If the user unchecks this checkbox, clear any saved passphrases
            if (!SavePassphraseEnabled)
            {
                // Wipe out any passphrases
                ClearAllSavedPassphrases();
                // Deselect the profile
                SelectedProfile = null;
            }
        }

        #endregion

        #region Helper methods

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

            SavePassphraseCheckedCommand = new Command(SavePassphraseCheckedCommandHandler);

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
        private async Task EnterProfile()
        {
            try
            {
                // Check the profile and determine if the passphrase is correct
                await KryptPadApi.LoadProfileAsync(SelectedProfile, Passphrase);


                // Success, tell the app we are signed in
                (App.Current as App).IsSignedIn = true;

                // Check if Windows hellow is supported and save the passphrase
                var supported = await IsWindowsHelloEnabled();
                if (supported && SavePassphraseEnabled)
                {
                    // Prompt to save profile passphrase if Windows Hello is enabled
                    StorePassphrase(SelectedProfile.Id.ToString());
                }

                // When a profile is selected, navigate to main page
                NavigationHelper.Navigate(typeof(ItemsPage), null);

            }
            catch (WebException)
            {
                // Something went wrong in the api
                await DialogHelper.ShowMessageDialogAsync("Sorry, you were either logged out or you entered the wrong passphrase.");

                // Clear out the passphrase
                Passphrase = null;
            }
            catch (Exception)
            {
                // Failed
                await DialogHelper.ShowConnectionErrorMessageDialog();
            }
        }

        /// <summary>
        /// Returns true if the user has a PIN set and Windows Hello is enabled
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsWindowsHelloEnabled()
        {
            return await KeyCredentialManager.IsSupportedAsync();
        }

        /// <summary>
        /// Verifies the user's identity and retrieves the selected profile's passphrase
        /// </summary>
        /// <param name="profileId"></param>
        private async void PromptForPin(string profileId)
        {
            // Create instance to credential locker
            var locker = new PasswordVault();

            try
            {
                // Clear out the saved credential for the resource
                var login = locker.FindAllByUserName($"Profile_{profileId}").FirstOrDefault();
                if (login != null)
                {
                    // We have a stored passphrase, verify the user's identity before releasing it.
                    UserConsentVerificationResult consentResult = await UserConsentVerifier.RequestVerificationAsync("Verify your identity to unlock your profile.");
                    if (consentResult == UserConsentVerificationResult.Verified)
                    {
                        // Verified. Get the passphrase.
                        login.RetrievePassword();
                        // Set the passphrase field
                        Passphrase = login.Password;
                        // Enter the profile with the saved passphrase
                        await EnterProfile();

                        return;
                    }
                    else
                    {
                        // Deselect the profile to try again
                        SelectedProfile = null;
                    }
                    
                }
                
            }
            catch { /* Nothing to see here */ }
            
        }

        /// <summary>
        /// Clears any saved passphrases
        /// </summary>
        private async void ClearAllSavedPassphrases()
        {
            await DialogHelper.Confirm(
                "Unchecking this option will clear any saved passphrases. You will have to re-type them. Are you sure?", 
                "Confirm",
                (c) => {
                    try
                    {
                        // Create instance to credential locker
                        var locker = new PasswordVault();
                        // Clear out the saved credential for the resource
                        var logins = locker.FindAllByResource("Profiles");
                        foreach (var login in logins)
                        {
                            locker.Remove(login);
                        }

                    } catch { }
                    
                });

            
        }

        /// <summary>
        /// Stores the passphrase of the profile
        /// </summary>
        /// <param name="profileId"></param>
        private void StorePassphrase(string profileId)
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


        }
        #endregion

        private bool CanLogIn(object p) => !string.IsNullOrWhiteSpace(Passphrase) && SelectedProfile != null;

    }
}
