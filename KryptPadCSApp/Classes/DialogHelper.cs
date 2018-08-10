using KryptPadCSApp.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
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
        
        #region MessageDialog
        
        /// <summary>
        /// Shows a generic connection error dialog
        /// </summary>
        /// <returns></returns>
        public static async Task<IUICommand> ShowGenericErrorDialogAsync(Exception ex)
        {
            // TODO: Implement some kind of error logging.

            // This is a generic error message
            return await ShowMessageDialogAsync(ResourceHelper.GetString("GenericError"), ResourceHelper.GetString("Error"));

        }

        /// <summary>
        /// Shows a message box with custom message
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<IUICommand> ShowMessageDialogAsync(string content)
        {
            return await ShowMessageDialogAsync(content, string.Empty);
        }

        /// <summary>
        /// Shows a message box with custom message and title
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<IUICommand> ShowMessageDialogAsync(string content, string title)
        {
            var msgBox = new MessageDialog(content, title);
            // Show
            return await msgBox.ShowAsync();

        }

        #endregion

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

        #region Confirm
        
        /// <summary>
        /// Creates a confirm prompt for the user
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="yes"></param>
        /// <returns></returns>
        public static async Task<IUICommand> Confirm(string prompt, UICommandInvokedHandler yes)
        {
            return await Confirm(prompt, ResourceHelper.GetString("Confirm"), yes);
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
            return await Confirm(prompt, title, yes, null);
        }

        /// <summary>
        /// Creates a confirm prompt for the user
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="title"></param>
        /// <param name="yes"></param>
        /// <param name="no"></param>
        /// <returns></returns>
        public static async Task<IUICommand> Confirm(string prompt, string title, UICommandInvokedHandler yes, UICommandInvokedHandler no)
        {
            var msg = new MessageDialog(prompt, title);
            //, async (ap) => { })
            msg.Commands.Add(new UICommand("Yes", yes, 1));
            msg.Commands.Add(new UICommand("No", no, 2));

            msg.DefaultCommandIndex = 1;

            return await msg.ShowAsync();
        }

        #endregion
        
    }
}
