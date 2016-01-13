using KryptPadCSApp.Models;
using KryptPadCSApp.UserControls;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Security.Authentication.OnlineId;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace KryptPadCSApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Frame RootFrame { get; private set; }

        public MainPage(Frame frame)
        {
            this.InitializeComponent();
            
            RootFrame = frame;
            ShellSplitView.Content = RootFrame;
            
        }

        private void MenuRadioButton_Click(object sender, RoutedEventArgs e)
        {

            // This button should not be checked
            MenuRadioButton.IsChecked = false;
            // Command the split view to be closed or opened
            ShellSplitView.IsPaneOpen = !ShellSplitView.IsPaneOpen;
            
        }

        private void HomeRadioButton_Click(object sender, RoutedEventArgs e)
        {

            //navigate
            Navigate(typeof(ItemsPage), null);

            // Close the pane
            ClosePane();
        }

        #region Helper Methods

        /// <summary>
        /// Closes the pane if in CompactOverlay mode
        /// </summary>
        private void ClosePane()
        {
            // Check to see if the pane is in overlay mode, if it is, then we close it,
            // otherwise, we do not close it
            if (ShellSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
            {
                // Only force closed when in CompactOverlay mode
                ShellSplitView.IsPaneOpen = false;
            }
        }

        // TODO: Move this to a static class somewhere
        /// <summary>
        /// Navigates to a page using the MainPage frame
        /// </summary>
        /// <param name="pageType"></param>
        /// <param name="parameter"></param>
        private void Navigate(Type pageType, object parameter)
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

    }
}
