using KryptPad.Api.Models;
using KryptPadCSApp.Interfaces;
using KryptPadCSApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class NewItemPage : Page
    {
        public NewItemPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles navigation to this page
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Initialize the model with the item and the category
            if (e.Parameter is EditItemPageParams)
            {
                //get the page params
                var pageParams = e.Parameter as EditItemPageParams;
                //get the model
                var model = DataContext as EditItemPageViewModel;

                //set the category of the NewItemPageViewModel
                if (pageParams != null && model != null)
                {
                    //set properties on the model
                    await model.LoadItemAsync(pageParams.Item, pageParams.Category);
                        
                }
            }
        }

        
    }
}
