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
            get { return MainPageViewModel.Document.Categories; }
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
        public ICommand AddCategoryCommand { get; private set; }

        

        #endregion


        public ItemsPageViewModel()
        {
            RegisterCommands();
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

    }
}
