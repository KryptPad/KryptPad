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

        public bool Cancel { get; set; }
        #endregion

        public ClosableContentDialog()
        {
            Closing += (sender, e) =>
            {
                if (Cancel)
                {
                    e.Cancel = Cancel;
                    // Reset
                    Cancel = false;
                    
                }
            };
        }

        

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
