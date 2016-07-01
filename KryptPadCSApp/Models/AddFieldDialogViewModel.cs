using KryptPad.Api;
using KryptPad.Api.Models;
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
                // Notify change
                OnPropertyChanged(nameof(FieldName));
                // Change can execure
                PrimaryCommand.CommandCanExecute = AddFieldCommandCanExecute;
            }
        }

        public ApiFieldType[] FieldTypes { get; protected set; }

        private ApiFieldType _selectedFieldType;
        public ApiFieldType SelectedFieldType
        {
            get { return _selectedFieldType; }
            set
            {
                _selectedFieldType = value;

                if (_selectedFieldType != null)
                {
                    // Update the field name to reflect the type. This is the default
                    // but the user can change it
                    FieldName = _selectedFieldType.Name;
                }
                
                // Notify change
                OnPropertyChanged(nameof(SelectedFieldType));
                // Change can execure
                PrimaryCommand.CommandCanExecute = AddFieldCommandCanExecute;
            }
        }

        public Command PrimaryCommand { get; protected set; }

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
                new ApiFieldType() { Id = FieldType.Password, Name = "Password" },
                new ApiFieldType() { Id = FieldType.Username, Name = "Username" },
                new ApiFieldType() { Id = FieldType.Email, Name = "Email" },
                new ApiFieldType() { Id = FieldType.AccountNumber, Name = "Account Number" },
                new ApiFieldType() { Id = FieldType.CreditCardNumber, Name = "Credit Card Number" },
                new ApiFieldType() { Id = FieldType.Numeric, Name = "Numeric" },
                new ApiFieldType() { Id = FieldType.Text, Name = "Text" }
            };
        }


        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            PrimaryCommand = new Command();


        }

        private bool AddFieldCommandCanExecute => !string.IsNullOrWhiteSpace(FieldName) && SelectedFieldType != null;

    }
}
