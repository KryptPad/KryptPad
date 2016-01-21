﻿using KryptPadCSApp.API;
using KryptPadCSApp.API.Models;
using KryptPadCSApp.API.Responses;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KryptPadCSApp.Models
{
    class ItemsPageViewModel : BasePageModel
    {
        #region Properties
        /// <summary>
        /// Gets the collection of categories
        /// </summary>
        public CategoryCollection Categories { get; protected set; } = new CategoryCollection();

        private ApiCategory _selectedCategory;
        /// <summary>
        /// Gets or sets the selected category
        /// </summary>
        public ApiCategory SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                //notify change
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }


        /// <summary>
        /// Opens new category page
        /// </summary>
        public Command AddCategoryCommand { get; private set; }

        /// <summary>
        /// Opens the add item page
        /// </summary>
        public Command AddItemCommand { get; private set; }

        /// <summary>
        /// Gets or sets the command that is fired when an item is clicked
        /// </summary>
        public Command ItemClickCommand { get; set; }
        #endregion


        public ItemsPageViewModel()
        {
            // Register commands
            RegisterCommands();

            // Get list of categories
            var t = RefreshCategories();
        }

        /// <summary>
        /// Registers commands for UI elements
        /// </summary>
        private void RegisterCommands()
        {
            AddCategoryCommand = new Command((p) =>
            {
                // Navigate
                Navigate(typeof(NewCategoryPage));
            });

            AddItemCommand = new Command((p) => {
                // Navigate
                Navigate(typeof(NewItemPage), SelectedCategory);
            });

            //handle item click
            ItemClickCommand = new Command((p) =>
            {
                var item = p as ApiItem;


                // Navigate to edit
                Navigate(typeof(NewItemPage), new EditItemPageParams() { Category = SelectedCategory, Item = item });


            }, false);
        }

        private async Task RefreshCategories()
        {
            var resp = await KryptPadApi.GetCategoriesAsync(CurrentProfile.Id, AccessToken, Passphrase);

            // Check to see if the response is success
            if (resp is CategoryResponse)
            {
                // Get categories
                var categories = (resp as CategoryResponse).Categories;
                // Create list of categories
                foreach (var category in categories)
                {
                    // Add to observable
                    Categories.Add(category);
                }
            }
        }
    }
}
