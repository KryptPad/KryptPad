using KryptPadCSApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KryptPadCSApp.Models
{
    class AuthenticateUserControlViewModel : BaseModel
    {

        #region Properties
        private string _password;
        /// <summary>
        /// Gets or sets the document password
        /// </summary>
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                //notify change
                OnPropertyChanged(nameof(Password));
                //update can change
                UnlockCommand.CommandCanExecute = !string.IsNullOrWhiteSpace(_password);
            }
        }

        public Command UnlockCommand { get; protected set; }

        public Command CancelCommand { get; protected set; }

        #endregion

        public AuthenticateUserControlViewModel()
        {
            //register commands
            RegisterCommands();
        }

        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            UnlockCommand = new Command(async (p) =>
            {
                //get the file we temporarily stored
                var selectedFile = (App.Current as App).SelectedFile;
                //set the password on the document
                var document = await Document.Load(selectedFile, Password);
                //set the document
                (App.Current as App).Document = document;

                //close the dialog
                DialogHelper.CloseDialog(p as FrameworkElement);

            }, false);

            CancelCommand = new Command((p) => {
                //close this dialog and go back to login
                DialogHelper.CloseDialog(p as FrameworkElement);

                //open login
                DialogHelper.LoginDialog();
            });

        }
    }
}
