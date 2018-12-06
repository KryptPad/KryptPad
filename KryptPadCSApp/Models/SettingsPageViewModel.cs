using KryptPad.Api;
using KryptPadCSApp.Classes;
using KryptPadCSApp.Views;
using System;
using System.Net;
using Windows.Security.Credentials;

namespace KryptPadCSApp.Models
{
    class SettingsPageViewModel : BasePageModel
    {
        /// <summary>
        /// Resource name for credential locker
        /// </summary>
#if DEBUG
        private const string LOCKER_RESOURCE = "KryptPadTest";
#else
        private const string LOCKER_RESOURCE = "KryptPad";
#endif

        #region Properties

        private bool _isSignedIn;
        /// <summary>
        /// Gets or sets the current item for editing
        /// </summary>
        public bool IsSignedIn
        {
            get { return _isSignedIn; }
            set
            {
                _isSignedIn = value;
                //notify change
                OnPropertyChanged(nameof(IsSignedIn));

            }
        }

        public Command DeleteAccountCommand { get; protected set; }

        #endregion

        #region Constructor
        public SettingsPageViewModel()
        {
            // Register commands
            RegisterCommands();

            // This is used to restrict or hide certain settings
            IsSignedIn = (App.Current as App).SignInStatus == SignInStatus.SignedIn;
        }
        #endregion

        #region Commands
        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            DeleteAccountCommand = new Command(DeleteAccountCommandHandlerAsync);
            
        }

        private async void DeleteAccountCommandHandlerAsync(object obj)
        {
            IsBusy = true;

            try
            {
                // Log in and get access token
                await KryptPadApi.DeleteAccountAsync();

                // Create instance to credential locker
                var locker = new PasswordVault();

                try
                {
                    // Clear out the saved credential for the resource
                    var creds = locker.FindAllByResource(LOCKER_RESOURCE);
                    foreach (var cred in creds)
                    {
                        // Remove only the credentials for the given resource
                        locker.Remove(cred);
                    }
                }
                catch { /* Nothing to see here */ }

                // Navigate to the login page
                NavigationHelper.Navigate(typeof(LoginPage), null);

            }
            catch (WebException ex)
            {
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
    }
}
