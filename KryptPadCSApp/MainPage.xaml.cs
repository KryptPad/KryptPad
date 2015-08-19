using KryptPadCSApp.Models;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace KryptPadCSApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage(Frame frame)
        {
            this.InitializeComponent();

            //set context
            DataContext = new MainPageViewModel();
            //var text = "Some text to encrypt that is a lot longer than the first one and should be a bigger cypher text size";

            //var cypherText = KryptPad.Security.Encryption.Encrypt(text, "P@ssw0rd!");

            //var decryptedText = KryptPad.Security.Encryption.Decrypt(cypherText, "P@ssw0rd!");

            ShellSplitView.Content = frame;


        }

        private void OnMenuButtonClicked(object sender, RoutedEventArgs e)
        {
            ShellSplitView.IsPaneOpen = !ShellSplitView.IsPaneOpen;
            ((RadioButton)sender).IsChecked = false;
        }

        private void OnHomeButtonChecked(object sender, RoutedEventArgs e)
        {
            ShellSplitView.IsPaneOpen = false;
            if (ShellSplitView.Content != null)
                ((Frame)ShellSplitView.Content).Navigate(typeof(ItemsPage));
        }

    }
}
