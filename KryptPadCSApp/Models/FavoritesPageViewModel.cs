using KryptPad.Api;
using KryptPad.Api.Models;
using KryptPad.Api.Responses;
using KryptPadCSApp.Classes;
using KryptPadCSApp.Collections;
using KryptPadCSApp.Dialogs;
using KryptPadCSApp.Models.Dialogs;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace KryptPadCSApp.Models
{
    class FavoritesPageViewModel : BasePageModel
    {
        #region Properties
        /// <summary>
        /// Gets the collection of categories
        /// </summary>
        public List<ApiCategory> Items { get; protected set; } = new List<ApiItem>();

        
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

        private Visibility _emptyMessageVisibility;
        /// <summary>
        /// Gets or sets whether the empty message is visible
        /// </summary>
        public Visibility EmptyMessageVisibility
        {
            get { return _emptyMessageVisibility; }
            set
            {
                _emptyMessageVisibility = value;
                // Changed
                OnPropertyChanged(nameof(EmptyMessageVisibility));
            }
        }
                
        /// <summary>
        /// Gets or sets the command that is fired when an item is clicked
        /// </summary>
        public Command ItemClickCommand { get; set; }

        /// <summary>
        /// Gets or sets the command that is fired when user sets or unsets a favorite
        /// </summary>
        public Command SetFavoriteCommand { get; set; }

       
        #endregion


        public FavoritesPageViewModel()
        {
            // Register commands
            RegisterCommands();

            // Set properties
            EmptyMessageVisibility = Visibility.Collapsed;
            
        }

        /// <summary>
        /// Registers commands for UI elements
        /// </summary>
        private void RegisterCommands()
        {
            
            // Handle item click
            ItemClickCommand = new Command((p) =>
            {
                var item = p as ApiItem;
                var category = new ApiCategory();

                // Navigate to edit
                NavigationHelper.Navigate(typeof(NewItemPage),
                    new EditItemPageParams()
                    {
                        Category = category,
                        Item = item
                    });


            });

            // Handle setting favorites
            SetFavoriteCommand = new Command(SetFavoriteCommandHandler);
        }

        #region Methods

        /// <summary>
        /// Get the list of categories from the database
        /// </summary>
        /// <returns></returns>
        public async Task RefreshCategoriesAsync()
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

                // Show empty message if there are no categories
                EmptyMessageVisibility = Categories.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
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
                // Issue #25 was fixed. Problem was returning an anonymous object instead of an ApiCategory
                var categories = (from c in Categories
                                  select new ApiCategory
                                  {
                                      Id = c.Id,
                                      Name = c.Name,
                                      Items = (from i in c.Items
                                               where i.Name.IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) >= 0
                                               select i).ToArray()
                                  }).Where(c => c.Items.Any());

                // Add view to the ItemsView object
                ItemsView.Source = categories;

                // Refresh
                OnPropertyChanged(nameof(ItemsView));
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

        #region Command handlers

        /// <summary>
        /// Handles the move command
        /// </summary>
        /// <param name="obj"></param>
        private async void MoveItemsCommandHandler(object obj)
        {
            // Show a dialog to pick a new category
            var dialog = new ChangeCategoryDialog();

            // Show the dialog
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // Get the model
                var m = dialog.DataContext as ChangeCategoryDialogViewModel;

                try
                {
                    // Save each item
                    foreach (var item in SelectedItems)
                    {
                        // Store old category
                        var oldCategoryId = item.CategoryId;
                        // Set new category
                        item.CategoryId = m.SelectedCategory.Id;
                        // Save
                        await KryptPadApi.SaveItemAsync(oldCategoryId, item);
                    }

                    // Refresh the view
                    await RefreshCategoriesAsync();
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
        }
        /// <summary>
        /// Handles the SelectModeCommand
        /// </summary>
        /// <param name="obj"></param>
        private void SelectModeCommandHandler(object obj)
        {
            if (SelectionMode == ListViewSelectionMode.Multiple)
            {
                SelectionMode = ListViewSelectionMode.None;
            }
            else
            {
                SelectionMode = ListViewSelectionMode.Multiple;
            }
        }

        private async void AddItemCommandHandler(object p)
        {
            // Prompt to create the new item
            var dialog = new AddItemDialog();

            // Show the dialog and wait for a response
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
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

                    // If a template was selected, create a couple of fields to start with
                    if (dialog.SelectedItemTemplate != null)
                    {
                        var templateFields = dialog.SelectedItemTemplate.Fields;

                        // A template was selected, add all the fields from the template
                        foreach (var templateField in templateFields)
                        {
                            // Create field
                            var field = new ApiField()
                            {
                                Name = templateField.Name,
                                FieldType = templateField.FieldType
                            };

                            // Send to api
                            await KryptPadApi.SaveFieldAsync(category.Id, item.Id, field);
                        }
                    }


                    // Navigate to item edit page
                    NavigationHelper.Navigate(typeof(NewItemPage), new EditItemPageParams()
                    {
                        Category = category,
                        Item = item
                    });

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
        }

        private async void RenameCategoryCommandHandler(object p)
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
                    await RefreshCategoriesAsync();
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
            }, "RENAME CATEGORY", category.Name);
        }

        private async void DeleteCategoryCommandHandler(object p)
        {
            // Confirm delete
            var res = await DialogHelper.Confirm("All items under this category will be deleted. Are you sure you want to delete this category?",
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
                                // Refresh the view
                                await RefreshCategoriesAsync();
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
                }
            );


        }

        private async void SetFavoriteCommandHandler(object obj)
        {
            var item = obj as ApiItem;
            try
            {

                // Set / remove favorite
                if (!item.IsFavorite)
                {
                    await KryptPadApi.AddItemToFavoritesAsync(item);
                }
                else
                {
                    await KryptPadApi.DeleteItemFromFavoritesAsync(item);
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

        #region Event handlers
        private void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Raise the event on the command to update the CanExecute property
            MoveItemsCommand.OnCanExecuteChanged();
        }

        #endregion

        #region Helper methods

        private bool CanMoveItems(object p) => SelectedItems.Count > 0;

        #endregion
    }
}
