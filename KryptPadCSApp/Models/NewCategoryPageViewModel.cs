using KryptPadCSApp.API;
using KryptPadCSApp.API.Models;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KryptPadCSApp.Models
{
    class NewCategoryPageViewModel : BasePageModel
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
                //ensure it is not null
                if (value == null)
                {
                    value = string.Empty;
                }

                //set value
                _categoryName = value.Trim();

                //notify change
                OnPropertyChanged(nameof(CategoryName));

                //if there is some text, then we can execute
                AddCategoryCommand.CommandCanExecute = !string.IsNullOrWhiteSpace(_categoryName);
            }
        }


        /// <summary>
        /// Adds the new category top the current document
        /// </summary>
        public Command AddCategoryCommand { get; private set; }

        public ICommand CancelCommand { get; private set; }
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
            //add the category
            AddCategoryCommand = new Command(async (p) =>
            {
                //create new category
                var category = new ApiCategory()
                {
                    Name = CategoryName
                };

                // Send the category to the api
                var resp = await KryptPadApi.CreateCategoryAsync(CurrentProfile, category, AccessToken, Passphrase);
                

                //navigate
                Navigate(typeof(ItemsPage), category);
            }, false);

            //cancel command
            CancelCommand = new Command((p) => { GoBack(); });

        }
    }
}
