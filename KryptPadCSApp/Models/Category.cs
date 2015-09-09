using KryptPadCSApp.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace KryptPadCSApp.Models
{
    class Category : BaseModel
    {

        #region Properties

        private string _name;
        /// <summary>
        /// Gets or sets the name of the profile
        /// </summary>
        public string Name
        {
            get
            { return _name; }
            set
            {
                _name = value;
                //raise change event
                OnPropertyChanged(nameof(Name));
            }
        }

        private char _icon;
        /// <summary>
        /// Gets or sets the category symbol from font icon set
        /// </summary>
        public char Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                //raise change event
                OnPropertyChanged(nameof(Icon));
            }
        }


        /// <summary>
        /// Gets the list of items in the category
        /// </summary>
        public ItemCollection Items { get; private set; } = new ItemCollection();

        /// <summary>
        /// Handles item click event
        /// </summary>
        [JsonIgnore]
        public ICommand ItemClickCommand { get; set; }

        #endregion

        public Category()
        {
            //initialize collection with an add item as the first item
            Items.Add(ItemBase.CreateAddItem());

            RegisterCommands();
        }

        /// <summary>
        /// Registers commands for UI elements
        /// </summary>
        private void RegisterCommands()
        {
            //handle item click
            ItemClickCommand = new Command((p) =>
            {
                var item = p as ItemBase;

                if (item is AddItem)
                {
                    //navigate to add new
                    Navigate(typeof(NewItemPage), this);
                }
                else 
                {
                    //TODO: fix this...
                    item.Category = this;
                    //navigate to edit
                    Navigate(typeof(EditItemPage), item);
                }

            }, false);
        }
    }
}
