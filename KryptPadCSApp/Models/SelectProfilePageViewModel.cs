using KryptPadCSApp.API;
using KryptPadCSApp.API.Models;
using KryptPadCSApp.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KryptPadCSApp.Models
{
    class SelectProfilePageViewModel : BasePageModel
    {
        #region Properties

        /// <summary>
        /// Gets the list of profiles for a user
        /// </summary>
        public ObservableCollection<ApiProfile> Profiles { get; protected set; } = new ObservableCollection<ApiProfile>();


        #endregion

        public SelectProfilePageViewModel()
        {
            //call the api and get some data!
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    var profiles = await KryptPadApi.GetProfilesAsync(AccessToken);
                    //update the collection using the dispatcher
                    await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { });
                }
                catch (Exception)
                {
                    var t = DialogHelper.ShowConnectionErrorMessageDialog();
                }
            });
            

        }

    }
}
