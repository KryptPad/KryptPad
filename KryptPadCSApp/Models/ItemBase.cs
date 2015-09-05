using KryptPadCSApp.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace KryptPadCSApp.Models
{
    class ItemBase : BaseModel, IItem
    {
        #region Properties

        private Category _category;
        /// <summary>
        /// Gets or sets the category this item is filed under
        /// </summary>
        [JsonIgnore]
        public Category Category
        {
            get { return _category; }
            set
            {
                _category = value;
                //raise change event
                OnPropertyChanged(nameof(Category));
            }
        }

        /// <summary>
        /// Gets the item type
        /// </summary>
        private ItemType _itemType;

        public ItemType ItemType
        {
            get { return _itemType; }
            set
            {
                _itemType = value;
                //raise change event
                OnPropertyChanged(nameof(ItemType));

                //configure defaults based on type
                ConfigureItemDefaults();
            }
        }




        private string _name;
        /// <summary>
        /// Gets or sets the name of the profile
        /// </summary>
        public string Name
        {
            get
            { return _name; }
            set
            {
                _name = value;
                //raise change event
                OnPropertyChanged(nameof(Name));
            }
        }

        private char _icon;
        /// <summary>
        /// Gets or sets the category symbol from font icon set
        /// </summary>
        public char Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                //raise change event
                OnPropertyChanged(nameof(Icon));
            }
        }

        /// <summary>
        /// Gets or sets the background color of the item
        /// </summary>
        [JsonIgnore]
        public Brush Background { get; protected set; }

        /// <summary>
        /// Gets a collection of fields
        /// </summary>
        public FieldCollection Fields { get; protected set; } = new FieldCollection();
        #endregion

        public ItemBase()
        {

        }

        //Returns an instance of ItemBase configured for Add Item
        public static ItemBase CreateAddItem()
        {
            return new ItemBase()
            {
                ItemType = ItemType.AddItem,
                Name = "Add Item",
                Icon = (char)0xE109
            };
        }

        private void ConfigureItemDefaults()
        {
            if (ItemType == ItemType.Note)
            {
                Background = new SolidColorBrush(Colors.Lavender);
            }
            else if (ItemType == ItemType.Profile)
            {
                Background = new SolidColorBrush(Colors.LightBlue);
            }
        }
    }
}
