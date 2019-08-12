using KryptPadCSApp.Classes;
using KryptPadCSApp.Views;
using System.Reflection;

namespace KryptPadCSApp.Models
{
    class AboutPageViewModel : BasePageModel
    {
        #region Properties
        /// <summary>
        /// Gets the assembly version
        /// </summary>
        public string Version { get { return typeof(App).GetTypeInfo().Assembly.GetName().Version.ToString(); } }

        //public Command ViewTermsPageCommand { get; protected set; }
        //public Command ViewPrivacyPageCommand { get; protected set; }
        #endregion

        #region Contstructor
        //public AboutPageViewModel()
        //{
        //    RegisterCommands();
        //}

        #endregion

        #region Methods
        //private void RegisterCommands()
        //{
        //    ViewTermsPageCommand = new Command(ViewTermsCommandHandler);
        //    ViewPrivacyPageCommand = new Command(ViewPrivacyPageCommandHandler);
        //}
        #endregion

        #region Command handlers
        //private void ViewTermsCommandHandler(object p)
        //{
        //    // Go to terms page
        //    NavigationHelper.Navigate(typeof(TermsPage), null);
        //}

        //private void ViewPrivacyPageCommandHandler(object p)
        //{
        //    // Go to privacy page
        //    NavigationHelper.Navigate(typeof(PrivacyPage), null);
        //}
        #endregion

    }
}
