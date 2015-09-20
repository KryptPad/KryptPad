using KryptPadCSApp.Classes;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace KryptPadCSApp.Models
{
    class MainPageViewModel : BaseModel
    {

        #region Properties

        public Command HomeNavButtonCommand { get; private set; }

        public Command LogoutNavButtonCommand { get; private set; }

        public Command MenuButtonCommand { get; private set; }

        private bool _isPaneOpen;

        public bool IsPaneOpen
        {
            get { return _isPaneOpen; }
            set
            {
                _isPaneOpen = value;
                //notify change
                OnPropertyChanged(nameof(IsPaneOpen));
            }
        }

        private bool _isMenuButtonChecked;
        /// <summary>
        /// Gets or sets whether the menu button is checked
        /// </summary>
        public bool IsMenuButtonChecked
        {
            get { return _isMenuButtonChecked; }
            set
            {
                _isMenuButtonChecked = value;
                //notify change
                OnPropertyChanged(nameof(IsMenuButtonChecked));
            }
        }

        #endregion

        public MainPageViewModel()
        {
            RegisterCommands();
        }

        /// <summary>
        /// Registers commands for UI elements
        /// </summary>
        private void RegisterCommands()
        {
            HomeNavButtonCommand = new Command((p) =>
            {
                IsPaneOpen = false;
                //navigate
                Navigate(typeof(ItemsPage));
            });

            LogoutNavButtonCommand = new Command((p) =>
            {
                IsPaneOpen = false;
                
                //navigate
                DialogHelper.LoginDialog();
            });

            MenuButtonCommand = new Command((p) =>
            {
                IsPaneOpen = !IsPaneOpen;
                IsMenuButtonChecked = false;
            });
        }

        
    }
}
