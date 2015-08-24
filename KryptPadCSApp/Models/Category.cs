﻿using KryptPadCSApp.Views;
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
    class Category : BaseModel, ICategory
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

        private SymbolIcon _symbol;
        /// <summary>
        /// Gets or sets the category symbol from font icon set
        /// </summary>
        public SymbolIcon Symbol
        {
            get { return _symbol; }
            set
            {
                _symbol = value;
                //raise change event
                OnPropertyChanged(nameof(Symbol));
            }
        }


        /// <summary>
        /// Gets the list of items in the category
        /// </summary>
        public ObservableCollection<IItem> Items { get; private set; } = new ObservableCollection<IItem>();

        /// <summary>
        /// Handles item click event
        /// </summary>
        public ICommand ItemClickCommand { get; set; }

        #endregion

        public Category()
        {
            //initialize collection with an add item as the first item
            Items.Add(new AddItem());

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
                if (p is AddItem)
                {
                    //navigate
                    Navigate(typeof(NewItemPage), this);
                }
            }, false);
        }
    }
}
