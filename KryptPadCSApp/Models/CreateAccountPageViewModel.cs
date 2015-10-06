using KryptPadCSApp.API;
using KryptPadCSApp.API.Responses;
using KryptPadCSApp.Classes;
using KryptPadCSApp.Views;
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

        private bool _isBusy;

        /// <summary>
        /// Gets or sets that the page is busy doing something
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                //notify change
                OnPropertyChanged(nameof(IsBusy));
                //set visibility
                AccountInfoVisibility = _isBusy ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private Visibility _accountInfoVisibility;

        public Visibility AccountInfoVisibility
        {
            get { return _accountInfoVisibility; }
            set
            {
                _accountInfoVisibility = value;
                //notify change
                OnPropertyChanged(nameof(AccountInfoVisibility));

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
                try
                {
                    //log in and get access token
                    var response = await KryptPadApi.CreateAccountAsync(Email, Password);

                    //if the response is ok, then go to login page
                    if (response is SuccessResponse)
                    {
                        await DialogHelper.ShowMessageDialog("Your account has been successfully created.");

                        //go to login page
                        Navigate(typeof(LoginPage));
                    }
                    else
                    {
                        //get the response
                        var error = (response as WebExceptionResponse).ModelState.Errors.FirstOrDefault();

                        await DialogHelper.ShowMessageDialog(error);
                    }
                }
                catch (Exception)
                {
                    await DialogHelper.ShowConnectionErrorMessageDialog();
                }
                

                IsBusy = false;

            });
        }



    }
}
