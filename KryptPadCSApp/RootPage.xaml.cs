using KryptPad.Api;
using KryptPadCSApp.Classes;
using KryptPadCSApp.Views;
using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace KryptPadCSApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RootPage : Page
    {
        #region Fields
        private bool _messageShowing = false;
        #endregion

        #region Properties
        public Frame RootFrame { get { return NavigationFrame; } }
        
        #endregion

        public RootPage()
        {
            this.InitializeComponent();
            NavigationFrame.Navigated += OnNavigated;

            // Some API events
            KryptPadApi.SessionEnded += async () =>
            {
                // Get the dispatcher
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    // Triggered when the access token has reached its expiration date
                    NavigationHelper.Navigate(typeof(LoginPage), null, NavigationHelper.NavigationType.Root);

                    // Hide the message
                    ShowSessionWarningMessage(false);
                });

            };

            KryptPadApi.SessionEnding += async (expiration) =>
            {
                var warningTime = expiration.AddMinutes(-KryptPadApi.SESSION_WARNING_MINUTES);

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
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationHelper.Navigate(typeof(LoginPage), null);

        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            // Each time a navigation event occurs, update the Back button's visibility
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = NavigationFrame.CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }

        private void SessionEndWarning_Tapped(object sender, TappedRoutedEventArgs e)
        {
            KryptPadApi.ExtendSessionTime();
            // Hide the message
            ShowSessionWarningMessage(false);
        }

        /// <summary>
        /// Shows or hides the session warning message
        /// </summary>
        /// <param name="value"></param>
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
        /// Sets the busy state
        /// </summary>
        /// <param name="value"></param>
        public void SetIsBusy(bool value)
        {
            BusyBorder.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            BusyIndicator.IsActive = value;
        }
    }
}
