using KryptPadCSApp.API.Models;
using KryptPadCSApp.Classes;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace KryptPadCSApp.Models
{
    class NewItemPageViewModel : BaseModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the category for the new item
        /// </summary>
        public ApiCategory Category { get; set; }

        private ItemBase _item;
        /// <summary>
        /// Gets or sets the current item for editing
        /// </summary>
        public ItemBase Item
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

        public string[] ItemTypes { get; protected set; }

        private string _selectedItem;
        /// <summary>
        /// Gets or sets the selected item
        /// </summary>
        public string SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                //notify change
                OnPropertyChanged(nameof(SelectedItem));
                OnPropertyChanged(nameof(FieldsVisibility));
                OnPropertyChanged(nameof(NotesVisibility));
                //if there is some text, then we can execute
                AddItemCommand.CommandCanExecute = CanAddItem();
            }
        }

        /// <summary>
        /// Gets a collection of fields
        /// </summary>
        public FieldCollection Fields { get; protected set; } = new FieldCollection();

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


        /// <summary>
        /// Gets whether or not to display the fields list
        /// </summary>
        public Visibility FieldsVisibility
        {
            get
            {
                return SelectedItem == "Profile" ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets whether or not to display the fields list
        /// </summary>
        public Visibility NotesVisibility
        {
            get
            {
                return SelectedItem == "Note" ? Visibility.Visible : Visibility.Collapsed;
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
            ItemTypes = new string[] { "Profile", "Note" };
        }

        /// <summary>
        /// Registers commands for UI elements
        /// </summary>
        private void RegisterCommands()
        {
            //add the category
            AddItemCommand = new Command((p) =>
            {
                ItemBase item = Item;

                //if we do not have an item for editing, then create the instance now
                if (item == null)
                {
                    //create new item and add it to the category
                    if (SelectedItem == "Profile")
                    {
                        item = new Profile();
                    }
                    else
                    {
                        item = new Note();
                    }

                }


                //set the properties of the item. If this was loaded from an existing item
                //then the properties will contain the name and category
                item.Name = ItemName;

                //if the item is a profile, add the fields that do not exist
                if (item is Profile)
                {
                    //we need reference to profile
                    var profileItem = item as Profile;

                    //compare to the fields already in the item
                    var itemsNotInProfile = (from f in Fields
                                             where !profileItem.Fields.Contains(f)
                                             select f);

                    //add the fields that are not already in the list
                    foreach (var field in itemsNotInProfile)
                    {
                        profileItem.Fields.Add(field);
                    }
                }
                else
                {
                    //set notes property
                    (item as Note).Notes = Notes;
                }

                // Check if the item is in the category
                if (!Category.Items.Contains(item))
                {
                    //add the item to the current category
                    Category.Items.Add(item);
                }


                //navigate back to items and make sure category is selected
                Navigate(typeof(ItemsPage), Category);

            }, false);

            AddFieldCommand = new Command(async (p) =>
            {

                //show the add field dialog
                var res = await DialogHelper.ShowAddFieldDialog();

                //get the result
                if (res != null)
                {
                    //create new field
                    var field = new Field()
                    {
                        Name = (string)res
                    };

                    //add to list of fields
                    Fields.Add(field);
                }
            });

            DeleteFieldCommand = new Command((p) =>
            {

                Fields.Remove(p as Field);
            });



            //cancel command
            CancelCommand = new Command((p) => { GoBack(); });

        }

        /// <summary>
        /// Loads an IItem into the view model
        /// </summary>
        /// <param name="item"></param>
        private void LoadItem(ItemBase item)
        {
            ItemName = item.Name;

            //if this is a profile, then we have fields
            if (item is Profile)
            {
                //copy the fields to the fields property
                foreach (var field in (item as Profile).Fields)
                {
                    Fields.Add(field);
                }

                SelectedItem = "Profile";
            }
            else
            {
                Notes = (item as Note).Notes;

                SelectedItem = "Note";
            }


        }

        /// <summary>
        /// Determines if the user can add an item
        /// </summary>
        /// <returns></returns>
        private bool CanAddItem() => SelectedItem != null
            && !string.IsNullOrWhiteSpace(_itemName);
        //&& (Fields.Count == 0 || (Fields.Count > 0 && Fields.All(f => !string.IsNullOrWhiteSpace(f.Name))));



    }
}
