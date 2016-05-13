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

        public enum NavigationType
        {
            Window,
            Frame
        }

        /// <summary>
        /// Gets the current frame
        /// </summary>
        /// <returns></returns>
        private static Frame GetFrame()
        {
            // Get the frame from the main page
            var mainPage = Window.Current.Content as MainPage;
            Frame frame = null;

            if (mainPage != null)
            {
                // Get frame from main page
                frame = mainPage.RootFrame;
            }
            else
            {
                // Try to get the frame from the window content itself
                frame = Window.Current.Content as Frame;
            }

            return frame;
        }

        /// <summary>
        /// Navigates to a page using the frame
        /// </summary>
        /// <param name="pageType"></param>
        /// <param name="parameter"></param>
        public static void Navigate(Type pageType, object parameter)
        {
            Navigate(pageType, parameter, NavigationType.Frame);
        }

        /// <summary>
        /// Navigates to a page using the frame
        /// </summary>
        /// <param name="pageType"></param>
        /// <param name="parameter"></param>
        public static void Navigate(Type pageType, object parameter, NavigationType navigationType)
        {
            var frame = GetFrame();

            if (frame != null)
            {
                if (navigationType == NavigationType.Window)
                {
                    // Set the frame on the window's main content
                    Window.Current.Content = frame;    
                }
                else
                {
                    // Move the frame back into the main page
                    if (!(Window.Current.Content is MainPage))
                    {
                        Window.Current.Content = new MainPage(frame);
                    }
                    
                }

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
