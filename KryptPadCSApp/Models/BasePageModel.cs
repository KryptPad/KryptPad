using KryptPad.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace KryptPadCSApp.Models
{
    class BasePageModel : BaseModel
    {

        #region Properties
               
        /// <summary>
        /// Gets or sets whether the auto login is temporarily disabled
        /// </summary>
        protected bool DisableAutoLogin
        {
            get { return (App.Current as App).DisableAutoLogin; }
            set { (App.Current as App).DisableAutoLogin = value; }
        }

        private bool _isBusy;
        /// <summary>
        /// Gets or sets the IsBusy status, indicating that the page is doing something that blocks user interaction
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                // Notify change
                OnPropertyChanged(nameof(IsBusy));
                
                // Call the OnIsBusyChanged method so derived classes can handle it
                OnIsBusyChanged();
            }
        }

        private Visibility _busyIndicatorVisibility;
        /// <summary>
        /// Gets or sets whether the ui element is visible
        /// </summary>
        public Visibility BusyIndicatorVisibility
        {
            get { return _busyIndicatorVisibility; }
            set
            {
                _busyIndicatorVisibility = value;
                // Notify change
                OnPropertyChanged(nameof(BusyIndicatorVisibility));

            }
        }

        #endregion

        #region Ctor
        public BasePageModel()
        {
            BusyIndicatorVisibility = Visibility.Collapsed;
        }
        #endregion

        #region Helper Methods
        
        #endregion

        #region Event Handlers

        /// <summary>
        /// When overriden in a derived class, handles the IsBusy property
        /// </summary>
        protected virtual void OnIsBusyChanged() {
            // Set the visibility of the busy indicator
            BusyIndicatorVisibility = IsBusy ? Visibility.Visible : Visibility.Collapsed;
            // Set main window busy state
            (Window.Current.Content as MainPage).SetIsBusy(IsBusy);
        }

        #endregion
    }
}
