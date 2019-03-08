using KryptPad.Api.Models;
using KryptPadCSApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace KryptPadCSApp.Models
{
    class BasePageModel : BaseModel
    {

        #region Properties

        /// <summary>
        /// Gets or sets whether the auto login is temporarily disabled
        /// </summary>
        protected bool DisableAutoLogin
        {
            get { return (App.Current as App).DisableAutoLogin; }
            set { (App.Current as App).DisableAutoLogin = value; }
        }

        private bool _isBusy;
        /// <summary>
        /// Gets or sets the IsBusy status, indicating that the page is doing something that blocks user interaction
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                // Notify change
                OnPropertyChanged(nameof(IsBusy));

                // Call the OnIsBusyChanged method so derived classes can handle it
                OnIsBusyChanged();
            }
        }

        private Visibility _busyIndicatorVisibility;
        /// <summary>
        /// Gets or sets whether the ui element is visible
        /// </summary>
        public Visibility BusyIndicatorVisibility
        {
            get { return _busyIndicatorVisibility; }
            set
            {
                _busyIndicatorVisibility = value;
                // Notify change
                OnPropertyChanged(nameof(BusyIndicatorVisibility));

            }
        }
        
        /// <summary>
        /// Gets the command to view the license terms
        /// </summary>
        public Command ViewLicenseTermsCommand { get; protected set; }

        public Command ViewPrivacyPolicyCommand { get; protected set; }

        public Command ViewFeedBackCommand { get; protected set; }

        public Command ViewDonateCommand { get; protected set; }

        public Command ViewAboutCommand { get; protected set; }
        #endregion

        #region Ctor
        public BasePageModel()
        {
            BusyIndicatorVisibility = Visibility.Collapsed;

            //// Set commands
            //ViewLicenseTermsCommand = new Command((p) => { NavigationHelper.GoToLicenseTerms(); });
            //ViewPrivacyPolicyCommand = new Command((p) => { NavigationHelper.GoToPrivacyPolicy(); });
            //ViewFeedBackCommand = new Command((p) => { NavigationHelper.GoToFeedback(); });
            //ViewDonateCommand = new Command((p) => { NavigationHelper.GoToDonate(); });
            //ViewAboutCommand = new Command((p) => { NavigationHelper.GoToAbout(); });
        }
        #endregion

        #region Helper Methods

        #endregion

        #region Event Handlers

        /// <summary>
        /// When overriden in a derived class, handles the IsBusy property
        /// </summary>
        protected virtual void OnIsBusyChanged()
        {
            // Set the visibility of the busy indicator
            BusyIndicatorVisibility = IsBusy ? Visibility.Visible : Visibility.Collapsed;
            // If the page is loaded inside of the MainPage, then set the IsBusy property on
            // that page. Otherwise, we can just ignore it
            ((RootPage)((Frame)(Window.Current.Content )).Content)?.SetIsBusy(IsBusy);
        }

        #endregion
    }
}
