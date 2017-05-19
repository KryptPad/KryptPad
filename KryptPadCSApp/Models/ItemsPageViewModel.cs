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

        /// <summary>
        /// Gets the selected items
        /// </summary>
        public ObservableCollection<ApiItem> SelectedItems { get; set; } = new ObservableCollection<ApiItem>();


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

        private ListViewSelectionMode _selectionMode;

        /// <summary>
        /// Gets or sets the grid view's selection mode
        /// </summary>
        public ListViewSelectionMode SelectionMode
        {
            get { return _selectionMode; }
            set
            {
                _selectionMode = value;

                // If the selection mode is none
                CanClickItem = (value == ListViewSelectionMode.None);

                // Clear the selected items
                SelectedItems.Clear();

                // Trigger the move command execution change
                MoveItemsCommand.OnCanExecuteChanged();

                // Notify change
                OnPropertyChanged(nameof(SelectionMode));
            }
        }

        private bool _canSelectItems;

        /// <summary>
        /// Gets or sets whether the grid view's items can be selected
        /// </summary>
        public bool CanClickItem
        {
            get { return _canSelectItems; }
            set
            {
                _canSelectItems = value;
                // Notify change
                OnPropertyChanged(nameof(CanClickItem));
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

        public Command DownloadProfileCommand { get; protected set; }

        /// <summary>
        /// Opens rename dialog
        /// </summary>
        public Command RenameCategoryCommand { get; protected set; }

        /// <summary>
        /// Deletes the category
        /// </summary>
        public Command DeleteCategoryCommand { get; protected set; }

        public Command SelectModeCommand { get; protected set; }

        public Command MoveItemsCommand { get; protected set; }

        #endregion


        public ItemsPageViewModel()
        {
            // Register commands
            RegisterCommands();

            // Set properties
            EmptyMessageVisibility = Visibility.Collapsed;
            SelectionMode = ListViewSelectionMode.None;

            // Events
            SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;

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
                await DialogHelper.ShowClosableDialog<NamePromptDialog>(async (d) =>
                {
                    try
                    {
                        //create new category
                        var category = new ApiCategory()
                        {
                            Name = d.Value,
                            Items = new ApiItem[] { }
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

                        // Hide empty message
                        EmptyMessageVisibility = Visibility.Collapsed;
                    }
                    catch (WebException ex)
                    {
                        // Something went wrong in the api
                        await DialogHelper.ShowMessageDialogAsync(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        // Failed
                        await DialogHelper.ShowConnectionErrorMessageDialog();
                    }

                }, "Add Category");
            });

            // Handle add new item
            AddItemCommand = new Command(AddItemCommandHandler);

            // Handle item click
            ItemClickCommand = new Command((p) =>
            {
                var item = p as ApiItem;
                var category = (from c in Categories
                                where c.Items.Contains(item)
                                select c).FirstOrDefault();

                // Navigate to edit
                NavigationHelper.Navigate(typeof(NewItemPage),
                    new EditItemPageParams()
                    {
                        Category = category,
                        Item = item
                    });


            });

            // Handle change passphrase command
            ChangePassphraseCommand = new Command(async (p) =>
            {

                var dialog = new ChangePassphraseDialog();

                await dialog.ShowAsync();

            });

            // Handle rename command
            RenameProfileCommand = new Command(async (p) =>
            {

                // Prompt for name
                await DialogHelper.ShowNameDialog(async (d) =>
                {
                    try
                    {
                        // Set new name
                        var profile = KryptPadApi.CurrentProfile;
                        profile.Name = d.Value;

                        // Send the category to the api
                        var resp = await KryptPadApi.SaveProfileAsync(profile);

                    }
                    catch (WebException ex)
                    {
                        // Something went wrong in the api
                        await DialogHelper.ShowMessageDialogAsync(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        // Failed
                        await DialogHelper.ShowConnectionErrorMessageDialog();
                    }
                }, "RENAME PROFILE", KryptPadApi.CurrentProfile.Name);
            });

            // Handle delete command
            DeleteProfileCommand = new Command(async (p) =>
            {

                var res = await DialogHelper.Confirm(
                    "All of your data in this profile will be deleted permanently. THIS ACTION CANNOT BE UNDONE. Are you sure you want to delete this entire profile?",
                    "WARNING - CONFIRM DELETE",
                    async (ap) =>
                    {
                        try
                        {
                            // Delete the selected profile
                            await KryptPadApi.DeleteProfileAsync(KryptPadApi.CurrentProfile);

                            // Navigate back to the profiles list
                            NavigationHelper.Navigate(typeof(SelectProfilePage), null);

                            // Clear backstack
                            NavigationHelper.ClearBackStack();

                        }
                        catch (WebException ex)
                        {
                            // Something went wrong in the api
                            await DialogHelper.ShowMessageDialogAsync(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            // Failed
                            await DialogHelper.ShowConnectionErrorMessageDialog();
                        }

                    }
                );


            });

            // Download profile handler
            DownloadProfileCommand = new Command(async (prop) =>
            {
                try
                {
                    // Prompt for a place to save the file
                    var sfd = new FileSavePicker()
                    {
                        SuggestedFileName = KryptPadApi.CurrentProfile.Name,

                    };

                    //sfd.FileTypeChoices.Add("KryptPad Document Format", new List<string>(new[] { ".kdf" }));
                    sfd.FileTypeChoices.Add("KryptPad Document Format", new[] { ".kdf" });

                    // Show the picker
                    var file = await sfd.PickSaveFileAsync();

                    if (file != null)
                    {

                        // Get profile
                        var profileData = await KryptPadApi.DownloadCurrentProfileAsync();

                        // Save profile to file
                        using (var fs = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                        using (var sw = new StreamWriter(fs.AsStreamForWrite()))
                        {
                            // Write the data
                            sw.Write(profileData);
                        }

                        await DialogHelper.ShowMessageDialogAsync("Profile downloaded successfully");
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
                    await DialogHelper.ShowConnectionErrorMessageDialog();
                }

            });

            // Handle category rename
            RenameCategoryCommand = new Command(RenameCategoryCommandHandler);

            // Handle category delete
            DeleteCategoryCommand = new Command(DeleteCategoryCommandHandler);

            // Handle selection mode
            SelectModeCommand = new Command(SelectModeCommandHandler);

            // Handle the move command
            MoveItemsCommand = new Command(MoveItemsCommandHandler, CanMoveItems);
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
                await DialogHelper.ShowConnectionErrorMessageDialog();
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
                await DialogHelper.ShowConnectionErrorMessageDialog();
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
                    await DialogHelper.ShowConnectionErrorMessageDialog();
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
                    await DialogHelper.ShowConnectionErrorMessageDialog();
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
                    await DialogHelper.ShowConnectionErrorMessageDialog();
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
                            await DialogHelper.ShowConnectionErrorMessageDialog();
                        }
                    }
                }
            );


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
