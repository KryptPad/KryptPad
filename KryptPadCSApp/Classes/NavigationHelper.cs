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
            Navigate(pageType, parameter, false);
        }

        /// <summary>
        /// Navigates to a page using the frame
        /// </summary>
        /// <param name="pageType"></param>
        /// <param name="parameter"></param>
        public static void Navigate(Type pageType, object parameter, bool forceRoot)
        {
            var frame = GetFrame();

            if (frame != null)
            {
                if (forceRoot)
                {
                    // Set the frame on the window's main content
                    Window.Current.Content = frame;

                    // Navigate
                    frame.Navigate(pageType, parameter);

                    // Clear backstack
                    ClearBackStack(frame);
                }
                else
                {
                    // Navigate
                    frame.Navigate(pageType, parameter);
                }


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
