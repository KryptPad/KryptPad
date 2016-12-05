using KryptPad.Api;
using KryptPad.Api.Models;
using KryptPad.Api.Responses;
using KryptPadCSApp.Classes;
using KryptPadCSApp.Collections;
using KryptPadCSApp.Dialogs;
using KryptPadCSApp.Exceptions;
using KryptPadCSApp.Models.Dialogs;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace KryptPadCSApp.Models
{
    class EditItemPageViewModel : BasePageModel
    {
        #region Properties

        private bool _isLoading;

        /// <summary>
        /// Gets the list of categories
        /// </summary>
        public CollectionViewSource CategoriesView { get; protected set; } = new CollectionViewSource();

        /// <summary>
        /// Gets the available colors
        /// </summary>
        public SolidColorBrush[] AvailableColors { get; protected set; } = {
            new SolidColorBrush(Colors.SkyBlue),
            new SolidColorBrush(Colors.CornflowerBlue),
            new SolidColorBrush(Colors.Pink),
            new SolidColorBrush(Colors.LightPink),
            new SolidColorBrush(Colors.Chartreuse),
            new SolidColorBrush(Colors.Violet),
            new SolidColorBrush(Colors.Orange)
        };


        private SolidColorBrush _selectedColor;
        /// <summary>
        /// Gets or sets the selected color
        /// </summary>
        public SolidColorBrush SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                // Find the selected color from the list of available colors
                var c = (from color in AvailableColors
                         where color.Color == value.Color
                         select color).FirstOrDefault();
                // Set the color
                _selectedColor = c;
                //_selectedColor = value;
                // Notify change
                OnPropertyChanged(nameof(SelectedColor));
                // Save the item (fire and forget)
                SaveItemAsync();
            }
        }


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
                SaveItemAsync();
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

                // Store old name
                var oldName = _itemName;

                // Set new name
                _itemName = value.Trim();

                // Check to make sure it is not empty
                if (!string.IsNullOrWhiteSpace(_itemName))
                {
                    // Save the item (fire and forget)
                    SaveItemAsync();

                }
                else
                {
                    // Restore old value
                    _itemName = oldName;

                }

                //notify change
                OnPropertyChanged(nameof(ItemName));
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
                SaveItemAsync();
            }
        }

        /// <summary>
        /// Gets a collection of fields
        /// </summary>
        public FieldCollection Fields { get; protected set; } = new FieldCollection();

        public ICommand AddFieldCommand { get; protected set; }

        public Command DeleteFieldCommand { get; protected set; }

        public Command RenameFieldCommand { get; protected set; }

        public Command GeneratePasswordCommand { get; protected set; }

        public ICommand DeleteItemCommand { get; protected set; }

        public ICommand CopyFieldValueCommand { get; protected set; }

        #endregion

        public EditItemPageViewModel()
        {
            RegisterCommands();
        }

        /// <summary>
        /// Registers commands for UI elements
        /// </summary>
        private void RegisterCommands()
        {

            // Handle add new field
            AddFieldCommand = new Command(async (p) =>
            {

                // Show the add field dialog
                var res = await DialogHelper.ShowClosableDialog<AddFieldDialog>(async (d) =>
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
                        // Something went wrong in the api
                        await DialogHelper.ShowMessageDialogAsync(ex.Message);
                    }
                    catch (Exception)
                    {
                        // Failed
                        await DialogHelper.ShowConnectionErrorMessageDialog();
                    }


                });


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
                            // Something went wrong in the api
                            await DialogHelper.ShowMessageDialogAsync(ex.Message);
                        }
                        catch (Exception)
                        {
                            // Failed
                            await DialogHelper.ShowConnectionErrorMessageDialog();
                        }
                    });

            });

            // Handle copy field value
            CopyFieldValueCommand = new Command(async (p) =>
            {
                try
                {
                    var field = p as FieldModel;
                    // Check if field value is null
                    if (!string.IsNullOrWhiteSpace(field.Value))
                    {
                        // Create a data package
                        var package = new DataPackage();
                        package.SetText(field.Value);

                        // Set the value of the field to the clipboard
                        Clipboard.SetContent(package);
                    }
                }
                catch (Exception)
                {
                    // Failed
                    await DialogHelper.ShowMessageDialogAsync("Failed to copy text to clipboard.");
                }

            });

            // Delete field
            DeleteFieldCommand = new Command(DeleteFieldCommandHandler);

            // Rename field
            RenameFieldCommand = new Command(RenameFieldCommandHandler);

            // Generate password
            GeneratePasswordCommand = new Command(GeneratePasswordCommandHandler);
        }

        /// <summary>
        /// Loads an item into the view model
        /// </summary>
        /// <param name="item"></param>
        public async Task LoadItemAsync(ApiItem selectedItem, ApiCategory category)
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
                SelectedColor = item.Background;

                // Set fields
                foreach (var field in item.Fields)
                {
                    // Add field to the list
                    AddFieldToCollection(new FieldModel(field));
                }


            }
            catch (WarningException ex)
            {
                // Operation failed
                await DialogHelper.ShowMessageDialogAsync(ex.Message);
            }
            catch (WebException ex)
            {
                // Operation failed
                await DialogHelper.ShowMessageDialogAsync(ex.Message);
                // Navigate back to the items page
                NavigationHelper.Navigate(typeof(ItemsPage), null);
            }
            catch (Exception)
            {
                // Failed
                await DialogHelper.ShowConnectionErrorMessageDialog();
                // Navigate back to the items page
                NavigationHelper.Navigate(typeof(ItemsPage), null);
            }

            _isLoading = false;
            IsBusy = false;
        }

        /// <summary>
        /// Saves details about an item
        /// </summary>
        private async void SaveItemAsync()
        {
            // If we are loading, do not save the item
            if (_isLoading) return;

            try
            {
                // Set item properties
                Item.Name = ItemName;
                Item.Notes = Notes;
                Item.Background = SelectedColor;

                var oldCategoryId = Item.CategoryId;

                Item.CategoryId = Category.Id;

                // Update the item
                await KryptPadApi.SaveItemAsync(oldCategoryId, Item);
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

            // Set main window busy state
            //(Window.Current.Content as MainPage).SetIsBusy(true);

            try
            {
                // Send the field to the API to be stored under the item
                await KryptPadApi.SaveFieldAsync(Category.Id, Item.Id, field.Field);
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

            // Set main window busy state
            //(Window.Current.Content as MainPage).SetIsBusy(false);
        }

        #region Command handlers
        protected async void DeleteFieldCommandHandler(object p)
        {
            // Get data context
            var field = p as FieldModel;

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
                        // Something went wrong in the api
                        await DialogHelper.ShowMessageDialogAsync(ex.Message);
                    }
                    catch (Exception)
                    {
                        // Failed
                        await DialogHelper.ShowConnectionErrorMessageDialog();
                    }
                });
        }

        protected async void RenameFieldCommandHandler(object p)
        {
            // Get data context
            var field = p as FieldModel;

            await DialogHelper.ShowNameDialog(async (d) =>
            {

                try
                {
                    // Update name
                    field.Name = d.Value;

                    // Call api to delete the field from the item
                    await KryptPadApi.SaveFieldAsync(Category.Id, Item.Id, field.Field);

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
            });
        }

        protected async void GeneratePasswordCommandHandler(object p)
        {
            // Get data context
            var field = p as FieldModel;
            // Prompt only if there is something in the field
            var prompt = !string.IsNullOrWhiteSpace(field.Value);
            // If we don't need to prompt, then show the dialog
            var result = !prompt;

            // Prompt only if we need to
            if (prompt)
            {
                await DialogHelper.Confirm(
                    "This will replace your existing password with a new one. Are you sure?",
                    (c) => { result = true; });

            }

            // If the result is true, then show the dialog
            if (result)
            {
                // Show the add field dialog
                var d = new PasswordGeneratorDialog();

                // Show the dialog
                var action = await d.ShowAsync();

                if (action == ContentDialogResult.Primary)
                {
                    // Set the password field to the new value
                    field.Value = (d.DataContext as PasswordGeneratorDialogViewModel)?.Password;
                }
            }


        }

        #endregion

    }
}
