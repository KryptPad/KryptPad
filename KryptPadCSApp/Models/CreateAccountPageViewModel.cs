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
                // Can user sign up?
                CreateAccountCommand.CommandCanExecute = CanSignUp();

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
                // Can user sign up?
                CreateAccountCommand.CommandCanExecute = CanSignUp();
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
                // Can user sign up?
                CreateAccountCommand.CommandCanExecute = CanSignUp();
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

                    // The account was created
                    await DialogHelper.ShowMessageDialogAsync("Your account has been successfully created.");

                    //go to login page
                    NavigationHelper.Navigate(typeof(LoginPage), null);

                }
                catch (Exception ex)
                {
                    await DialogHelper.ShowMessageDialogAsync(ex.Message);
                }


                IsBusy = false;

            }, false);
        }

        protected override void OnIsBusyChanged()
        {
            base.OnIsBusyChanged();

            //set visibility
            AccountInfoVisibility = IsBusy ? Visibility.Collapsed : Visibility.Visible;
        }

        protected bool CanSignUp() => !string.IsNullOrEmpty(Email) && !string.IsNullOrWhiteSpace(Password) && Password.Equals(ConfirmPassword);

    }
}
