using KryptPad.Api;
using KryptPad.Api.Models;
using KryptPadCSApp.Classes;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace KryptPadCSApp.Dialogs
{
    public sealed partial class ProfileDetailsDialog : ContentDialog
    {

        #region Properties

        private string _profileName;

        public string ProfileName
        {
            get { return _profileName; }
            set
            {
                _profileName = value;
                // Notify change
                //OnPropertyChanged(nameof(ProfileName));
                // Can we execute command?
                IsPrimaryButtonEnabled = CanSaveProfile;
            }
        }

        private string _profilePassphrase;

        public string ProfilePassphrase
        {
            get { return _profilePassphrase; }
            set
            {
                _profilePassphrase = value;
                // Notify change
                //OnPropertyChanged(nameof(ProfilePassphrase));
                // Can we execute command?
                IsPrimaryButtonEnabled = CanSaveProfile;
            }
        }
        private string _confirmProfilePassphrase;
        
        public string ConfirmProfilePassphrase
        {
            get { return _confirmProfilePassphrase; }
            set
            {
                _confirmProfilePassphrase = value;
                // Notify change
                //OnPropertyChanged(nameof(ConfirmProfilePassphrase));
                // Can we execute command?
                IsPrimaryButtonEnabled = CanSaveProfile;

            }
        }


        #endregion

        public ProfileDetailsDialog()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        #region Methods
        private async Task SaveProfile()
        {

            try
            {
                // Create a new profile
                var profile = new ApiProfile()
                {
                    Name = ProfileName
                };

                // Call api to create the profile.
                var response = await KryptPadApi.CreateProfileAsync(profile, ProfilePassphrase);

                profile.Id = response.Id;

                // Go to profile
                await KryptPadApi.LoadProfileAsync(profile, ProfilePassphrase);

                // Redirect to the main item list page
                NavigationHelper.Navigate(typeof(ItemsPage), null);
                
                // Hide the dialog
                Hide();
            }
            catch (WebException ex)
            {
                // Something went wrong in the api
                await DialogHelper.ShowMessageDialogAsync(ex.Message);

            }
            catch (Exception)
            {
                // Failed
                await DialogHelper.ShowConnectionErrorMessageDialog();

            }

        }

        #endregion

        #region Events
        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Disable the primary button
            IsPrimaryButtonEnabled = false;

            // Force the dialog to stay open until the operation completes.
            // We will call Hide() when the api calls are done.
            args.Cancel = true;

            // Save the profile
            await SaveProfile();

            // Restore the button
            IsPrimaryButtonEnabled = CanSaveProfile;
        }

        private async void TriggerAction(object sender, KeyRoutedEventArgs e)
        {
            // Trigger the primary action
            if (e.Key == Windows.System.VirtualKey.Enter && IsPrimaryButtonEnabled)
            {
                // Disable the primary button
                IsPrimaryButtonEnabled = false;

                // Save the profile
                await SaveProfile();
                
                // Restore the button
                IsPrimaryButtonEnabled = CanSaveProfile;
            }
        }


        private bool CanSaveProfile => !string.IsNullOrWhiteSpace(ProfileName)
            && !string.IsNullOrWhiteSpace(ProfilePassphrase)
            && !string.IsNullOrWhiteSpace(ConfirmProfilePassphrase);
        #endregion

    }
}
