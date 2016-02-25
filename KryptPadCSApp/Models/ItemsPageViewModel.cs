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
        public ApiCategory[] Categories { get; protected set; } = { };
        //public CategoryCollection Categories { get; protected set; } = new CategoryCollection();

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

        //private ApiCategory _selectedCategory;
        ///// <summary>
        ///// Gets or sets the selected category
        ///// </summary>
        //public ApiCategory SelectedCategory
        //{
        //    get { return _selectedCategory; }
        //    set
        //    {
        //        _selectedCategory = value;

        //        // Notify change
        //        OnPropertyChanged(nameof(SelectedCategory));

        //    }
        //}


        private Visibility _bottomAppBarVisible;
        /// <summary>
        /// Gets or sets the bottom app bar's visibility
        /// </summary>
        public Visibility BottomAppBarVisible
        {
            get { return _bottomAppBarVisible; }
            set
            {
                _bottomAppBarVisible = value;

                // Notify change
                OnPropertyChanged(nameof(BottomAppBarVisible));
            }
        }

        private ListViewSelectionMode _selectionMode;
        /// <summary>
        /// Gets or sets the grid selection mode
        /// </summary>
        public ListViewSelectionMode SelectionMode
        {
            get { return _selectionMode; }
            set
            {
                _selectionMode = value;

                // Notify change
                OnPropertyChanged(nameof(SelectionMode));

                // Set item click enabled if selection mode is none
                IsItemClickEnabled = value == ListViewSelectionMode.None;

            }
        }

        private bool _isItemClickEnabled;
        /// <summary>
        /// Gets or sets whether the grid has item click enabled
        /// </summary>
        public bool IsItemClickEnabled
        {
            get { return _isItemClickEnabled; }
            set
            {
                _isItemClickEnabled = value;

                // Notify change
                OnPropertyChanged(nameof(IsItemClickEnabled));

                // Hide the bottom app bar when the items are clickable
                BottomAppBarVisible = value ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private ApiItem _selectedItem;
        /// <summary>
        /// Gets or sets the selected item
        /// </summary>
        public ApiItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;

                // Enable the delete command
                DeleteItemCommand.CommandCanExecute = value != null;
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
        /// Deletes the selected item(s)
        /// </summary>
        public Command DeleteItemCommand { get; private set; }

        /// <summary>
        /// Gets or sets the command that is fired when an item is clicked
        /// </summary>
        public Command ItemClickCommand { get; set; }

        /// <summary>
        /// Gets or sets the command that is fired when toggle selection is clicked
        /// </summary>
        public Command ToggleSelectionMode { get; set; }

        #endregion


        public ItemsPageViewModel()
        {
            // Register commands
            RegisterCommands();

            // Set some defaults
            SelectionMode = ListViewSelectionMode.None;

#if DEBUG
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) { return; }
#endif

            

            // Get list of categories
            var task = RefreshCategories();

            // When task completes, turn off indicator
            //task.ContinueWith(async (t) =>
            //{
            //    // This is weird
            //    await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
            //        () =>
            //        {
            //            // Turn off indicator
            //            //IsBusy = false;
            //        });
            //});
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
                        var resp = await KryptPadApi.SaveCategoryAsync(CurrentProfile, category, AccessToken, Passphrase);

                        // Set the id of the newly created category
                        category.Id = resp.Id;

                        // Add the category to the collection
                        //Categories.Add(category);

                        // Select the category
                        //SelectedCategory = category;
                    }
                    catch (Exception ex)
                    {
                        // Operation failed
                        await DialogHelper.ShowMessageDialogAsync(ex.Message);
                    }
                }, "Add Category");
            });

            AddItemCommand = new Command((p) =>
            {
                var category = p as ApiCategory;

                // Navigate
                Navigate(typeof(NewItemPage), category);
            });

            //handle item click
            ItemClickCommand = new Command((p) =>
            {
                var item = p as ApiItem;
                var category = (from c in Categories
                                where c.Items.Contains(item)
                                select c).FirstOrDefault();

                // Navigate to edit
                Navigate(typeof(NewItemPage), new EditItemPageParams() { Category = category, Item = item });


            }, false);

            // Handle item delete
            DeleteItemCommand = new Command(async (p) =>
            {

                // Get the selected items and delete them
                if (SelectedItem != null)
                {
                    try
                    {
                        //// Delete the item
                        //var success = await KryptPadApi.DeleteItemAsync(CurrentProfile.Id, SelectedCategory.Id, SelectedItem.Id, AccessToken);

                        //// If sucessful, remove item from the list
                        //if (success)
                        //{
                        //    await RefreshCategories();
                        //}

                    }
                    catch (Exception ex)
                    {
                        // Operation failed
                        await DialogHelper.ShowMessageDialogAsync(ex.Message);

                    }
                }

            }, false);

            // Handle toggle selection mode
            ToggleSelectionMode = new Command((p) =>
            {

                // Toggle the grid's selection mode
                SelectionMode = (SelectionMode == ListViewSelectionMode.Single ?
                    ListViewSelectionMode.None : ListViewSelectionMode.Single);

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
                var resp = await KryptPadApi.GetAllItemsAsync(CurrentProfile, SearchText, AccessToken, Passphrase);

                Categories = resp.Categories;

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
