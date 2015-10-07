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
        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                //notify change
                OnPropertyChanged(nameof(IsBusy));
                
                //call the OnIsBusyChanged method so derived classes can handle it
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
                //notify change
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

        protected override void Navigate(Type pageType, object parameter)
        {
            var mainPage = Window.Current.Content as MainPage;

            if (mainPage != null)
            {
                var frame = mainPage.RootFrame;
                if (frame != null)
                {
                    frame.Navigate(pageType, parameter);
                }
            }
            else
            {
                //try to get the frame from the window content itself
                var frame = Window.Current.Content as Frame;

                if (frame != null)
                {
                    frame.Navigate(pageType, parameter);
                }
            }

        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// When overriden in a derived class, handles the IsBusy property
        /// </summary>
        protected virtual void OnIsBusyChanged() {
            //set the visibility of the busy indicator
            BusyIndicatorVisibility = IsBusy ? Visibility.Visible : Visibility.Collapsed;

        }

        #endregion
    }
}
