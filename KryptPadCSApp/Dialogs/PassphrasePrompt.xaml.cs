using KryptPadCSApp.Classes;
using KryptPadCSApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public sealed partial class PassphrasePrompt : ClosableContentDialog
    {

        #region Properties

        /// <summary>
        /// Gets the passphrase the user entered
        /// </summary>
        public string Passphrase { get { return PasswordTextBox.Password; } }
        #endregion

        public PassphrasePrompt()
        {
            this.InitializeComponent();
        }

        #region Methods
        private async Task<bool> ValidateInput()
        {
            // If the user entered a passphrase, return it and hide the dialog
            if (string.IsNullOrWhiteSpace(PasswordTextBox.Password))
            {   
                await DialogHelper.ShowMessageDialogAsync("Please enter your passphrase for this profile.");
                return false;
            }

            return true;
        }
        #endregion

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // If the user entered a passphrase, return it and hide the dialog
            if (!await ValidateInput())
            {
                args.Cancel = true;
            }
        }

        
        private async void PasswordTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                // If the user entered a passphrase, return it and hide the dialog
                if (await ValidateInput())
                {
                    Close(ContentDialogResult.Primary);
                }
            }
        }

        
    }
}
