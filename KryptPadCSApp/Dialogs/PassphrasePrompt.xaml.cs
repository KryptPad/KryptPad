using KryptPadCSApp.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class PassphrasePrompt : ContentDialog
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

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // If the user entered a passphrase, return it and hide the dialog
            if (string.IsNullOrWhiteSpace(PasswordTextBox.Password))
            {
                args.Cancel = true;
                await DialogHelper.ShowMessageDialogAsync("Please enter your passphrase for this profile.");

            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }
    }
}
