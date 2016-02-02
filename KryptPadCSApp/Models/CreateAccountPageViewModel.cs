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
                        await DialogHelper.ShowMessageDialogAsync("Your account has been successfully created.");

                        //go to login page
                        Navigate(typeof(LoginPage));
                    }
                    else
                    {
                        //get the response
                        var error = (response as WebExceptionResponse).ModelState;//.Errors.FirstOrDefault();

                        await DialogHelper.ShowMessageDialogAsync(error.ToString());
                    }
                }
                catch (Exception ex)
                {
                    await DialogHelper.ShowConnectionErrorMessageDialog();
                }
                

                IsBusy = false;

            });
        }

        protected override void OnIsBusyChanged()
        {
            base.OnIsBusyChanged();

            //set visibility
            AccountInfoVisibility = IsBusy ? Visibility.Collapsed : Visibility.Visible;
        }

    }
}
