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
        private ApiItem draggedItem;

        public ItemsPage()
        {
            this.InitializeComponent();
            
        }

        private async void ItemsViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            await (DataContext as ItemsPageViewModel).RefreshCategoriesAsync();
        }

        private void GridView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            draggedItem = e.Items[0] as ApiItem;

            e.Data.RequestedOperation = DataPackageOperation.Move;
        }

        private async void VariableSizedWrapGrid_Drop(object sender, DragEventArgs e)
        {
            try

            {

                if (draggedItem != null)

                {
                    //var m = DataContext as ItemsPageViewModel;
                    //var sourceCategory = draggedItem.CategoryId;


                    //var child = (((ItemsWrapGrid)sender).Children[0] as GridViewItem).Content as ApiItem;

                    //draggedItem.CategoryId = child.CategoryId;

                    //await m.RefreshCategoriesAsync();

                    //child.Cate.BookList.Add(draggedItem);

                    //sourceCategory.BookList.Remove(draggedItem);

                    draggedItem = null;

                }

            }

            catch (Exception ex)

            {

            }
        }

        private void VariableSizedWrapGrid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
        }
    }
}
