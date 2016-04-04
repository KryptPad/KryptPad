using KryptPadCSApp.API;
using KryptPadCSApp.API.Models;
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

        public ApiFieldType[] FieldTypes { get; protected set; }

        public Command AddCommand { get; protected set; }

        public Command CancelCommand { get; protected set; }

        #endregion

        public AddFieldDialogViewModel()
        {
            RegisterCommands();
            CreateFieldTypes();
        }

        /// <summary>
        /// Creates field types
        /// </summary>
        private void CreateFieldTypes()
        {
            FieldTypes = new[] {
                new ApiFieldType() { Id = (int)FieldType.Password, Name = "Password" },
                new ApiFieldType() { Id = (int)FieldType.Username, Name = "Username" },
                new ApiFieldType() { Id = (int)FieldType.Email, Name = "Email" },
                new ApiFieldType() { Id = (int)FieldType.AccountNumber, Name = "Account Number" },
                new ApiFieldType() { Id = (int)FieldType.CreditCardNumber, Name = "Credit Card Number" },
                new ApiFieldType() { Id = (int)FieldType.Numeric, Name = "Numeric" },
                new ApiFieldType() { Id = (int)FieldType.Text, Name = "Text" }
            };
        }


        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            AddCommand = new Command((p) =>
            {


            }, false);


        }

    }
}
