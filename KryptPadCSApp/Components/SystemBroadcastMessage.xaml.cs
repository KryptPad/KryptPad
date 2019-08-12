using KryptPad.Api;
using KryptPadCSApp.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace KryptPadCSApp.Components
{
    public sealed partial class SystemBroadcastMessage : UserControl
    {
        public SystemBroadcastMessage()
        {
            this.InitializeComponent();
        }

        #region System broadcast message

        /// <summary>
        /// Fetch the system message if there is one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BroadcastMessageText_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                // Get broadcast message
                var message = await KryptPadApi.GetBroadcastMessage();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    BroadcastMessage.Visibility = Visibility.Visible;
                    BroadcastMessageText.Text = message;
                }

            }
            catch (Exception) { }

        }

        private void BroadcastCloseButton_Click(object sender, RoutedEventArgs e)
        {
            BroadcastMessage.Visibility = Visibility.Collapsed;
        }
        #endregion
    }
}
