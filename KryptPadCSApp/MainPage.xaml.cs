using KryptPadCSApp.Models;
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
using KryptPadCSApp.Classes;
using KryptPad.Api;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace KryptPadCSApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Properties
        /// <summary>
        /// Gets the main frame for content
        /// </summary>
        public Frame RootFrame { get { return NavFrame; } }

        private bool IsBusy { get; set; }
        #endregion


        public MainPage()
        {
            this.InitializeComponent();

            //RootFrame = frame;
            //ShellSplitView.Content = RootFrame;

            //KryptPadApi.AccessTokenExpirationTimer += async (expiration) =>
            //{
            //    // Get time remaining
            //    var timeRemaining = DateTime.Now.Subtract(expiration);

            //    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //    {
            //        CountDownTextBlock.Text = timeRemaining.ToString(@"mm\:ss");
            //    });
            //};

            // Success, tell the app we are signed in
            (App.Current as App).PropertyChanged += (sender, e) =>
            {
                // Success, tell the app we are signed in
                if (e.PropertyName == nameof(App.IsSignedIn))
                {
                    ShowPane((App.Current as App).IsSignedIn);
                }
            };

            RootFrame.Navigated += RootFrame_Navigated;

        }

        private void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // Get the MainPage instance and hide the pane
            var page = Window.Current.Content as MainPage;
            if (page != null)
            {
                // Check the page type, and hide or show the pane
                if (e.Content is ItemsPage)
                {
                    HomeRadioButton.IsChecked = true;
                }
                else if (e.Content is DonatePage)
                {
                    DonateRadioButton.IsChecked = true;
                }
                else if (e.Content is FeedbackPage)
                {
                    FeedbackRadioButton.IsChecked = true;
                }
                else if (e.Content is AboutPage)
                {
                    AboutRadioButton.IsChecked = true;
                }

            }
        }

        //private void MenuRadioButton_Click(object sender, RoutedEventArgs e)
        //{

        //    // This button should not be checked
        //    //MenuRadioButton.IsChecked = false;
        //    // Command the split view to be closed or opened
        //    ShellSplitView.IsPaneOpen = !ShellSplitView.IsPaneOpen;

        //}

        private void HomeRadioButton_Click(object sender, RoutedEventArgs e)
        {
            //if (IsBusy) return;

            // Navigate
            NavigationHelper.Navigate(typeof(ItemsPage), null);

        }

        //private void CategoriesRadioButton_Click(object sender, RoutedEventArgs e)
        //{
        //    //if (IsBusy) return;

        //    // Navigate
        //    NavigationHelper.Navigate(typeof(ManageCategoriesPage), null);

        //}

        private void DonateRadioButton_Click(object sender, RoutedEventArgs e)
        {
            //if (IsBusy) return;

            // Navigate
            NavigationHelper.Navigate(typeof(DonatePage), null);

        }

        private void FeedbackRadioButton_Click(object sender, RoutedEventArgs e)
        {
            //if (IsBusy) return;

            // Navigate
            NavigationHelper.Navigate(typeof(FeedbackPage), null);

        }

        private void AboutRadioButton_Click(object sender, RoutedEventArgs e)
        {
            //if (IsBusy) return;

            // Navigate
            NavigationHelper.Navigate(typeof(AboutPage), null);

        }

        private async void SignOutRadioButton_Click(object sender, RoutedEventArgs e)
        {
            //if (IsBusy) return;

            await DialogHelper.Confirm("Are you sure you want to sign out?", "SIGN OUT", (p) =>
            {
                // Navigate
                NavigationHelper.Navigate(typeof(LoginPage), null);
                // Clear backstack
                NavigationHelper.ClearBackStack();
            });

        }

        #region Helper Methods

        ///// <summary>
        ///// Closes the pane if in CompactOverlay mode
        ///// </summary>
        //private void ClosePane()
        //{
        //    // Check to see if the pane is in overlay mode, if it is, then we close it,
        //    // otherwise, we do not close it
        //    if (ShellSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
        //    {
        //        // Only force closed when in CompactOverlay mode
        //        ShellSplitView.IsPaneOpen = false;
        //    }
        //}


        #endregion

        #region Public methods

        /// <summary>
        /// Shows or hides the pane
        /// </summary>
        /// <param name="value"></param>
        public void ShowPane(bool value)
        {
            // Hide nav panel
            NavPanel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;

            //// Hide buttons we can't access yet
            //var visibility = value ? Visibility.Visible : Visibility.Collapsed;
            //SignOutRadioButton.Visibility = visibility;
            //HomeRadioButton.Visibility = visibility;
        }

        /// <summary>
        /// Sets the busy state
        /// </summary>
        /// <param name="value"></param>
        public void SetIsBusy(bool value)
        {
            IsBusy = value;
            BusyBorder.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            BusyIndicator.IsActive = value;
        }
        #endregion
    }
}
