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
using CodeCreators.UWP.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace KryptPadCSApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        #region Fields
        private bool _messageShowing = false;
        #endregion

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

            // Some API events
            KryptPadApi.SessionEnded += async () =>
            {
                // Get the dispatcher
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    // Triggered when the access token has reached its expiration date
                    NavigationHelper.Navigate(typeof(LoginPage), null);
                    // Clear backstack too
                    NavigationHelper.ClearBackStack();

                    // Hide the warning message
                    SessionEndWarning.Visibility = Visibility.Collapsed;

                    _messageShowing = false;
                });

            };

            KryptPadApi.SessionEnding += async (expiration) =>
            {
                var warningTime = expiration.AddMinutes(-1);

                // Show the message
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    
                    if (DateTime.Now >= warningTime && !_messageShowing)
                    {
                        // Show the warning
                        //SessionEndWarning.Visibility = Visibility.Visible;
                        BorderStoryBoardFadeIn.Begin();
                        
                        _messageShowing = true;
                    }
                    else if (DateTime.Now < warningTime && _messageShowing)
                    {
                        // Hide the warning
                        //SessionEndWarning.Visibility = Visibility.Collapsed;
                        BorderStoryBoardFadeOut.Begin();
                        BorderStoryBoardFadeIn.Stop();

                        _messageShowing = false;
                    }

                    // Show time remaining
                    if (DateTime.Now >= warningTime)
                    {
                        // Get time remaining
                        var timeRemaining = DateTime.Now.Subtract(expiration);
                        // Set the label with how much time the user has left
                        TimeRemainingRun.Text = timeRemaining.ToString(@"mm\:ss");
                    }
                });
            };

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

        #region Navigation
        
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
       
        private void HomeRadioButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate
            NavigationHelper.Navigate(typeof(ItemsPage), null);

        }

        private void DonateRadioButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate
            NavigationHelper.Navigate(typeof(DonatePage), null);

        }

        private void FeedbackRadioButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate
            NavigationHelper.Navigate(typeof(FeedbackPage), null);

        }

        private void AboutRadioButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate
            NavigationHelper.Navigate(typeof(AboutPage), null);

        }

        private async void SignOutRadioButton_Click(object sender, RoutedEventArgs e)
        {
            await DialogHelper.Confirm("Are you sure you want to sign out?", "SIGN OUT", (p) =>
            {
                // Navigate
                NavigationHelper.Navigate(typeof(LoginPage), null);
                // Clear backstack
                NavigationHelper.ClearBackStack();
            });

        }

        #endregion

        private void SessionEndWarning_Tapped(object sender, TappedRoutedEventArgs e)
        {
            KryptPadApi.ExtendSessionTime();
            // Hide the message
            BorderStoryBoardFadeOut.Begin();
            BorderStoryBoardFadeIn.Stop();
            
            _messageShowing = false;
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
