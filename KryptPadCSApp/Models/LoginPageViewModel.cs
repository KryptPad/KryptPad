using KryptPad.Api;
using KryptPad.Api.Responses;
using KryptPadCSApp.Classes;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace KryptPadCSApp.Models
{
    class LoginPageViewModel : BasePageModel
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
        private string _email;

        /// <summary>
        /// Gets or sets the email address
        /// </summary>
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                // Notify change
                OnPropertyChanged(nameof(Email));
                // Is login enabled?
                LogInCommand.OnCanExecuteChanged();
            }
        }

        private string _password;
        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                // Notify change
                OnPropertyChanged(nameof(Password));
                // Is login enabled?
                LogInCommand.OnCanExecuteChanged();
            }
        }

        private bool _autoSignIn;
        /// <summary>
        /// Gets or sets the auto login value
        /// </summary>
        public bool AutoSignIn
        {
            get { return _autoSignIn; }
            set
            {
                _autoSignIn = value;
                // Notify change
                OnPropertyChanged(nameof(AutoSignIn));

            }
        }

        /// <summary>
        /// Gets the url of the website that the API is connected to
        /// </summary>
        public string WebsiteUrl
        {
            get { return KryptPadApi.ServiceHost; }
        }

        public Command LogInCommand { get; protected set; }

        public Command CreateAccountCommand { get; protected set; }

        public Command GoToFacebookCommand { get; protected set; }

        public Command GoToTwitterCommand { get; protected set; }

        #endregion

        public LoginPageViewModel()
        {
            // Ensure that the access token is cleared upon arrival
            KryptPadApi.SignOutAsync();

            (App.Current as App).SignInStatus = SignInStatus.SignedOut;

            // Register commands
            RegisterCommands();

        }

        /// <summary>
        /// Create instance of the view model
        /// </summary>
        /// <returns></returns>
        public async Task AutoLoginAsync()
        {
            // Check the password vault for any saved credentials.
            await LoginFromSavedCredentialsAsync();

        }

        #region Helper Methods

        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            LogInCommand = new Command(LogInCommandHandler, IsLoginEnabled);
            CreateAccountCommand = new Command(CreateAccountCommandHandler);
            GoToFacebookCommand = new Command(GoToFacebookCommandHandler);
            GoToTwitterCommand = new Command(GoToTwitterCommandHandler);
        }
        
        /// <summary>
        /// Checks to see if there are any saved credentials. If there are, the app is auto-logged in.
        /// Only logs in if there is no access token already
        /// </summary>
        private async Task LoginFromSavedCredentialsAsync()
        {
            // Create instance to credential locker
            var locker = new PasswordVault();

            try
            {

                // Find the saved credentials
                var login = locker.FindAllByResource(LOCKER_RESOURCE).FirstOrDefault();

                if (login != null)
                {
                    // Get the users password
                    login.RetrievePassword();
                    // Set properties
                    Email = login.UserName;
                    Password = login.Password;
                    AutoSignIn = true;

                    // If autologin is enabled, attempt to sign in
                    if (!DisableAutoLogin)
                    {
                        // Do login
                        await LoginAsync();

                        // Disable the auto login
                        DisableAutoLogin = true;
                    }

                }
            }
            catch { /* Nothing to see here */ }

        }

        /// <summary>
        /// Stores the user's credentials in the credential locker
        /// </summary>
        private void SaveCredentialsIfAutoSignIn()
        {
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


            // If we chose auto sign in, save the new credential
            if (AutoSignIn)
            {
                // Create new credential
                var credential = new PasswordCredential()
                {
                    Resource = LOCKER_RESOURCE,
                    UserName = Email,
                    Password = Password
                };

                // Store the credentials
                locker.Add(credential);
            }

        }

        /// <summary>
        /// Performs login
        /// </summary>
        /// <returns></returns>
        private async Task LoginAsync()
        {
            IsBusy = true;

            try
            {
                //log in and get access token
                await KryptPadApi.AuthenticateAsync(Email, Password);

                //save credentials
                SaveCredentialsIfAutoSignIn();

                //navigate to the select profile page
                NavigationHelper.Navigate(typeof(SelectProfilePage), null);

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

        /// <summary>
        /// Gets whether the login command is enabled
        /// </summary>
        /// <returns></returns>
        private bool IsLoginEnabled(object p) => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
        #endregion

        #region Command handlers

        private async void LogInCommandHandler(object p)
        {
            await LoginAsync();
        }

        private void CreateAccountCommandHandler(object p)
        {
            // Navigate to the create account page
            NavigationHelper.Navigate(typeof(CreateAccountPage), null);
        }

        private async void GoToFacebookCommandHandler(object obj)
        {
            try
            {
                // Launch the uri
                await Windows.System.Launcher.LaunchUriAsync(new Uri(ResourceHelper.GetString("FacebookUrl")));

            }
            catch (Exception)
            {
                // Failed
                await DialogHelper.ShowMessageDialogAsync(ResourceHelper.GetString("UriFail"));
            }
        }

        private async void GoToTwitterCommandHandler(object obj)
        {
            try
            {
                // Launch the uri
                await Windows.System.Launcher.LaunchUriAsync(new Uri(ResourceHelper.GetString("TwitterUrl")));

            }
            catch (Exception)
            {
                // Failed
                await DialogHelper.ShowMessageDialogAsync(ResourceHelper.GetString("UriFail"));
            }
        }

        #endregion

        #region Public methods
        public async Task SendForgotPasswordLinkAsync()
        {
            try
            {
                // Get the email address
                var email = await DialogHelper.GetValueAsync(null, "Email", Email);
                if (email != null)
                {
                    // Log in and get access token
                    await KryptPadApi.SendForgotPasswordLinkAsync(email);

                    await DialogHelper.ShowMessageDialogAsync("If your email address is associated to your account, you should recieve an email with password reset instructions.");
                }
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
        }
        #endregion
    }
}
