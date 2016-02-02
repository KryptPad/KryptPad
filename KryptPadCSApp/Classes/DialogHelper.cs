using KryptPadCSApp.Dialogs;
using KryptPadCSApp.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
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
        /// Shows a message box with custom content
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<IUICommand> ShowMessageDialogAsync(string content)
        {
            var msgBox = new MessageDialog(content);
            //show
            return await msgBox.ShowAsync();
            
        }

        /// <summary>
        /// Shows a generic connection error dialog
        /// </summary>
        /// <returns></returns>
        public static async Task<IUICommand> ShowConnectionErrorMessageDialog()
        {
            var msgBox = new MessageDialog(
                "An error occurred while trying to process your request. Make sure you are connected to the internet.");
            //show
            return await msgBox.ShowAsync();

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
