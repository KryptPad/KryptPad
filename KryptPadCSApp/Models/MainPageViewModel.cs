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

        public ICommand HomeNavButtonCommand { get; private set; }

        public ICommand MenuButtonCommand { get; private set; }

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

            MenuButtonCommand = new Command((p) =>
            {
                IsPaneOpen = !IsPaneOpen;
                IsMenuButtonChecked = false;
            });
        }

        
    }
}
