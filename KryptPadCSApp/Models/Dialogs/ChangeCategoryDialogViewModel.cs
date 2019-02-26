using KryptPad.Api;
using KryptPad.Api.Models;
using KryptPadCSApp.Classes;
using KryptPadCSApp.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.Models.Dialogs
{
    class ChangeCategoryDialogViewModel : BaseModel
    {

        #region Properties

        /// <summary>
        /// Gets the list of categories
        /// </summary>
        public CategoryCollection Categories { get; set; } = new CategoryCollection();

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
                // Notify change
                OnPropertyChanged(nameof(SelectedCategory));
                // Update the command
                PrimaryCommand.OnCanExecuteChanged();
            }
        }

        public Command PrimaryCommand { get; set; }

        #endregion

        #region Constructors
        public ChangeCategoryDialogViewModel()
        {
            // Register commands
            RegisterCommands();
        }
        #endregion

        #region Methods

        private void RegisterCommands()
        {
            PrimaryCommand = new Command(null, (p) => SelectedCategory != null);
        }

        /// <summary>
        /// Loads the categories
        /// </summary>
        /// <returns></returns>
        public async Task LoadCategoriesAsync()
        {

            try
            {
                // Get the categories from the api
                var result = await KryptPadApi.GetCategoriesAsync();

                // Add the categories
                foreach (var category in result.Categories)
                {
                    Categories.Add(category);
                }

            }
            catch (WebException ex)
            {
                // Something went wrong in the api
                await DialogHelper.ShowMessageDialogAsync(ex.Message);
            }
            catch (Exception ex)
            {
                // Failed
                await DialogHelper.ShowGenericErrorDialogAsync(ex);
            }
        }
        #endregion
    }
}
