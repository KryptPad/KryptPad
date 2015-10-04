using KryptPadCSApp.API;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KryptPadCSApp.Models
{
    class LoginPageViewModel : BaseModel
    {

        #region Properties
        private string _email;

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

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                //notify change
                OnPropertyChanged(nameof(IsBusy));
                //set the visibility of the login panel
                LoginVisibility = _isBusy ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private Visibility _loginVisibility;

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
                //log in and get access token
                var data = await KryptPadApi.AuthenticateAsync(Email, Password);
                var t = data;

                IsBusy = false;
            });

            CreateAccountCommand = new Command((p) =>
            {
                //navigate to the create account page
                //Navigate(typeof(CreateAccountPage));
                
            });
        }


    }
}
