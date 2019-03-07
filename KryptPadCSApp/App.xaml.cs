using KryptPad.Api;
using KryptPadCSApp.Classes;
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
        public event PropertyChangedEventHandler PropertyChanged;
        
        #region Properties

        /// <summary>
        /// Gets or sets whether the auto login is temporarily disabled
        /// </summary>
        internal bool DisableAutoLogin { get; set; }

        private SignInStatus _signInStatus;
        /// <summary>
        /// Gets or sets the time remaining on the session
        /// </summary>
        internal SignInStatus SignInStatus
        {
            get { return _signInStatus; }
            set
            {
                // Set value
                _signInStatus = value;
                // Value changed
                OnPropertyChanged(nameof(SignInStatus));

            }

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
            // Get the lifetime app installation instance id
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings != null)
            {
                // Do we have a GUID?
                var appId = localSettings.Values["AppId"];
                if (appId == null || (Guid)appId == Guid.Empty)
                {
                    appId = Guid.NewGuid();
                    localSettings.Values["AppId"] = appId;
                }

                // Set the app ID in the API
                KryptPadApi.AppId = (Guid)appId;
            }


            var _rootFrame = Window.Current.Content as Frame;
            if (_rootFrame == null)
            {
                // Create main page instance
                _rootFrame = new Frame();

                // Add some event handlers
                _rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = _rootFrame;

            }


            if (_rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                _rootFrame.Navigate(typeof(RootPage), e.Arguments);
            }

            // Ensure the current window is active
            Window.Current.Activate();

            // Set up back request handler
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;

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
               

        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            // If we didn't handle the request, do default
            if (!e.Handled)
            {
                NavigationHelper.GoBack(e);
            }
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
