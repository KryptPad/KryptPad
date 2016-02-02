using KryptPadCSApp.API;
using KryptPadCSApp.API.Models;
using KryptPadCSApp.API.Responses;
using KryptPadCSApp.Classes;
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
                //if there is some text, then we can execute
                AddItemCommand.CommandCanExecute = CanAddItem();
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
            }
        }


        public Command AddItemCommand { get; protected set; }

        public Command AddFieldCommand { get; protected set; }

        public Command CancelCommand { get; protected set; }

        public Command DeleteFieldCommand { get; protected set; }



        #endregion

        public NewItemPageViewModel()
        {
            RegisterCommands();
            //ItemTypes = new string[] { "Profile", "Note" };
        }

        /// <summary>
        /// Registers commands for UI elements
        /// </summary>
        private void RegisterCommands()
        {
            //add the category
            AddItemCommand = new Command(SaveItem, false);

            AddFieldCommand = new Command(async (p) =>
            {

                //show the add field dialog
                var res = await DialogHelper.ShowAddFieldDialog();

                //get the result
                if (res != null)
                {
                    //create new field
                    var field = new ApiField()
                    {
                        Name = (string)res
                    };

                    //add to list of fields
                    Fields.Add(field);
                }
            });

            DeleteFieldCommand = new Command((p) =>
            {
                var f = p as ApiField;
                // Remove the field
                Fields.Remove(f);

                // Add the field to the delete list. This will be sent to the API for deletion
                // Add to deleted fields
                if (f.Id > 0)
                {
                    DeletedFields.Add(f);
                }
            });



            //cancel command
            CancelCommand = new Command((p) => { GoBack(); });

        }

        /// <summary>
        /// Loads an IItem into the view model
        /// </summary>
        /// <param name="item"></param>
        private async void LoadItem(ApiItem selectedItem)
        {


            try
            {
                // Get the item
                var itemResp = await KryptPadApi.GetItemAsync(CurrentProfile.Id, Category.Id, selectedItem.Id, AccessToken, Passphrase);

                // Get the item
                var item = itemResp.Items.FirstOrDefault();

                // Success?
                if (item != null)
                {

                    // Set properties
                    ItemName = item.Name;
                    Notes = item.Notes;
                    
                    // Get the fields from the API
                    var fieldResp = await KryptPadApi.GetFieldsAsync(CurrentProfile.Id, Category.Id, item.Id, AccessToken, Passphrase);
                    
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


        }

        /// <summary>
        /// Creates / edits item
        /// </summary>
        /// <param name="p"></param>
        private async void SaveItem(object p)
        {
            ApiItem item = Item;

            // If we do not have an item for editing, then create the instance now
            if (item == null)
            {
                item = new ApiItem();
            }

            // Set the properties of the item. If this was loaded from an existing item
            // then the properties will contain the name and category.
            item.Name = ItemName;
            item.Notes = Notes;

            try
            {
                // Create or update the item
                var resp = await KryptPadApi.SaveItemAsync(CurrentProfile.Id, Category.Id, item, AccessToken, Passphrase);

                // Set the item id
                item.Id = resp.Id;

                // Add the fields that do not exist
                foreach (var field in Fields)
                {
                    // Send the field to the API to be stored under the item
                    resp = await KryptPadApi.SaveFieldAsync(CurrentProfile.Id, Category.Id, item.Id, field, AccessToken, Passphrase);
                }

                // TODO: Delete the items in our deleted list
                foreach (var field in DeletedFields)
                {
                    // Call api to delete the field from the item
                    await KryptPadApi.DeleteFieldAsync(CurrentProfile.Id, Category.Id, item.Id, field.Id, AccessToken);
                }

                //navigate back to items and make sure category is selected
                Navigate(typeof(ItemsPage), Category);

            }
            catch (Exception ex)
            {
                // Operation failed
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }

        }

        /// <summary>
        /// Determines if the user can add an item
        /// </summary>
        /// <returns></returns>
        private bool CanAddItem() => !string.IsNullOrWhiteSpace(_itemName);



    }
}
