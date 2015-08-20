using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KryptPadCSApp.Models
{
    class NewCategoryPageViewModel : BaseModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the new category
        /// </summary>
        private string _categoryName;

        public string CategoryName
        {
            get { return _categoryName; }
            set
            {
                _categoryName = value;
                //notify change
                OnPropertyChanged(nameof(CategoryName));
            }
        }


        /// <summary>
        /// Adds the new category top the current document
        /// </summary>
        public ICommand AddCategoryCommand { get; private set; }
        #endregion

        public NewCategoryPageViewModel()
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
                //create new category
                var category = new Category()
                {
                    Name = CategoryName
                };

                //add to the list
                MainPageViewModel.Document.Categories.Add(category);

                //navigate
                Navigate(typeof(ItemsPage), category);
            });

        }
    }
}
