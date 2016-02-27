using KryptPadCSApp.API;
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
    public sealed partial class AddItemDialog : ContentDialog
    {
        /// <summary>
        /// Gets or sets the category id
        /// </summary>
        private int CategoryId { get; set; }

        public AddItemDialog(int categoryId)
        {
            this.InitializeComponent();

            // Set category
            CategoryId = categoryId;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

            try
            {
                // Save the item to the api
                var resp = await KryptPadApi.SaveItemAsync(CategoryId, null);
            }catch
            {

            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
