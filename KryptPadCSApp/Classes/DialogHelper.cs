using KryptPadCSApp.Dialogs;
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

        //store a reference to the currently open ContentDialog
        private static ContentDialog _currentDialog;

        /// <summary>
        /// Gets or sets the result object
        /// </summary>
        private static object _result;

        /// <summary>
        /// Displays the login dialog
        /// </summary>
        public static async void LoginDialog()
        {

            _currentDialog = new ContentDialog()
            {
                Title = "KryptPad",
                MaxWidth = Window.Current.Bounds.Width,
                Content = new LoginUserControl()
            };

            var res = await _currentDialog.ShowAsync();
        }

        /// <summary>
        /// Displays the authentication dialog
        /// </summary>
        public static async void AuthenticateDialog()
        {
            _currentDialog = new ContentDialog()
            {
                Title = "Authenticate",
                MaxWidth = Window.Current.Bounds.Width,
                Content = new AuthenticateUserControl()
            };

            var res = await _currentDialog.ShowAsync();
        }

        /// <summary>
        /// Displays the create password dialog
        /// </summary>
        public static async void CreatePasswordDialog()
        {
            //prompt for a new password
            _currentDialog = new ContentDialog()
            {
                Title = "Create Password",
                MaxWidth = Window.Current.Bounds.Width,
                Content = new CreatePasswordUserControl()
            };

            var res = await _currentDialog.ShowAsync();
        }

        /// <summary>
        /// Displays the create password dialog
        /// </summary>
        public static async Task<object> ShowAddFieldDialog()
        {
            //prompt for a new password
            _currentDialog = new AddFieldDialog()
            {
                MaxWidth = Window.Current.Bounds.Width
            };

            //show dialog
            await _currentDialog.ShowAsync();

            return _result;

            //return await Task.Factory.StartNew(async () => {
                
            //    //return result
            //    return _result;
            //});
 
        }

        /// <summary>
        /// Closes the dialog window
        /// </summary>
        public static void CloseDialog()
        {
            CloseDialog(null);
        }

        /// <summary>
        /// Closes the dialog window
        /// </summary>
        /// <param name="parent">The framework element set as the Content of the ContentDialog</param>
        public static void CloseDialog(object result)
        {
            if (_currentDialog != null)
            {
                _result = result;
                //hide the dialog
                _currentDialog.Hide();
            }
        }
    }
}
