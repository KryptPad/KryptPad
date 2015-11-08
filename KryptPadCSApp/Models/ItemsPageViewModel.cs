using KryptPadCSApp.API;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KryptPadCSApp.Models
{
    class ItemsPageViewModel : BaseModel
    {
        #region Properties
        /// <summary>
        /// Gets the collection of categories
        /// </summary>
        public CategoryCollection Categories
        {
            get
            {

                // Call api to get categories list
                return new CategoryCollection();
            }
        }

        private Category _selectedCategory;
        /// <summary>
        /// Gets or sets the selected category
        /// </summary>
        public Category SelectedCategory
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
                //navigate
                Navigate(typeof(NewCategoryPage));
            });

        }

        private async Task RefreshCategories()
        {
            var resp = await KryptPadApi.GetCategoriesAsync(1, (App.Current as App).AccessToken);
        }
    }
}
