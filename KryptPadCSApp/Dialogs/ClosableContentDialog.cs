using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace KryptPadCSApp.Dialogs
{
    public class ClosableContentDialog : ContentDialog, INotifyPropertyChanged
    {

        #region Properties
        TaskCompletionSource<ContentDialogResult> tcs = new TaskCompletionSource<ContentDialogResult>();

        public event PropertyChangedEventHandler PropertyChanged;

        //public bool Cancel { get; set; }
        #endregion

        //public ClosableContentDialog()
        //{
        //    Closing += (sender, e) =>
        //    {
        //        if (Cancel)
        //        {
        //            e.Cancel = Cancel;
        //            // Reset
        //            Cancel = false;

        //        }
        //    };
        //}



        #region Methods
        
        public void Hide(ContentDialogResult result)
        {
            tcs.TrySetResult(ContentDialogResult.Primary);
            base.Hide();
        }

        public new IAsyncOperation<ContentDialogResult> ShowAsync()
        {
            var asyncOperation = base.ShowAsync();
            asyncOperation.AsTask().ContinueWith(task => tcs.TrySetResult(task.Result));
            return tcs.Task.AsAsyncOperation();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Raises the PropertyChanged event for a property
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
