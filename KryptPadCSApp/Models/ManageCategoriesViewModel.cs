using KryptPadCSApp.API;
using KryptPadCSApp.API.Models;
using KryptPadCSApp.Classes;
using KryptPadCSApp.Collections;
using KryptPadCSApp.Dialogs;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace KryptPadCSApp.Models
{
    class ManageCategoriesViewModel : BasePageModel
    {
        #region Properties

        ///// <summary>
        ///// Gets the collection view
        ///// </summary>
        //public CollectionViewSource CategorySource { get; protected set; }

        /// <summary>
        /// Gets the collection of categories
        /// </summary>
        public CategoryCollection Categories { get; protected set; } = new CategoryCollection();

        /// <summary>
        /// Opens dialog to add a new category
        /// </summary>
        public Command AddCategoryCommand { get; private set; }

        /// <summary>
        /// Opens new category page
        /// </summary>
        public Command RenameCategoryCommand { get; private set; }

        /// <summary>
        /// Opens new category page
        /// </summary>
        public Command DeleteCategoryCommand { get; private set; }

        #endregion

        public ManageCategoriesViewModel()
        {
            // Register commands
            RegisterCommands();

        }

        /// <summary>
        /// Registers commands for UI elements
        /// </summary>
        private void RegisterCommands()
        {
            AddCategoryCommand = new Command(async (p) =>
            {
                // Prompt for name
                await DialogHelper.ShowDialog<NamePromptDialog>(async (d) =>
                {
                    try
                    {
                        //create new category
                        var category = new ApiCategory()
                        {
                            Name = d.Value
                        };

                        // Send the category to the api
                        var resp = await KryptPadApi.SaveCategoryAsync(category);

                        // Set the id of the newly created category
                        category.Id = resp.Id;

                        // Add the category to the collection
                        Categories.Add(category);
                    }
                    catch (WebException ex)
                    {
                        // Something went wrong in the api
                        await DialogHelper.ShowMessageDialogAsync(ex.Message);
                    }
                    catch (Exception)
                    {
                        // Failed
                        await DialogHelper.ShowConnectionErrorMessageDialog();
                    }
                }, "ADD CATEGORY");
            });

            // Handle rename category
            RenameCategoryCommand = new Command(async (p) =>
            {
                //create new category
                var category = p as ApiCategory;

                // Prompt for name
                await DialogHelper.ShowNameDialog(async (d) =>
                {
                    try
                    {
                        // Set new name
                        category.Name = d.Value;

                        // Send the category to the api
                        var resp = await KryptPadApi.SaveCategoryAsync(category);

                        // Refresh the view
                        Categories.RefreshItem(category);
                    }
                    catch (WebException ex)
                    {
                        // Something went wrong in the api
                        await DialogHelper.ShowMessageDialogAsync(ex.Message);
                    }
                    catch (Exception)
                    {
                        // Failed
                        await DialogHelper.ShowConnectionErrorMessageDialog();
                    }
                }, "RENAME CATEGORY", category.Name);
            });

            // Handle item delete
            DeleteCategoryCommand = new Command(async (p) =>
            {
                // Confirm delete
                var res = await DialogHelper.Confirm("Are you sure you want to delete this category?",
                    async (ap) =>
                    {
                        var category = p as ApiCategory;
                        // Get the selected items and delete them
                        if (category != null)
                        {
                            try
                            {
                                // Delete the item
                                var success = await KryptPadApi.DeleteCategoryAsync(category.Id);

                                // If sucessful, remove item from the list
                                if (success)
                                {
                                    Categories.Remove(category);
                                }

                            }
                            catch (WebException ex)
                            {
                                // Something went wrong in the api
                                await DialogHelper.ShowMessageDialogAsync(ex.Message);
                            }
                            catch (Exception)
                            {
                                // Failed
                                await DialogHelper.ShowConnectionErrorMessageDialog();
                            }
                        }
                    }
                );


            });


        }

        /// <summary>
        /// Get the list of categories from the database
        /// </summary>
        /// <returns></returns>
        public async Task RefreshCategoriesAsync()
        {

            // Clear the list
            Categories.Clear();

            try
            {
                // Get categories
                var resp = await KryptPadApi.GetCategoriesAsync();

                // Create list of categories
                foreach (var category in resp.Categories)
                {
                    // Add to observable
                    Categories.Add(category);
                }
            }
            catch (WebException ex)
            {
                // Something went wrong in the api
                await DialogHelper.ShowMessageDialogAsync(ex.Message);
            }
            catch (Exception)
            {
                // Failed
                await DialogHelper.ShowConnectionErrorMessageDialog();
            }


        }
    }
}
