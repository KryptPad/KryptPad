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
            get { return (App.Current as App).Document.Categories; }
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
            //register commands
            RegisterCommands();

            //listen for changes to document. this occurs mainly when a new document
            //is created. we have to let the model know so we can notify changes to the
            //categories property
            (App.Current as App).PropertyChanged += (sender, e) =>
            {
                //raise property changed event for categories
                OnPropertyChanged(nameof(Categories));
            };

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
