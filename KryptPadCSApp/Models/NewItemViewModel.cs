using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KryptPadCSApp.Models
{
    class NewItemViewModel : BaseModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the category for the new item
        /// </summary>
        public Category Category { get; set; }

        private string _itemName;
        /// <summary>
        /// Gets or sets the name of the item
        /// </summary>
        public string ItemName
        {
            get { return _itemName; }
            set
            {
                //ensure it is not null
                if (value == null)
                {
                    value = string.Empty;
                }

                _itemName = value.Trim();
                //notify change
                OnPropertyChanged(nameof(ItemName));
                //if there is some text, then we can execute
                AddItemCommand.CommandCanExecute = !string.IsNullOrWhiteSpace(_itemName);
            }
        }

        public Command AddItemCommand { get; private set; }

        public ICommand CancelCommand { get; private set; }


        #endregion

        public NewItemViewModel()
        {
            RegisterCommands();
        }

        /// <summary>
        /// Registers commands for UI elements
        /// </summary>
        private void RegisterCommands()
        {
            //add the category
            AddItemCommand = new Command((p) =>
            {
                //create new category
                var item = new Profile()
                {
                    Name = ItemName
                };

                //add the item to the current category
                Category.Items.Add(item);

                //navigate
                Navigate(typeof(ItemsPage), item);
            }, false);

            //cancel command
            CancelCommand = new Command((p) => { GoBack(); });

        }
    }
}
