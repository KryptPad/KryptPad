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
                CreateAccountCommand.OnCanExecuteChanged();

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
                CreateAccountCommand.OnCanExecuteChanged();
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
                CreateAccountCommand.OnCanExecuteChanged();
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
                    // Log in and get access token
                    var response = await KryptPadApi.CreateAccountAsync(Email, Password, ConfirmPassword);

                    // The account was created
                    await DialogHelper.ShowMessageDialogAsync("Your account has been successfully created.");

                    // Go to login page
                    NavigationHelper.Navigate(typeof(LoginPage), null);

                    // Clear backstack
                    NavigationHelper.ClearBackStack();

                }
                catch (WebException ex)
                {
                    // Something went wrong in the api
                    await DialogHelper.ShowMessageDialogAsync(ex.Message);
                }
                catch (Exception)
                {
                    // Failed
                    await DialogHelper.ShowGenericErrorDialogAsync();
                }


                IsBusy = false;

            }, CanSignUp);
        }

        protected override void OnIsBusyChanged()
        {
            base.OnIsBusyChanged();

            //set visibility
            AccountInfoVisibility = IsBusy ? Visibility.Collapsed : Visibility.Visible;
        }

        protected bool CanSignUp(object p) => 
            !string.IsNullOrEmpty(Email) 
            && !string.IsNullOrWhiteSpace(Password)
            && ConfirmPassword == Password;

    }
}
