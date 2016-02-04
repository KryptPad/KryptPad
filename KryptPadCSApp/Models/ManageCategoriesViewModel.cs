using KryptPadCSApp.API;
using KryptPadCSApp.API.Models;
using KryptPadCSApp.Classes;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace KryptPadCSApp.Models
{
    class ManageCategoriesViewModel : BasePageModel
    {
        #region Properties
        /// <summary>
        /// Gets the collection of categories
        /// </summary>
        public CategoryCollection Categories { get; protected set; } = new CategoryCollection();

        /// <summary>
        /// Opens new category page
        /// </summary>
        public Command AddCategoryCommand { get; private set; }

        /// <summary>
        /// Opens new category page
        /// </summary>
        public Command DeleteCategoryCommand { get; private set; }

        #endregion

        public ManageCategoriesViewModel()
        {
            // Register commands
            RegisterCommands();

#if DEBUG
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) { return; }
#endif
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

            // Handle item delete
            DeleteCategoryCommand = new Command(async (p) =>
            {
                // Confirm delete
                var res = await DialogHelper.Confirm("Are you sure you want to delete this category?");

                if (res == ContentDialogResult.Primary)
                {

                    var category = p as ApiCategory;
                    // Get the selected items and delete them
                    if (category != null)
                    {
                        try
                        {
                            // Delete the item
                            var success = await KryptPadApi.DeleteCategoryAsync(CurrentProfile, category.Id, AccessToken);

                            // If sucessful, remove item from the list
                            if (success)
                            {
                                Categories.Remove(category);
                            }

                        }
                        catch (Exception ex)
                        {
                            // Operation failed
                            await DialogHelper.ShowMessageDialogAsync(ex.Message);

                        }
                    }
                }

            });


        }

        /// <summary>
        /// Get the list of categories from the database
        /// </summary>
        /// <returns></returns>
        private async Task RefreshCategories()
        {

            // Clear the list
            Categories.Clear();

            try
            {
                // Get categories
                var resp = await KryptPadApi.GetCategoriesAsync(CurrentProfile, AccessToken, Passphrase);

                // Create list of categories
                foreach (var category in resp.Categories)
                {
                    // Add to observable
                    Categories.Add(category);
                }
            }
            catch (Exception ex)
            {
                // Operation failed
                await DialogHelper.ShowMessageDialogAsync(ex.Message);
            }


        }
    }
}
