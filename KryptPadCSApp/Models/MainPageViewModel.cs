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

        private bool? _isPaneOpen;
        /// <summary>
        /// Gets or sets the open state of the splitview
        /// </summary>
        public bool? IsPaneOpen
        {
            get { return _isPaneOpen; }
            set
            {
                _isPaneOpen = value;
                //notify change
                OnPropertyChanged(nameof(IsPaneOpen));
            }
        }

        private SplitViewDisplayMode _displayMode;
        /// <summary>
        /// Gets or sets the pane's display mode
        /// </summary>
        public SplitViewDisplayMode DisplayMode
        {
            private get { return _displayMode; }
            set { _displayMode = value; }
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
                ClosePane();

                // Navigate
                Navigate(typeof(ItemsPage));
            });

            LogoutNavButtonCommand = new Command((p) =>
            {
                ClosePane();
                
            });

            MenuButtonCommand = new Command((p) =>
            {
                if (IsPaneOpen == null)
                {
                    IsPaneOpen = false;
                }

                IsPaneOpen = !IsPaneOpen;
                IsMenuButtonChecked = false;
            });
        }

        private void ClosePane()
        {
            
            if (DisplayMode == SplitViewDisplayMode.CompactOverlay)
            {
                // Only force closed when in CompactOverlay mode
                IsPaneOpen = false;
            }
        }
        
    }
}
