using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace KryptPadCSApp.Models
{
    class Category : BaseModel, ICategory
    {

        #region Properties

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
        /// Gets the list of items in the category
        /// </summary>
        public ObservableCollection<IItem> Items { get; private set; } = new ObservableCollection<IItem>();

        #endregion

        public Category()
        {
            //initialize collection with an add item as the first item
            Items.Add(new AddItem());
        }
    }
}
