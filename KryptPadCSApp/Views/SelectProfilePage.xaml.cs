using KryptPadCSApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace KryptPadCSApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectProfilePage : Page
    {
        public SelectProfilePage()
        {
            this.InitializeComponent();
        }
        
        private async void SelectProfileViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Focus on the passphrase textbox
            PassphrasePasswordBox.Focus(FocusState.Programmatic);

            // Load the profiles
            await (DataContext as SelectProfilePageViewModel).GetProfilesAsync();
        }

        private void PassphrasePasswordBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                // Get context
                var m = DataContext as SelectProfilePageViewModel;
                if (m.EnterProfileCommand.CanExecute(null))
                {
                    // Execute button command
                    m.EnterProfileCommand.Execute(null);
                }
            }
        }
    }
}
