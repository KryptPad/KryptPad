using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace KryptPadCSApp.Dialogs
{
    public class ClosableContentDialog : ContentDialog
    {

        #region Properties

        public ContentDialogResult Result { get; private set; }

        #endregion

        #region Methods
        /// <summary>
        /// Hides the dialog, and sets the Result property
        /// </summary>
        /// <param name="result"></param>
        public void Close(ContentDialogResult result)
        {
            // Set the return value
            Result = result;
            // Hide the dialog window
            Hide();
            
        }

        #endregion

    }
}
