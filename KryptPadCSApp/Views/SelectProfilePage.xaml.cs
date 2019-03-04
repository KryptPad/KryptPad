using KryptPadCSApp.Classes;
using KryptPadCSApp.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace KryptPadCSApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectProfilePage : Page
    {
        //private SystemNavigationManager _currentView = SystemNavigationManager.GetForCurrentView();

        public SelectProfilePage()
        {
            this.InitializeComponent();


        }

        private async void SelectProfileViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Load the profiles
            var model = DataContext as SelectProfilePageViewModel;

            await model.GetProfilesAsync();
            await model.CheckIfWindowsHelloSupported();

        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            // If we came from the main page, clear the backstack
            if (Frame.BackStack.Count > 0 && Frame.BackStack[Frame.BackStack.Count - 1].SourcePageType == typeof(MainPage))
            {
                NavigationHelper.RemoveLastFromBackStack();

            }

            base.OnNavigatedTo(e);
        }

    }
}
