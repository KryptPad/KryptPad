using KryptPadCSApp.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KryptPadCSApp.Models
{
    class CreateAccountPageViewModel : BasePageModel
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

        private string _confirmPassword;

        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                _confirmPassword = value;
                //notify change
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }

        public Command CreateAccountCommand { get; protected set; }

        #endregion

        public CreateAccountPageViewModel()
        {
            RegisterCommands();
        }

        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            

            CreateAccountCommand = new Command(async (p) =>
            {
                IsBusy = true;
                //log in and get access token
                var data = await KryptPadApi.CreateAccountAsync(Email, Password);
                var t = data;

                IsBusy = false;

            });
        }



    }
}
