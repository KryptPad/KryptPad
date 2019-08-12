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

        public Command DeauthorizeDevicesCommand { get; protected set; }

        #endregion

        #region Constructor
        public SettingsPageViewModel()
        {
            // Register commands
            RegisterCommands();

            // This is used to restrict or hide certain settings
            var signinStatus = (App.Current as App).SignInStatus;
            IsSignedIn = signinStatus != SignInStatus.SignedOut;
        }
        #endregion

        #region Commands
        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            DeleteAccountCommand = new Command(DeleteAccountCommandHandlerAsync);
            DeauthorizeDevicesCommand = new Command(DeauthorizeDevicesCommandHandlerAsync);

        }

        private async void DeleteAccountCommandHandlerAsync(object obj)
        {
            IsBusy = true;

            try
            {

                await DialogHelper.Confirm(
                    "Are you sure you want to delete your account? ALL OF YOUR DATA WILL BE DELETED! THIS ACTION CANNOT BE UNDONE.",
                    async (p) =>
                    {
                        // Log in and get access token
                        await KryptPadApi.DeleteAccountAsync();

                        // Create instance to credential locker
                        var locker = new PasswordVault();

                        try
                        {
                            // Clear out the saved credential for the resource
                            var creds = locker.FindAllByResource(Constants.LOCKER_RESOURCE);
                            foreach (var cred in creds)
                            {
                                // Remove only the credentials for the given resource
                                locker.Remove(cred);
                            }
                        }
                        catch { /* Nothing to see here */ }

                        // Navigate to the login page
                        NavigationHelper.Navigate(typeof(LoginPage), null, NavigationHelper.NavigationType.Root);
                    }
                );

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

        private async void DeauthorizeDevicesCommandHandlerAsync(object obj)
        {
            IsBusy = true;

            try
            {

                // Log in and get access token
                await KryptPadApi.DeauthorizeDevices();

                // Navigate to the login page
                NavigationHelper.Navigate(typeof(LoginPage), null, NavigationHelper.NavigationType.Root);

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
