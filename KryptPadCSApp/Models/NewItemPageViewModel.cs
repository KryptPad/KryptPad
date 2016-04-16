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
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using KryptPadCSApp.Exceptions;

namespace KryptPadCSApp.Models
{
    class NewItemPageViewModel : BasePageModel
    {
        #region Properties

        private bool _isLoading;

        /// <summary>
        /// Gets the list of categories
        /// </summary>
        public CollectionViewSource CategoriesView { get; protected set; } = new CollectionViewSource();

        private ApiCategory _category;
        /// <summary>
        /// Gets or sets the category for the new item
        /// </summary>
        public ApiCategory Category
        {
            get { return _category; }
            set
            {
                _category = value;

                // Notify change
                OnPropertyChanged(nameof(Category));

                // Save the item (fire and forget)
                var t = SaveItem();
            }
        }

        private ApiItem _item;
        /// <summary>
        /// Gets or sets the current item for editing
        /// </summary>
        public ApiItem Item
        {
            get { return _item; }
            set
            {
                _item = value;
                //notify change
                OnPropertyChanged(nameof(Item));

            }
        }


        private string _itemName;
        /// <summary>
        /// Gets or sets the name of the item
        /// </summary>
        public string ItemName
        {
            get { return _itemName; }
            set
            {
                //ensure it is not null
                if (value == null)
                {
                    value = string.Empty;
                }

                _itemName = value.Trim();
                //notify change
                OnPropertyChanged(nameof(ItemName));

                // Save the item (fire and forget)
                var t = SaveItem();

            }
        }

        private string _notes;
        /// <summary>
        /// Gets or sets the text for Note items
        /// </summary>
        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                //notify change
                OnPropertyChanged(nameof(Notes));

