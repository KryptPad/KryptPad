using KryptPadCSApp.API;
using KryptPadCSApp.API.Responses;
using KryptPadCSApp.Classes;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
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
                LogInCommand.CommandCanExecute = IsLoginEnabled();
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
                LogInCommand.CommandCanExecute = IsLoginEnabled();
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

        private Visibility _loginVisibility;
        /// <summary>
        /// Gets or sets whether the ui element is visible
        /// </summary>
        public Visibility LoginVisibility
        {
            get { return _loginVisibility; }
            set
            {
                _loginVisibility = value;
                //notify change
                OnPropertyChanged(nameof(LoginVisibility));

            }
        }

        public Command LogInCommand { get; protected set; }

        public Command CreateAccountCommand { get; protected set; }

        #endregion

        public LoginPageViewModel()
        {
            RegisterCommands();

            // Ensure that the access token is cleared upon arrival
            AccessToken = null;

            // Check the password vault for any saved credentials.
            LoginFromSavedCredentials();

        }


        #region Helper Methods

        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            LogInCommand = new Command(async (p) =>
            {
                await LoginAsync();
            }, false);

            CreateAccountCommand = new Command((p) =>
            {
                // Navigate to the create account page
                Navigate(typeof(CreateAccountPage));
            });
        }

        /// <summary>
        /// Checks to see if there are any saved credentials. If there are, the app is auto-logged in.
        /// Only logs in if there is no access token already
        /// </summary>
        private void LoginFromSavedCredentials()
        {


            //create instance to credential locker
            var locker = new PasswordVault();
            try
            {
                //find the saved credentials
                var login = locker.FindAllByResource("KryptPad").FirstOrDefault();

                if (login != null)
                {
                    // Get the users password
                    login.RetrievePassword();
                    // Set properties
                    Email = login.UserName;
                    Password = login.Password;

                    // Make sure we can auto login in, and that we don't already have an access token
                    if (!DisableAutoLogin && AccessToken == null)
                    {
                        // Do login
                        var t = LoginAsync();

                        // Disable the auto login
                        DisableAutoLogin = true;
                    }

                }
            }
            catch (Exception)
            {
                //no saved credentials, ignore
            }

        }

        private void SaveCredentialsIfAutoSignIn()
        {
            if (AutoSignIn)
            {
                //create instance to credential locker
                var locker = new PasswordVault();

                //clear out any saved credentials
                locker.RetrieveAll().ToList().ForEach((l) => locker.Remove(l));

                //create new credential
                var credential = new PasswordCredential()
                {
                    Resource = "KryptPad",
                    UserName = Email,
                    Password = Password
                };

                //store the credentials
                locker.Add(credential);
            }
        }

        private async Task LoginAsync()
        {
            IsBusy = true;

            try
            {
                //log in and get access token
                var response = await KryptPadApi.AuthenticateAsync(Email, Password);
                
                //store the access token
                AccessToken = (response as OAuthTokenResponse).AccessToken;

                //save credentials
                SaveCredentialsIfAutoSignIn();

                //navigate to the select profile page
                Navigate(typeof(SelectProfilePage));


            }
            catch (Exception ex)
            {
                //await DialogHelper.ShowMessageDialogAsync(
                //"Your username or password is incorrect.");
                await DialogHelper.ShowConnectionErrorMessageDialog();
            }


            IsBusy = false;
        }

        /// <summary>
        /// Gets whether the login command is enabled
        /// </summary>
        /// <returns></returns>
        private bool IsLoginEnabled() => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
        #endregion

        protected override void OnIsBusyChanged()
        {
            base.OnIsBusyChanged();

            LoginVisibility = IsBusy ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
