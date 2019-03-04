using KryptPad.Api.Models;
using KryptPadCSApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace KryptPadCSApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ItemsPage : Page
    {
        //private ApiItem draggedItem;

        public ItemsPage()
        {
            this.InitializeComponent();
                        
        }

        #region Overrides

        #endregion

        #region Events

        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            var m = DataContext as ItemsPageViewModel;

            if (!m.CanClickItem)
            {
                // Go back to none selection mode
                m.SelectionMode = ListViewSelectionMode.None;
                e.Handled = true;
                
            }
        }
        
        private async void ItemsViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            await (DataContext as ItemsPageViewModel).RefreshCategoriesAsync();

            // Set the selection changed event
            ItemsGridView.SelectionChanged += ItemsGridView_SelectionChanged;
        }
               
        private void ItemsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var m = DataContext as ItemsPageViewModel;

            // Only manage selected items if selection mode is set to multiple
            if (m.SelectionMode == ListViewSelectionMode.Multiple)
            {
                // Add items
                foreach (ApiItem item in e.AddedItems)
                {
                    m.SelectedItems.Add(item);
                }

                // Remove items
                foreach (ApiItem item in e.RemovedItems)
                {
                    m.SelectedItems.Remove(item);
                }

            }
        }

        #endregion
    }
}
