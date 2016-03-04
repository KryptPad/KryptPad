using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KryptPadCSApp.Models
{
    public class BaseModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Methods

        /// <summary>
        /// Navigates the frame to a page type
        /// </summary>
        /// <param name="pageType"></param>
        protected void Navigate(Type pageType)
        {
            Navigate(pageType, null);

        }

        protected virtual void Navigate(Type pageType, object parameter)
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

        }

        
        #endregion

        #region Event Handlers

        /// <summary>
        /// Raises the PropertyChanged event for a property
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

    }
}