                // Save the item (fire and forget)
                var t = SaveItem();
            }
        }

        /// <summary>
        /// Gets a collection of fields
        /// </summary>
        public FieldCollection Fields { get; protected set; } = new FieldCollection();

        public ICommand AddFieldCommand { get; protected set; }

        public UICommand DeleteFieldCommand { get; protected set; }

        public ICommand FieldMenuButtonCommand { get; protected set; }

        public ICommand DeleteItemCommand { get; protected set; }

        public ICommand CopyFieldValueCommand { get; protected set; }

        public ICommand GeneratePasswordCommand { get; protected set; }

        #endregion

        public NewItemPageViewModel()
        {
            RegisterCommands();
        }

        /// <summary>
        /// Registers commands for UI elements
        /// </summary>
        private void RegisterCommands()
        {
            GeneratePasswordCommand = new Command(async (p) =>
            {
                



            });

            // Handle add new field
            AddFieldCommand = new Command(async (p) =>
            {

                // Show the add field dialog
                var res = await DialogHelper.ShowDialog<AddFieldDialog>(async (d) =>
                {

                    try
                    {
                        var m = (d.DataContext as AddFieldDialogViewModel);
                        var field = new FieldModel(new ApiField()
                        {
                            Name = m.FieldName,
                            FieldType = m.SelectedFieldType.Id
                        });

                        // Send the field to the API to be stored under the item
                        var resp = await KryptPadApi.SaveFieldAsync(Category.Id, Item.Id, field.Field);

                        field.Id = resp.Id;

                        // Add field to the list
                        AddFieldToCollection(field);
                    }
                    catch (WebException ex)
                    {
                        // Operation failed
                        await DialogHelper.ShowMessageDialogAsync(ex.Message);
                    }


                });


            });

            // Handle field menu click
            FieldMenuButtonCommand = new Command(async p =>
            {
                // Get data context
                var field = (p as FrameworkElement)?.DataContext as FieldModel;
                // Create popup menu
                PopupMenu menu = new PopupMenu();

                menu.Commands.Add(new UICommand("Delete", async (dp) =>
                {
                    // Prompt user to delete the field
                    var promptResp = await DialogHelper.Confirm(
                        "This action cannot be undone. Are you sure you want to delete this field?",
                        async (c) =>
                        {

                            try
                            {
                                // Call api to delete the field from the item
                                await KryptPadApi.DeleteFieldAsync(Category.Id, Item.Id, field.Id);

                                // Remove the field
                                Fields.Remove(field);
                            }
                            catch (WebException ex)
                            {
                                // Operation failed
                                await DialogHelper.ShowMessageDialogAsync(ex.Message);
                            }
                        });

                }));

                menu.Commands.Add(new UICommand("Generate Password", async (dp) => {

                    // Show the add field dialog
                    var d = new PasswordGeneratorDialog();

                    // Show the dialog
                    var result = await d.ShowAsync();

                    // Set the password field to the new value
                    field.Value = (d.DataContext as PasswordGeneratorDialogViewModel)?.Password;
                }));


                var chosenCommand = await menu.ShowForSelectionAsync(GetElementRect((FrameworkElement)p));
                if (chosenCommand == null)
                {

                }

            });

            // Handle item delete
            DeleteItemCommand = new Command(async (p) =>
            {

                // Prompt user to delete the item
                var promptResp = await DialogHelper.Confirm(
                    "This action cannot be undone. All data associated with this item will be deleted. Are you sure you want to delete this item?",
                    async (c) =>
                    {
                        try
                        {
                            // Delete the item
                            await KryptPadApi.DeleteItemAsync(Category.Id, Item.Id);

                            // Navigate back to items page
                            NavigationHelper.Navigate(typeof(ItemsPage), null);

                        }
                        catch (WebException ex)
                        {
                            // Operation failed
                            await DialogHelper.ShowMessageDialogAsync(ex.Message);

                        }
                    });

            });

            // Handle copy field value
            CopyFieldValueCommand = new Command((p) =>
            {

                var field = p as FieldModel;

                // Create a data package
                var package = new DataPackage();
                package.SetText(field.Value);

                // Set the value of the field to the clipboard
                Clipboard.SetContent(package);
            });
        }

        /// <summary>
        /// Loads an item into the view model
        /// </summary>
        /// <param name="item"></param>
        public async Task LoadItem(ApiItem selectedItem, ApiCategory category)
        {

            // Prevent change triggers
            _isLoading = true;
            IsBusy = true;

            try
            {
                // Get list of categories for the combobox control
                var categories = await KryptPadApi.GetCategoriesAsync();

                // Set the category view source for the combobox
                CategoriesView.Source = categories.Categories;

                // Update view
                OnPropertyChanged(nameof(CategoriesView));

                // Set the selected category in the list
                Category = (from c in categories.Categories
                            where c.Id == category.Id
                            select c).SingleOrDefault();

                // Check to make sure our parameters are set
                if (Category == null)
                {
                    // Show error
                    throw new WarningException("The item you are trying to edit does not exist in this category.");
                }

                // Get the item
                var itemResp = await KryptPadApi.GetItemAsync(Category.Id, selectedItem.Id);

                // Get the item
                var item = itemResp.Items.FirstOrDefault();

                // Set item
                Item = item;

                // Set properties
                ItemName = item.Name;
                Notes = item.Notes;

                // Set fields
                foreach (var field in item.Fields)
                {
                    // Add field to the list
                    AddFieldToCollection(new FieldModel(field));
                }


            }
            catch (WebException ex)
            {
                // Operation failed
                await DialogHelper.ShowMessageDialogAsync(ex.Message);
            }
            catch (WarningException ex)
            {
                // Operation failed
                await DialogHelper.ShowMessageDialogAsync(ex.Message);
            }

            _isLoading = false;
            IsBusy = false;
        }

        /// <summary>
        /// Saves details about an item
        /// </summary>
        private async Task SaveItem()
        {
            // If we are loading, do not save the item
            if (_isLoading) return;


            try
            {
                // Set item properties
                Item.Name = ItemName;
                Item.Notes = Notes;

                var oldCategoryId = Item.CategoryId;

                Item.CategoryId = Category.Id;

                // Update the item
                await KryptPadApi.SaveItemAsync(oldCategoryId, Item);
            }
            catch (WebException ex)
            {
                // Failed
                await DialogHelper.ShowMessageDialogAsync(ex.Message);
            }
        }

        /// <summary>
        /// Adds a field to the collection and listens to changes to properties
        /// </summary>
        /// <param name="field"></param>
        private void AddFieldToCollection(FieldModel field)
        {
            // Listen to changes to the Value property
            field.PropertyChanged += (sender, e) =>
            {
                // If it is the value property, call update on our field
                if (e.PropertyName == nameof(field.Value))
                {
                    UpdateField((FieldModel)sender);
                }
            };

            // Add field model to the list
            Fields.Add(field);
        }

        /// <summary>
        /// Updates the field
        /// </summary>
        /// <param name="field"></param>
        private async void UpdateField(FieldModel field)
        {

            if (_isLoading) return;

            try
            {
                // Send the field to the API to be stored under the item
                await KryptPadApi.SaveFieldAsync(Category.Id, Item.Id, field.Field);
            }
            catch (WebException ex)
            {
                // Operation failed
                await DialogHelper.ShowMessageDialogAsync(ex.Message);
            }
        }

        #region Helpers
        /// <summary>
        /// Gets the rect from an element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }
        #endregion
    }
}
