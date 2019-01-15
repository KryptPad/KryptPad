using KryptPad.Api;
using KryptPad.Api.Models;
using KryptPad.Api.Responses;
using KryptPadCSApp.Classes;
using KryptPadCSApp.Collections;
using KryptPadCSApp.Dialogs;
using KryptPadCSApp.Models.Dialogs;
using KryptPadCSApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace KryptPadCSApp.Models
{
    class FavoritesPageViewModel : BasePageModel
    {
        #region Properties
        /// <summary>
        /// Gets the collection of categories
        /// </summary>
        public ObservableCollection<ApiItem> Items { get; protected set; } = new ObservableCollection<ApiItem>();

        private Visibility _emptyMessageVisibility;
        /// <summary>
        /// Gets or sets whether the empty message is visible
        /// </summary>
        public Visibility EmptyMessageVisibility
        {
            get { return _emptyMessageVisibility; }
            set
            {
                _emptyMessageVisibility = value;
                // Changed
                OnPropertyChanged(nameof(EmptyMessageVisibility));
            }
        }

        /// <summary>
        /// Gets or sets the command that is fired when an item is clicked
        /// </summary>
        public Command ItemClickCommand { get; set; }

        /// <summary>
        /// Gets or sets the command that is fired when user sets or unsets a favorite
        /// </summary>
        public Command SetFavoriteCommand { get; set; }


        #endregion


        public FavoritesPageViewModel()
        {
            // Register commands
            RegisterCommands();

            // Set properties
            EmptyMessageVisibility = Visibility.Collapsed;

        }

        /// <summary>
        /// Registers commands for UI elements
        /// </summary>
        private void RegisterCommands()
        {

            // Handle item click
            ItemClickCommand = new Command((p) =>
            {
                var item = p as ApiItem;
                var category = new ApiCategory();

                // Navigate to edit
                NavigationHelper.Navigate(typeof(NewItemPage),
                    new EditItemPageParams()
                    {
                        Category = category,
                        Item = item
                    });


            });

            // Handle setting favorites
            SetFavoriteCommand = new Command(SetFavoriteCommandHandler);
        }

        #region Methods

        /// <summary>
        /// Get the list of categories from the database
        /// </summary>
        /// <returns></returns>
        public async Task RefreshItemsAsync()
        {
            // Set busy
            IsBusy = true;

            try
            {
                // Get the items if not already got
                var resp = await KryptPadApi.GetFavoritesAsync();

                // Set the list to our list of categories
                foreach (var item in resp.Items)
                {
                    Items.Add(item);
                }

                // Refresh
                OnPropertyChanged(nameof(Items));

                // Show empty message if there are no categories
                EmptyMessageVisibility = Items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (WebException ex)
            {
                // Something went wrong in the api
                await DialogHelper.ShowMessageDialogAsync(ex.Message);
            }
            catch (Exception ex)
            {
                // Failed
                await DialogHelper.ShowGenericErrorDialogAsync(ex);
            }

            // Not busy any more
            IsBusy = false;

        }

        #endregion

        #region Command handlers

        private async void SetFavoriteCommandHandler(object obj)
        {
            var item = obj as ApiItem;
            try
            {

                // Remove favorite
                await KryptPadApi.DeleteItemFromFavoritesAsync(item);
                // Remove from list
                Items.Remove(item);

            }
            catch (WebException ex)
            {
                // Something went wrong in the api
                await DialogHelper.ShowMessageDialogAsync(ex.Message);
            }
            catch (Exception ex)
            {
                // Failed
                await DialogHelper.ShowGenericErrorDialogAsync(ex);
            }
        }
        #endregion


    }
}
