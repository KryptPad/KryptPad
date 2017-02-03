using KryptPad.Api;
using KryptPadCSApp.Views;
using System;
using System.ComponentModel;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace KryptPadCSApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application, INotifyPropertyChanged
    {
        private Frame _rootFrame;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<BackRequestedEventArgs> BackRequested;

        #region Properties

        /// <summary>
        /// Gets or sets whether the auto login is temporarily disabled
        /// </summary>
        internal bool DisableAutoLogin { get; set; }

        private bool _isSignedIn;
        /// <summary>
        /// Gets or sets the time remaining on the session
        /// </summary>
        internal bool IsSignedIn
        {
            get { return _isSignedIn; }
            set
            {
                // Set value
                _isSignedIn = value;
                // Value changed
                OnPropertyChanged(nameof(IsSignedIn));
            }

        }

#if DEBUG
        private bool _isLiveMode;
#endif
        /// <summary>
        /// Gets whether the app is in Live mode
        /// </summary>
        internal bool IsLiveMode
        {
            get
            {
#if DEBUG
                return _isLiveMode;
#else
                return true;
#endif
            }
#if DEBUG
            set
            {
                _isLiveMode = value;
                
                // Change the host url
                if (_isLiveMode)
                {
                    KryptPadApi.ServiceHost = "https://www.kryptpad.com/";
                } else
                {
                    KryptPadApi.ServiceHost = "http://test.kryptpad.com/";
                }
                
                // Notify change
                OnPropertyChanged(nameof(IsLiveMode));

            }
#endif
        }

        #endregion


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.Resuming += OnResuming;
        }



        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (Window.Current.Content == null)
            {
                // Create main page instance
                var mainPage = new MainPage();


                //Get the Frame to act as the navigation context and navigate to the first page
                _rootFrame = mainPage.RootFrame;

                // Add some event handlers
                _rootFrame.NavigationFailed += OnNavigationFailed;
                _rootFrame.Navigated += OnNavigated;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = mainPage;


                // Register a handler for BackRequested events and set the
                // visibility of the Back button
                SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    _rootFrame.CanGoBack ?
                    AppViewBackButtonVisibility.Visible :
                    AppViewBackButtonVisibility.Collapsed;
                
            }

            if (_rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                _rootFrame.Navigate(typeof(LoginPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();

            // Check for presence of the status bar
            //if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            //{
            //    // Get status bar
            //    //#FF173A55
            //    var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            //    statusBar.BackgroundColor = Windows.UI.ColorHelper.FromArgb(255, 17, 58, 85);
            //    statusBar.BackgroundOpacity = 1;
            //    statusBar.ForegroundColor = Windows.UI.Colors.White;

            //}
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
        }

        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            base.OnFileActivated(args);
        }




        #region Events
        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            // Each time a navigation event occurs, update the Back button's visibility
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                ((Frame)sender).CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private void OnResuming(object sender, object e)
        {

        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            // Pass the event args to the back requested handler for handling
            BackRequested?.Invoke(sender, e);

            // If we didn't handle the requerst, do default
            if (!e.Handled)
            {
                if (_rootFrame != null && _rootFrame.CanGoBack)
                {
                    e.Handled = true;
                    _rootFrame.GoBack();
                }
            }
        }
        #endregion

        #region Event Handlers

        /// <summary>
        /// Raises the PropertyChanged event for a property
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
