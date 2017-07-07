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

        #region Common navigation tasks
        public static void GoToLicenseTerms()
        {
            // Go to terms page
            Navigate(typeof(TermsPage), null);
        }

        public static void GoToPrivacyPolicy()
        {
            // Go to terms page
            Navigate(typeof(PrivacyPage), null);
        }

        public static void GoToDonate()
        {
            // Go to terms page
            Navigate(typeof(DonatePage), null);
        }

        public static void GoToFeedback()
        {
            // Go to terms page
            Navigate(typeof(FeedbackPage), null);
        }

        public static void GoToAbout()
        {
            // Go to terms page
            Navigate(typeof(AboutPage), null);
        }
        #endregion


    }
}
