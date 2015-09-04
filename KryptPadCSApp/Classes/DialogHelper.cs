using KryptPadCSApp.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace KryptPadCSApp.Classes
{
    /// <summary>
    /// Common dialogs throughout the app
    /// </summary>
    class DialogHelper
    {
        /// <summary>
        /// Displays the login dialog
        /// </summary>
        public static async void LoginDialog()
        {
            var dialog = new ContentDialog()
            {
                Title = "KryptPad",
                MaxWidth = Window.Current.Bounds.Width,
                Content = new LoginUserControl()
            };

            var res = await dialog.ShowAsync();
        }

        /// <summary>
        /// Displays the create password dialog
        /// </summary>
        public static async void CreatePasswordDialog()
        {
            //prompt for a new password
            var dialog = new ContentDialog()
            {
                Title = "Create Password",
                MaxWidth = Window.Current.Bounds.Width,
                Content = new CreatePasswordUserControl()
            };

            var res = await dialog.ShowAsync();
        }

        /// <summary>
        /// Closes the dialog window
        /// </summary>
        /// <param name="parent">The framework element set as the Content of the ContentDialog</param>
        public static void CloseDialog(FrameworkElement parent)
        {
            if (parent != null)
            {
                //the parent is the host dialog
                var dialog = parent.Parent as ContentDialog;

                if (dialog != null)
                {
                    dialog.Hide();
                }
            }
        }
    }
}
