using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.Models
{
    class NamePromptDialogViewModel : BaseModel
    {
        #region Properties
        private string _value;
        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                // Notify change
                OnPropertyChanged(nameof(Value));
                // Check if valid
                PrimaryCommand.CommandCanExecute = IsValid;
            }
        }

        public Command PrimaryCommand { get; protected set; }
        #endregion

        #region Ctor
        public NamePromptDialogViewModel() {
            RegisterCommands();
        }
        #endregion

        #region Methods
        private void RegisterCommands() {
            PrimaryCommand = new Command(null, false);
        }

        private bool IsValid => !string.IsNullOrWhiteSpace(Value);
        #endregion
    }
}
