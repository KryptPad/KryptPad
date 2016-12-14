using KryptPadCSApp.Dialogs;
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
                "An error occurred while trying to process your request. Make sure you are connected to the internet.", "No Network");
            
            //show
            return await msgBox.ShowAsync();

        }

        /// <summary>
        /// Displays the specified content dialog type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryAction"></param>
        /// <returns></returns>
        public static async Task<ContentDialogResult> ShowNameDialog(Action<NamePromptDialog> primaryAction, string title = null, string originalValue = null)
        {
            // Create instance of content dialog
            var d = new NamePromptDialog();

            // Set the dialog title
            if (title != null)
            {
                d.Title = title;
            }

            if (originalValue != null)
            {
                d.Value = originalValue;
            }

            // Show the dialog
            var res = await d.ShowAsync();

            // Determine which button was fired, and decide if we need to execute the primary action
            if ((res == ContentDialogResult.Primary || d.Result == ContentDialogResult.Primary) && primaryAction != null)
            {
                primaryAction(d);
            }

            return res;
        }

        /// <summary>
        /// Displays the specified content dialog type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryAction"></param>
        /// <returns></returns>
        public static async Task<ContentDialogResult> ShowClosableDialog<T>(Action<T> primaryAction, string title = null) where T : ClosableContentDialog, new()
        {
            // Create instance of content dialog
            var d = new T();

            // Set the dialog title
            if (title != null)
            {
                d.Title = title;
            }

            // Show the dialog
            var res = await d.ShowAsync();

            // Determine which button was fired, and decide if we need to execute the primary action
            if ((res == ContentDialogResult.Primary || d.Result == ContentDialogResult.Primary) && primaryAction != null)
            {
                primaryAction(d);
            }

            return res;
        }
        
        ///// <summary>
        ///// Displays the specified content dialog type
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="primaryAction"></param>
        ///// <returns></returns>
        //public static async Task<ContentDialogResult> ShowDialog<T>(Func<T, Task> primaryAction) where T : ContentDialog, new()
        //{
        //    // Create instance of content dialog
        //    var d = new T();

        //    // Show the dialog
        //    var res = await d.ShowAsync();

        //    // Determine which button was fired, and decide if we need to execute the primary action
        //    if (res == ContentDialogResult.Primary && primaryAction != null)
        //    {
        //        await primaryAction(d);
        //    }

        //    return res;
        //}

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
        /// Creates a confirm prompt for the user
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="yes"></param>
        /// <returns></returns>
        public static async Task<IUICommand> Confirm(string prompt)
        {
            return await Confirm(prompt, "CONFIRM", null);
        }

        /// <summary>
        /// Creates a confirm prompt for the user
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="yes"></param>
        /// <returns></returns>
        public static async Task<IUICommand> Confirm(string prompt, UICommandInvokedHandler yes)
        {
            return await Confirm(prompt, "CONFIRM", yes);
        }

        /// <summary>
        /// Creates a confirm prompt for the user
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static async Task<IUICommand> Confirm(string prompt, string title)
        {
            return await Confirm(prompt, title, null);
        }

        /// <summary>
        /// Creates a confirm prompt for the user
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="title"></param>
        /// <param name="yes"></param>
        /// <returns></returns>
        public static async Task<IUICommand> Confirm(string prompt, string title, UICommandInvokedHandler yes)
        {
            var msg = new MessageDialog(prompt, title);
            //, async (ap) => { })
            msg.Commands.Add(new UICommand("Yes", yes, 1));
            msg.Commands.Add(new UICommand("No", null, 2));

            msg.DefaultCommandIndex = 1;

            return await msg.ShowAsync();
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
