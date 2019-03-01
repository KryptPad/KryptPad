using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace KryptPadCSApp.Classes
{
    class NavigationHelper
    {

        public enum NavigationType
        {
            Root,
            Main,
            Current
        };

        /// <summary>
        /// Navigates to the desired page using the specified navigation type
        /// </summary>
        /// <param name="pageType"></param>
        /// <param name="parameter"></param>
        /// <param name="navigationType"></param>
        public static void Navigate(Type pageType, object parameter, NavigationType navigationType = NavigationType.Current)
        {
            if (navigationType == NavigationType.Root)
            {
                NavigateRoot(pageType, parameter);
            }
            else if (navigationType == NavigationType.Main)
            {
                NavigateMain(pageType, parameter);
            }
            else
            {
                NavigateCurrentFrame(pageType, parameter);
            }
        }

        /// <summary>
        /// Gets the current frame
        /// </summary>
        /// <returns></returns>
        private static Frame GetFrame()
        {
            var rootFrame = (Window.Current.Content as Frame);
            if (rootFrame != null)
            {
                // Check if the root frame's content is the MainPage
                if (rootFrame.Content is MainPage)
                {
                    rootFrame = (rootFrame.Content as MainPage).RootFrame;

                }
            }

            return rootFrame;
        }

        private static void NavigateRoot(Type pageType, object parameter)
        {
            var rootFrame = (Window.Current.Content as Frame);
            // Navigate
            NavigateToPage(rootFrame, pageType, parameter);
        }

        /// <summary>
        /// Navigates to page using the current frame. If the current frame is the root, then
        /// use that. If the MainPage frame is the root, then use that.
        /// </summary>
        /// <param name="pageType"></param>
        /// <param name="parameter"></param>
        private static void NavigateCurrentFrame(Type pageType, object parameter)
        {
            var rootFrame = GetFrame();
            // Navigate
            NavigateToPage(rootFrame, pageType, parameter);
        }

        /// <summary>
        /// Navigates to a page using the frame
        /// </summary>
        /// <param name="pageType"></param>
        /// <param name="parameter"></param>
        private static void NavigateMain(Type pageType, object parameter)
        {
            var rootFrame = (Window.Current.Content as Frame);
            if (rootFrame != null)
            {
                // Check if the root frame's content is the MainPage. If not,
                // we need to navigate to it.
                if (!(rootFrame.Content is MainPage))
                {
                    NavigatedEventHandler handler = null;
                    handler = (sender, e) =>
                    {
                        // Remove handler
                        rootFrame.Navigated -= handler;
                        // Get root frame from main page
                        var mainFrame = (e.Content as MainPage).RootFrame;
                        // Navigate to our target frame
                        NavigateToPage(mainFrame, pageType, parameter);
                    };

                    // Set navigated handler. This will navigate to our target after we
                    // have navigated to the main page
                    rootFrame.Navigated += handler;
                    // Load main page
                    rootFrame.Navigate(typeof(MainPage));

                }
                else
                {
                    // The content of the root frame is already the main page, so we
                    // don't need to navigate to it, just use the root frame inside
                    // the main page.
                    var mainFrame = (rootFrame.Content as MainPage).RootFrame;
                    // Navigate to our target frame
                    NavigateToPage(mainFrame, pageType, parameter);
                }
            }

        }

        private static void NavigateToPage(Frame frame, Type pageType, object parameter)
        {
            // Navigate if we have a frame object and if the new page type is not
            // the same as the old page. This prevents the back stack from getting
            // weird.
            if (frame != null && frame.CurrentSourcePageType != pageType)
            {
                // Navigate
                frame.Navigate(pageType, parameter);

            }
        }

        public static void GoBack(BackRequestedEventArgs e)
        {
            var frame = GetFrame();
            if (frame.CanGoBack)
            {
                e.Handled = true;
                frame.GoBack();
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
