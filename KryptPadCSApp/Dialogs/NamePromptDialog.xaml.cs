using KryptPadCSApp.Classes;
using KryptPadCSApp.Models.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
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
    public sealed partial class NamePromptDialog : ClosableContentDialog
    {

        #region Properties

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public string Value
        {
            get { return (DataContext as NamePromptDialogViewModel).Value; }
            set { (DataContext as NamePromptDialogViewModel).Value = value; }
        }
        #endregion

        public NamePromptDialog()
        {
            this.InitializeComponent();

            // Determine the command's can execute state, and hook into the changed event
            var m = DataContext as NamePromptDialogViewModel;
            if (m != null && m.PrimaryCommand != null)
            {
                m.PrimaryCommand.CanExecuteChanged += (sender, e) =>
                {
                    IsPrimaryButtonEnabled = (sender as ICommand).CanExecute(null);
                };
            }

        }

        private void NameTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            // Trigger the primary action
            if (e.Key == Windows.System.VirtualKey.Enter && PrimaryButtonCommand.CanExecute(null))
            {
                // Yes, it can execute, call it
                PrimaryButtonCommand.Execute(null);

                // Close the dialog
                Hide(ContentDialogResult.Primary);
            }
        }
    }
}
