﻿using KryptPadCSApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KryptPadCSApp.Models
{
    class CreatePasswordUserControlViewModel : BaseModel
    {

        #region Properties

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                //notify change
                OnPropertyChanged(nameof(Password));
                //update can execute
                DoneCommand.CommandCanExecute = CanSavePassword();
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
                //update can execute
                DoneCommand.CommandCanExecute = CanSavePassword();
            }
        }



        /// <summary>
        /// Gets the command to handle unlocking
        /// </summary>
        public Command DoneCommand { get; protected set; }
        #endregion

        public CreatePasswordUserControlViewModel()
        {

            //register commands
            RegisterCommands();
        }

        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            DoneCommand = new Command((p) =>
            {
                //set the password on the document
                MainPageViewModel.Document.SessionPassword = Password;

                //close the dialog
                DialogHelper.CloseDialog(p as FrameworkElement);

            }, false);

        }

        /// <summary>
        /// Determines if the user can save the password. Not very strict.
        /// </summary>
        /// <returns></returns>
        public bool CanSavePassword() => Password.Length >= 8 &&
            Password.Equals(ConfirmPassword);
    }
}
