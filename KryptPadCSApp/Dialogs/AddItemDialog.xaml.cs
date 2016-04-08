using KryptPadCSApp.API;
using KryptPadCSApp.API.Models;
using KryptPadCSApp.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace KryptPadCSApp.Dialogs
{
    public sealed partial class AddItemDialog : ContentDialog
    {
        #region Properties
        private string _itemName;
        /// <summary>
        /// gets or sets the item name
        /// </summary>
        public string ItemName
        {
            get { return _itemName; }
            set
            {
                _itemName = value;
                // Can the user save?
                IsPrimaryButtonEnabled = CanPrimaryButtonExecute;
            }
        }

        public ObservableCollection<ItemTemplate> ItemTemplates { get; set; } = new ObservableCollection<ItemTemplate>();

        //private ItemTemplate _selectedItemTemplate;
        public ItemTemplate SelectedItemTemplate { get; set; }
        //{
        //    get { return _selectedItemTemplate; }
        //    set
        //    {
        //        _selectedItemTemplate = value;


        //    }
        //}
        #endregion


        public AddItemDialog()
        {
            this.InitializeComponent();

            this.DataContext = this;

        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private async void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            // Get the available templates
            var itemTemplates = await ItemTemplate.LoadTemplatesAsync();

            foreach (var itemTemplate in itemTemplates)
            {
                ItemTemplates.Add(itemTemplate);
            }
        }

        private bool CanPrimaryButtonExecute => !string.IsNullOrWhiteSpace(ItemName);
    }
}
