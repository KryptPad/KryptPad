using KryptPadCSApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
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

        private StorageFile _selectedFile;

        public StorageFile SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                _selectedFile = value;
                //notify change
                OnPropertyChanged(nameof(SelectedFile));
            }
        }


        public Command UnlockCommand { get; protected set; }

        public Command CancelCommand { get; protected set; }

        #endregion

        public AuthenticateUserControlViewModel()
        {
            //get the file we temporarily stored
            SelectedFile = (App.Current as App).SelectedFile;

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

                try
                {
                    //load the most recent file
                    (App.Current as App).Document = await Document.Load(SelectedFile, Password);

                    //close the dialog
                    DialogHelper.CloseDialog();
                }
                catch (Exception ex)
                {
                    //alert the user that something wen't wrong here
                    var msgBox = new MessageDialog(ex.Message);
                    //show
                    await msgBox.ShowAsync();
                }

            }, false);

            CancelCommand = new Command((p) =>
            {
                //close this dialog and go back to login
                DialogHelper.CloseDialog();

                //open login
                DialogHelper.LoginDialog();
            });

        }
    }
}
