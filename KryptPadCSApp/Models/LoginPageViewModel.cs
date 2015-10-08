using KryptPadCSApp.API;
using KryptPadCSApp.API.Responses;
using KryptPadCSApp.Classes;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                //notify change
                OnPropertyChanged(nameof(Email));

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
                //notify change
                OnPropertyChanged(nameof(Password));
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
                //notify change
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
        }

        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            LogInCommand = new Command(async (p) =>
            {
                IsBusy = true;

                try
                {
                    //log in and get access token
                    var response = await KryptPadApi.AuthenticateAsync(Email, Password);

                    //check the response. if it is an OAuthTokenResponse then store the token and navigate user
                    //to select profile page
                    if (response is OAuthTokenResponse)
                    {
                        //store the access token
                        (App.Current as App).AccessToken = (response as OAuthTokenResponse).AccessToken;

                        //navigate to the select profile page
                        Navigate(typeof(SelectProfilePage));

                    }
                    else
                    {
                        await DialogHelper.ShowMessageDialog(
                            "Your username or password is incorrect.");
                    }
                }
                catch (Exception)
                {
                    await DialogHelper.ShowConnectionErrorMessageDialog();
                }
                

                IsBusy = false;
            });

            CreateAccountCommand = new Command((p) =>
            {
                //navigate to the create account page
                Navigate(typeof(CreateAccountPage));
            });
        }

        protected override void OnIsBusyChanged()
        {
            base.OnIsBusyChanged();

            LoginVisibility = IsBusy ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
