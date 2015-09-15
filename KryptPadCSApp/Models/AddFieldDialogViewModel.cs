using KryptPadCSApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.Models
{
    class AddFieldDialogViewModel : BaseModel
    {

        #region Properties

        private string _fieldName;
        /// <summary>
        /// Gets or sets the name of the field to add
        /// </summary>
        public string FieldName
        {
            get { return _fieldName; }
            set
            {
                _fieldName = value;
                //notify change
                OnPropertyChanged(nameof(FieldName));
                //change can execure
                AddCommand.CommandCanExecute = !string.IsNullOrWhiteSpace(_fieldName);
            }
        }

        public Command AddCommand { get; protected set; }

        public Command CancelCommand { get; protected set; }

        #endregion

        public AddFieldDialogViewModel()
        {
            RegisterCommands();
        }


        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            AddCommand = new Command((p) =>
            {
                //close the dialog
                DialogHelper.CloseDialog(FieldName);

            }, false);

            CancelCommand = new Command((p) =>
            {
                //close this dialog and go back to login
                DialogHelper.CloseDialog();
                
            });

        }

    }
}
