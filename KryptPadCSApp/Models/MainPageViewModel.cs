using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace KryptPadCSApp.Models
{
    class MainPageViewModel : BaseModel
    {

        #region Properties
        public ObservableCollection<MenuItem> MenuItems { get; set; } = new ObservableCollection<MenuItem>();

        /// <summary>
        /// Gets or sets the selected menu item
        /// </summary>
        private MenuItem _selectedMenuItem;

        public MenuItem SelectedMenuItem
        {
            get { return _selectedMenuItem; }
            set
            {
                _selectedMenuItem = value;

                //if we have a selected item, then nav pane is not open
                IsNavPaneOpen = false;

                //notify change
                OnPropertyChanged(nameof(SelectedMenuItem));
            }
        }


        private bool _isNavPaneOpen;

        public bool IsNavPaneOpen
        {
            get { return _isNavPaneOpen; }
            set
            {
                _isNavPaneOpen = value;
                //notify change
                OnPropertyChanged(nameof(IsNavPaneOpen));
            }
        }



        #endregion

        public MainPageViewModel()
        {
            //create menu items
            MenuItems.Add(new MenuItem() { Symbol = Symbol.Home, Text = "Home" });
            MenuItems.Add(new MenuItem() { Symbol = Symbol.Tag, Text = "Manage Categories" });
        }

    }
}
