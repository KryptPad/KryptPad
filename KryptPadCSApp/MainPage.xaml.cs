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
using System.Net;
using OneCode.Windows.UWP.Controls;
using System.ComponentModel;

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

        // List of ValueTuple holding the Navigation Tag and the relative Navigation Page  
        private readonly IList<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("home", typeof(ItemsPage)),
            ("favs", typeof(ItemsPage)),
            ("about", typeof(AboutPage)),
            ("feedback", typeof(FeedbackPage)),
            ("donate", typeof(DonatePage))
        };
        #endregion

        #region Properties
        /// <summary>
        /// Gets the main frame for content
        /// </summary>
        public Frame RootFrame { get { return NavigationFrame; } }

        /// <summary>
        /// Gets or sets whether the app is busy
        /// </summary>
        private bool IsBusy { get; set; }

        /// <summary>
        /// Gets whether the user is signed in
        /// </summary>
        public bool IsSignedIn
        {
            get
            {
                return (App.Current as App).SignInStatus == SignInStatus.SignedInWithProfile;
            }
        }

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

                    // Hide the message
                    ShowSessionWarningMessage(false);
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
                        ShowSessionWarningMessage(true);
                    }
                    else if (DateTime.Now < warningTime && _messageShowing)
                    {
                        // Hide the message
                        ShowSessionWarningMessage(false);
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
                if (e.PropertyName == nameof(App.SignInStatus))
                {
                    ShowSignedInControls((App.Current as App).SignInStatus == SignInStatus.SignedInWithProfile);
                }
            };

        }

        #region Navigation

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // This turns the back button on if the frame can go back
            UpdateBackButtonVisibility();

            // Set up back request handler
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            // Set the nav frame events
            NavigationFrame.Navigated += On_Navigated;

            // NavView doesn't load any page by default: you need to specify it
            NavView_Navigate(typeof(LoginPage));

        }

        private void NavView_Navigate(string navItemTag)
        {
            var item = _pages.First(p => p.Tag.Equals(navItemTag));
            NavigationFrame.Navigate(item.Page);
        }
        private void NavView_Navigate(Type page)
        {
            NavigationFrame.Navigate(page);
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {

            //if (args.IsSettingsInvoked)
            //    NavigationFrame.Navigate(typeof(SettingsPage));
            //else
            {
                // Getting the Tag from Content (args.InvokedItem is the content of NavigationViewItem)
                var navItemTag = NavView.MenuItems
                    .OfType<NavigationViewItem>()
                    .First(i => args.InvokedItem.Equals(i.Content))
                    .Tag.ToString();

                NavView_Navigate(navItemTag);
            }
        }


        private void On_Navigated(object sender, NavigationEventArgs e)
        {


            //if (NavigationFrame.SourcePageType == typeof(SettingsPage))
            //{
            //    // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag
            //    NavView.SelectedItem = (NavigationViewItem)NavView.SettingsItem;
            //}
            //else
            {
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);
                if (item.Page != null)
                {
                    NavView.SelectedItem = NavView.MenuItems
                        .OfType<NavigationViewItem>()
                        .First(n => n.Tag.Equals(item.Tag));
                }
                else
                {
                    // No page in the list
                    NavView.SelectedItem = null;
                }
            }

            // Each time a navigation event occurs, update the Back button's visibility
            UpdateBackButtonVisibility();
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            // Pass the event args to the back requested handler for handling
            //BackRequested?.Invoke(sender, e);

            // If we didn't handle the requerst, do default
            if (!e.Handled)
            {
                if (NavigationFrame.CanGoBack)
                {
                    e.Handled = true;
                    NavigationFrame.GoBack();
                }
            }
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
            ShowSessionWarningMessage(false);
        }


        #region Helper Methods

        private void ShowSessionWarningMessage(bool value)
        {
            if (value)
            {
                // Show the message
                BorderStoryBoardFadeIn.Begin();
            }
            else
            {
                // Hide the message
                BorderStoryBoardFadeOut.Begin();
            }


            _messageShowing = value;
        }

        /// <summary>
        /// Shows or hides the back button
        /// </summary>
        private void UpdateBackButtonVisibility()
        {
            // Each time a navigation event occurs, update the Back button's visibility
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                NavigationFrame.CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Shows or hides the pane
        /// </summary>
        /// <param name="value"></param>
        public void ShowSignedInControls(bool value)
        {
            // Hide nav panel
            //NavPanel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;

            // Hide buttons we can't access yet
            var visibility = value ? Visibility.Visible : Visibility.Collapsed;
            HomeNavButton.Visibility = visibility;
            FavNavButton.Visibility = visibility;
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

        private async void BroadcastMessageText_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                // Get broadcast message
                var message = await KryptPadApi.GetBroadcastMessage();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    BroadcastMessage.Visibility = Visibility.Visible;
                    BroadcastMessageText.Text = message;
                }

            }
            catch (WebException ex)
            {
                // Something went wrong in the api
                await DialogHelper.ShowMessageDialogAsync(ex.Message);
            }
            catch (Exception ex)
            {
                // Failed
                await DialogHelper.ShowGenericErrorDialogAsync(ex);
            }

        }

        private void BroadcastCloseButton_Click(object sender, RoutedEventArgs e)
        {
            BroadcastMessage.Visibility = Visibility.Collapsed;
        }

    }
}
