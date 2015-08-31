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
        public Brush Background { get; protected set; }
        #endregion
    }
}
