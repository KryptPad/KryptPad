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
    public sealed partial class NamePromptDialog : ContentDialog
    {

        #region Properties
        
        /// <summary>
        /// Gets or sets the name textbox field
        /// </summary>
        public string NameValue
        {
            get
            {
                return NameTextBox.Text;
            }
            set
            {
                NameTextBox.Text = value;
            }
        }
        #endregion

        public NamePromptDialog()
        {
            this.InitializeComponent();
        }
        
        #region Methods
        private async Task<bool> ValidateInput()
        {
            // If the user entered a passphrase, return it and hide the dialog
            if (string.IsNullOrWhiteSpace(NameValue))
            {
                await DialogHelper.ShowMessageDialogAsync("Please enter a value.");
                return false;
            }

            return true;
        }
        #endregion

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Check if user entered a name
            if (!await ValidateInput())
            {
                args.Cancel = true;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private async void NameTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            //// Check if user entered a name
            //if (await ValidateInput())
            //{
                
            //}
        }
    }
}
