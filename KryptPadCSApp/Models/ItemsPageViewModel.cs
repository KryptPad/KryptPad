using KryptPadCSApp.API;
using KryptPadCSApp.API.Models;
using KryptPadCSApp.API.Responses;
using KryptPadCSApp.Classes;
using KryptPadCSApp.Collections;
using KryptPadCSApp.Dialogs;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace KryptPadCSApp.Models
{
    class ItemsPageViewModel : BasePageModel
    {
        #region Properties
        /// <summary>
        /// Gets the collection of categories
        /// </summary>
        public List<ApiCategory> Categories { get; protected set; } = new List<ApiCategory>();

        /// <summary>
        /// Gets the filtered view of items
        /// </summary>
        public CollectionViewSource ItemsView { get; protected set; } = new CollectionViewSource()
        {
            IsSourceGrouped = true,
            ItemsPath = new PropertyPath("Items")
        };


        private string _searchText;
        /// <summary>
        /// Gets or sets the search text
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                // Notify change
                OnPropertyChanged(nameof(SearchText));
                // Trigger search
                var t = SearchForItems();
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
        /// Gets or sets the command that is fired when an item is clicked
        /// </summary>
        public Command ItemClickCommand { get; set; }

        public Command ChangePassphraseCommand { get; protected set; }

        public Command RenameProfileCommand { get; protected set; }

        public Command DeleteProfileCommand { get; protected set; }

        #endregion


        public ItemsPageViewModel()
        {
            // Register commands
            RegisterCommands();

#if DEBUG
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) { return; }
#endif

            // Get list of categories
            var task = RefreshCategories();

        }

        /// <summary>
        /// Registers commands for UI elements
        /// </summary>
        private void RegisterCommands()
        {
            // Handle add category
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

                        // Add the category to the list
                        Categories.Add(category);

                        // Add view to the ItemsView object
                        ItemsView.Source = Categories;

                        // Refresh
                        OnPropertyChanged(nameof(ItemsView));
                    }
                    catch (Exception ex)
                    {
                        // Operation failed
                        await DialogHelper.ShowMessageDialogAsync(ex.Message);
                    }
                }, "Add Category");
            });

            // Handle add new item
            AddItemCommand = new Command(async (p) =>
            {

                // Prompt to create the new item
                var dialog = new AddItemDialog();

                // Show the dialog and wait for a response
                var dialogResp = await dialog.ShowAsync();

                if (dialogResp == ContentDialogResult.Primary)
                {

                    try
                    {
                        // Get the category
                        var category = p as ApiCategory;

                        // Create an item
                        var item = new ApiItem()
                        {
                            Name = dialog.ItemName
                        };

                        // Save the item to the api
                        var r = await KryptPadApi.SaveItemAsync(category.Id, item);

                        // Set the item
                        item.Id = r.Id;

                        // Navigate to item edit page
                        Navigate(typeof(NewItemPage), new EditItemPageParams()
                        {
                            Category = category,
                            Item = item
                        });

                    }
                    catch (Exception ex)
                    {
                        // Failed
                        await DialogHelper.ShowMessageDialogAsync(ex.Message);
                    }

                }


            });

            // Handle item click
            ItemClickCommand = new Command((p) =>
            {
                var item = p as ApiItem;
                var category = (from c in Categories
                                where c.Items.Contains(item)
                                select c).FirstOrDefault();

                // Navigate to edit
                Navigate(typeof(NewItemPage), new EditItemPageParams() { Category = category, Item = item });


            }, false);

            // Handle change passphrase command
            ChangePassphraseCommand = new Command(async (p) => {
                IsBusy = true;
                try
                {
                    // Change the passphrase
                    await KryptPadApi.ChangePassphraseAsync("87654321");
                }
                catch (Exception ex)
                {
                    // Operation failed
                    await DialogHelper.ShowMessageDialogAsync(ex.Message);
                }

                IsBusy = false;

            });

            // Handle rename command
            RenameProfileCommand = new Command(async (p) =>
            {

                // Prompt for name
                await DialogHelper.ShowDialog<NamePromptDialog>(async (d) =>
                {
                    try
                    {
                        // Set new name
                        var profile = KryptPadApi.CurrentProfile;
                        profile.Name = d.Value;

                        // Send the category to the api
                        var resp = await KryptPadApi.SaveProfileAsync(profile);
                        
                    }
                    catch (Exception ex)
                    {
                        // Operation failed
                        await DialogHelper.ShowMessageDialogAsync(ex.Message);
                    }
                }, "RENAME PROFILE");
            });

            // Handle delete command
            DeleteProfileCommand = new Command(async (p) =>
            {

                var res = await DialogHelper.Confirm(
                    "All of your data in this profile will be deleted permanently. THIS ACTION CANNOT BE UNDONE. Are you sure you want to delete this entire profile?",
                    "WARNING - COMFIRM DELETE",
                    async (ap) =>
                    {
                        // Delete the selected profile
                        await KryptPadApi.DeleteProfileAsync(KryptPadApi.CurrentProfile);

                        // Navigate back to the profiles list
                        NavigationHelper.Navigate(typeof(SelectProfilePage), null, NavigationHelper.NavigationType.Window);
                    }
                );


            });

        }

        /// <summary>
        /// Get the list of categories from the database
        /// </summary>
        /// <returns></returns>
        private async Task RefreshCategories()
        {
            // Set busy
            IsBusy = true;

            try
            {
                // Get the items if not already got
                var resp = await KryptPadApi.GetCategoriesWithItemsAsync();

                // Set the list to our list of categories
                Categories = resp.Categories.ToList();

                // Add view to the ItemsView object
                ItemsView.Source = Categories;

                // Refresh
                OnPropertyChanged(nameof(ItemsView));
            }
            catch (Exception ex)
            {
                // Operation failed
                await DialogHelper.ShowMessageDialogAsync(ex.Message);
            }

            // Not busy any more
            IsBusy = false;

        }

        /// <summary>
        /// Searches for items using a search string
        /// </summary>
        /// <param name="searchText"></param>
        public async Task SearchForItems()
        {

            try
            {

                // Filter out the items that don't match the search text. Also, filter out empty
                // categories. Only categories with items will show up
                var categories = (from c in Categories
                                  select new
                                  {
                                      Name = c.Name,
                                      Items = (from i in c.Items
                                               where i.Name.IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) >= 0
                                               select i)
                                  }).Where(c => c.Items.Any());

                // Add view to the ItemsView object
                ItemsView.Source = categories;

                // Refresh
                OnPropertyChanged(nameof(ItemsView));
            }
            catch (Exception ex)
            {
                // Operation failed
                await DialogHelper.ShowMessageDialogAsync(ex.Message);
            }

        }

    }
}
