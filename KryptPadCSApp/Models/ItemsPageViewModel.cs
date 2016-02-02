using KryptPadCSApp.API;
using KryptPadCSApp.API.Models;
using KryptPadCSApp.API.Responses;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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


        private Visibility _bottomAppBarVisible;
        /// <summary>
        /// Gets or sets the bottom app bar's visibility
        /// </summary>
        public Visibility BottomAppBarVisible
        {
            get { return _bottomAppBarVisible; }
            set
            {
                _bottomAppBarVisible = value;

                // Notify change
                OnPropertyChanged(nameof(BottomAppBarVisible));
            }
        }

        private ListViewSelectionMode _selectionMode;
        /// <summary>
        /// Gets or sets the grid selection mode
        /// </summary>
        public ListViewSelectionMode SelectionMode
        {
            get { return _selectionMode; }
            set
            {
                _selectionMode = value;

                // Notify change
                OnPropertyChanged(nameof(SelectionMode));

                // Set item click enabled if selection mode is none
                IsItemClickEnabled = value == ListViewSelectionMode.None;
                
            }
        }

        private bool _isItemClickEnabled;
        /// <summary>
        /// Gets or sets whether the grid has item click enabled
        /// </summary>
        public bool IsItemClickEnabled
        {
            get { return _isItemClickEnabled; }
            set
            {
                _isItemClickEnabled = value;

                // Notify change
                OnPropertyChanged(nameof(IsItemClickEnabled));

                // Hide the bottom app bar when the items are clickable
                BottomAppBarVisible = value ? Visibility.Collapsed : Visibility.Visible;
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
        /// Deletes the selected item(s)
        /// </summary>
        public Command DeleteItemCommand { get; private set; }

        /// <summary>
        /// Gets or sets the command that is fired when an item is clicked
        /// </summary>
        public Command ItemClickCommand { get; set; }

        /// <summary>
        /// Gets or sets the command that is fired when toggle selection is clicked
        /// </summary>
        public Command ToggleSelectionMode { get; set; }
        #endregion


        public ItemsPageViewModel()
        {
            // Register commands
            RegisterCommands();

            // Set some defaults
            SelectionMode = ListViewSelectionMode.None;

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

            AddItemCommand = new Command((p) =>
            {
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

            ToggleSelectionMode = new Command((p) =>
            {

                // Toggle the grid's selection mode
                SelectionMode = (SelectionMode == ListViewSelectionMode.Multiple ?
                    ListViewSelectionMode.None : ListViewSelectionMode.Multiple);

            });
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
