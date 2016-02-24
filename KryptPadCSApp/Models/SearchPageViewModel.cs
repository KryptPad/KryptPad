using KryptPadCSApp.API;
using KryptPadCSApp.API.Models;
using KryptPadCSApp.Classes;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace KryptPadCSApp.Models
{
    class SearchPageViewModel : BasePageModel
    {
        #region Properties

        /// <summary>
        /// Stores a list of all the items retrieved by the api
        /// </summary>
        protected ObservableCollection<ApiItem> Items { get; private set; }

        /// <summary>
        /// Gets the filtered view of items
        /// </summary>
        public CollectionViewSource ItemsView { get; protected set; } = new CollectionViewSource();

        private string _searchText;
        /// <summary>
        /// Gets or sets the search text
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                // Notify change
                OnPropertyChanged(nameof(SearchText));
                // Trigger search
                SearchForItems();
            }
        }

        /// <summary>
        /// Gets or sets the command that is fired when an item is clicked
        /// </summary>
        public Command ItemClickCommand { get; set; }

        #endregion

        #region Ctor
        public SearchPageViewModel()
        {
            RegisterCommands();
        }
        #endregion

        #region Helper methods

        private void RegisterCommands()
        {
            //handle item click
            ItemClickCommand = new Command((p) =>
            {
                var item = p as ApiItem;

                // Navigate to edit
                Navigate(typeof(NewItemPage), new EditItemPageParams() { Item = item });


            });
        }
        
        /// <summary>
        /// Searches for items using a search string
        /// </summary>
        /// <param name="searchText"></param>
        public async void SearchForItems()
        {
            try
            {
                //if (Items == null)
                //{
#if DEBUG
                    Debug.WriteLine("Getting items from server");
#endif
                    // Get the items if not already got
                    var resp = await KryptPadApi.GetAllItemsAsync(CurrentProfile, SearchText, AccessToken, Passphrase);

                    // Create collection
                    //Items = new ObservableCollection<ApiItem>();

                    // Add items
                    //foreach (var item in resp.Items)
                    //{
                        //Items.Add(item);
                    //}
                //}

                //// Search for item with name that contains the search text
                //var view = Items.Where((i) =>
                //    i.Name.IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) >= 0);
                // Add view to the ItemsView object
                ItemsView.Source = resp.Items;

                // Refresh
                OnPropertyChanged(nameof(ItemsView));
            }
            catch (Exception ex)
            {
                // Operation failed
                await DialogHelper.ShowMessageDialogAsync(ex.Message);
            }
        }

        #endregion
    }
}
