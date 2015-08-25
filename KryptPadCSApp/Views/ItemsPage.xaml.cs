using KryptPadCSApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        public ItemsPage()
        {
            this.InitializeComponent();
            
        }

        /// <summary>
        /// Handles navigation to this page
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //if we are coming from the add new category page, then the
            //parameter is the new category that was added. set the selected
            //item to the category, and when binding occurs, the pivot control
            //will select the new category
            if (e.Parameter != null && e.Parameter is Category)
            {
                var model = DataContext as ItemsPageViewModel;
                if (model != null)
                {
                    model.SelectedCategory = e.Parameter as Category;
                }
            }
        }

        
    }
}
