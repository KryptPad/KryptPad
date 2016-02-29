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

namespace KryptPadCSApp.Models
{
    class NewItemPageViewModel : BasePageModel
    {
        #region Properties

        private bool _isLoading;

        /// <summary>
        /// Gets or sets the category for the new item
        /// </summary>
        public ApiCategory Category { get; set; }

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
                //load the item
                LoadItem(_item);
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

        /// <summary>
        /// Gets a collection of delete fields
        /// </summary>
        public FieldCollection DeletedFields { get; protected set; } = new FieldCollection();
        
        public Command AddFieldCommand { get; protected set; }

        public Command DeleteFieldCommand { get; protected set; }

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

            AddFieldCommand = new Command(async (p) =>
            {

                // Show the add field dialog
                var res = await DialogHelper.ShowDialog<AddFieldDialog>(async (d) =>
                {

                    //create new field
                    var field = new ApiField()
                    {
                        Name = (d.DataContext as AddFieldDialogViewModel).FieldName
                    };

                    try
                    {
                        // Send the field to the API to be stored under the item
                        await KryptPadApi.SaveFieldAsync(Category.Id, Item.Id, field);

                        //add to list of fields
                        Fields.Add(field);
                    }
                    catch (Exception ex)
                    {
                        // Operation failed
                        await DialogHelper.ShowMessageDialogAsync(ex.Message);
                    }


                });


            });

            // Handles field delete
            DeleteFieldCommand = new Command(async (p) =>
            {
                // Prompt user to delete the field
                var promptResp = await DialogHelper.Confirm(
                    "This action cannot be undone. Are you sure you want to delete this field?",
                    async (c) =>
                    {
                        // Get field
                        var f = p as ApiField;

                        try
                        {
                            // Call api to delete the field from the item
                            await KryptPadApi.DeleteFieldAsync(Category.Id, Item.Id, f.Id);

                            // Remove the field
                            Fields.Remove(f);
                        }
                        catch (Exception ex)
                        {
                            // Operation failed
                            await DialogHelper.ShowMessageDialogAsync(ex.Message);
                        }
                    });


            });


        }

        /// <summary>
        /// Loads an IItem into the view model
        /// </summary>
        /// <param name="item"></param>
        private async void LoadItem(ApiItem selectedItem)
        {
            _isLoading = true;

            try
            {
                // Get the item
                var itemResp = await KryptPadApi.GetItemAsync(Category.Id, selectedItem.Id);

                // Get the item
                var item = itemResp.Items.FirstOrDefault();

                // Success?
                if (item != null)
                {

                    // Set backing fields. Don't want to trigger the property's set methods
                    ItemName = item.Name;
                    Notes = item.Notes;

                    // Get the fields from the API
                    var fieldResp = await KryptPadApi.GetFieldsAsync(Category.Id, item.Id);

                    // Set fields
                    foreach (var field in fieldResp.Fields)
                    {
                        Fields.Add(field);
                    }
                }

            }
            catch (Exception ex)
            {
                // Operation failed
                await DialogHelper.ShowMessageDialogAsync(ex.Message);
            }

            _isLoading = false;
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

                // Update the item
                await KryptPadApi.SaveItemAsync(Category.Id, Item);
            }
            catch (Exception ex)
            {
                // Failed
                await DialogHelper.ShowMessageDialogAsync(ex.Message);
            }
        }

    }
}
