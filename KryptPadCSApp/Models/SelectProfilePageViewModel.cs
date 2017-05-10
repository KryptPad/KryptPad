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
                // Notify
                OnPropertyChanged(nameof(SelectedProfile));
                // Enable login button}
                EnterProfileCommand.OnCanExecuteChanged();
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


        public Command CreateProfileCommand { get; protected set; }

        public Command EnterProfileCommand { get; protected set; }

        public Command RestoreBackupCommand { get; protected set; }

        #endregion

        public SelectProfilePageViewModel()
        {
            RegisterCommands();

            // Set the profile selection visibility
            ProfileSelectionVisible = Visibility.Collapsed;

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
                SelectedProfile = Profiles.FirstOrDefault();

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

        #region Command handlers
        private async void EnterProfileCommandHandler(object p)
        {
            try
            {
                // Check the profile and determine if the passphrase is correct
                await KryptPadApi.LoadProfileAsync(SelectedProfile, Passphrase);

                // Success, tell the app we are signed in
                (App.Current as App).IsSignedIn = true;

                // When a profile is selected, navigate to main page
                //NavigationHelper.Navigate(typeof(ItemsPage), null);

                // Introduce windows hello
                RegisterUser(SelectedProfile.Id.ToString());
                //var keyCredentialAvailable = await KeyCredentialManager.IsSupportedAsync();
                //if (!keyCredentialAvailable)
                //{
                //    // Something went wrong in the api
                //    await DialogHelper.ShowMessageDialogAsync("To use Windows Hello, set up your PIN.");
                //    // User didn't set up PIN yet
                //    return;
                //}
                //else
                //{
                //    // Set up Windows Hello
                //}


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
        #endregion

        #region Helper methods
        private async void RegisterUser(string AccountId)
        {
            var keyCredentialAvailable = await KeyCredentialManager.IsSupportedAsync();
            if (!keyCredentialAvailable)
            {
                // The user didn't set up a PIN yet
                return;
            }

            var keyCreationResult = await KeyCredentialManager.RequestCreateAsync(AccountId, KeyCredentialCreationOption.ReplaceExisting);
            if (keyCreationResult.Status == KeyCredentialStatus.Success)
            {
                var userKey = keyCreationResult.Credential;
                var publicKey = userKey.RetrievePublicKey();

                IBuffer keyAttestation = null;
                IBuffer certificateChain = null;
                bool keyAttestationIncluded = false;
                bool keyAttestationCanBeRetrievedLater = false;

                KeyCredentialAttestationResult keyAttestationResult = await userKey.GetAttestationAsync();
                KeyCredentialAttestationStatus keyAttestationRetryType = 0;

                if (keyAttestationResult.Status == KeyCredentialAttestationStatus.Success)
                {
                    keyAttestationIncluded = true;
                    keyAttestation = keyAttestationResult.AttestationBuffer;
                    certificateChain = keyAttestationResult.CertificateChainBuffer;
                }
                else if (keyAttestationResult.Status == KeyCredentialAttestationStatus.TemporaryFailure)
                {
                    keyAttestationRetryType = KeyCredentialAttestationStatus.TemporaryFailure;
                    keyAttestationCanBeRetrievedLater = true;
                }
                else if (keyAttestationResult.Status == KeyCredentialAttestationStatus.NotSupported)
                {
                    keyAttestationRetryType = KeyCredentialAttestationStatus.NotSupported;
                    keyAttestationCanBeRetrievedLater = true;
                }
            }
            else if (keyCreationResult.Status == KeyCredentialStatus.UserCanceled ||
                keyCreationResult.Status == KeyCredentialStatus.UserPrefersPassword)
            {
                // Show error message to the user to get confirmation that user
                // does not want to enroll.
            }

            var openKeyResult = await KeyCredentialManager.OpenAsync(AccountId);
            //openKeyResult.Credential.

        }
        #endregion

        private bool CanLogIn(object p) => !string.IsNullOrWhiteSpace(Passphrase) && SelectedProfile != null;

    }
}
