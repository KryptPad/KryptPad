using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace KryptPadCSApp.Classes
{
    class NavigationHelper
    {
        
        /// <summary>
        /// Gets the current frame
        /// </summary>
        /// <returns></returns>
        private static Frame GetFrame()
        {
            return ((Window.Current.Content as Frame).Content as MainPage).RootFrame;
        }


        /// <summary>
        /// Navigates to a page using the frame
        /// </summary>
        /// <param name="pageType"></param>
        /// <param name="parameter"></param>
        public static void Navigate(Type pageType, object parameter)
        {
            var frame = GetFrame();

            // Navigate if we have a frame object and if the new page type is not
            // the same as the old page. This prevents the back stack from getting
            // weird.
            if (frame != null && frame.CurrentSourcePageType != pageType)
            {
                // Navigate
                frame.Navigate(pageType, parameter);

            }

        }

        /// <summary>
        /// Clear the backstack
        /// </summary>
        public static void ClearBackStack(Frame frame = null)
        {
            // Use the supplied frame, or get it if we didn't get it from parameter
            frame = frame ?? GetFrame();

            if (frame != null)
            {
                // Clear
                frame.BackStack.Clear();
                // Hide the back button
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }
        
    }
}
