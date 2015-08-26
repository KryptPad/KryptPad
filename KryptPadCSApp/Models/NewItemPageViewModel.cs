using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KryptPadCSApp.Models
{
    class NewItemPageViewModel : BaseModel
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
                AddItemCommand.CommandCanExecute = CanAddItem();
            }
        }

        public string[] ItemTypes { get; protected set; }

        private string _selectedItem;
        /// <summary>
        /// Gets or sets the selected item
        /// </summary>
        public string SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                //notify change
                OnPropertyChanged(nameof(SelectedItem));
                //if there is some text, then we can execute
                AddItemCommand.CommandCanExecute = CanAddItem();
            }
        }


        public Command AddItemCommand { get; protected set; }

        public ICommand CancelCommand { get; protected set; }


        #endregion

        public NewItemPageViewModel()
        {
            RegisterCommands();
            ItemTypes = new string[] { "Profile", "Note" };
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

                //navigate back to items and make sure category is selected
                Navigate(typeof(ItemsPage), Category);
               
            }, false);

            //cancel command
            CancelCommand = new Command((p) => { GoBack(); });

        }

        /// <summary>
        /// Determines if the user can add an item
        /// </summary>
        /// <returns></returns>
        private bool CanAddItem() => SelectedItem !=null && !string.IsNullOrWhiteSpace(_itemName);
        
            

    }
}
