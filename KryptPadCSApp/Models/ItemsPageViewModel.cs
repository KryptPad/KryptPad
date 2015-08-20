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
        //public CategoryCollection Categories { get; private set; } = new CategoryCollection();

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
