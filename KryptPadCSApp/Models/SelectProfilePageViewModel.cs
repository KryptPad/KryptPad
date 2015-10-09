using KryptPadCSApp.API;
using KryptPadCSApp.API.Models;
using KryptPadCSApp.API.Responses;
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

        public Command CreateProfileCommand { get; protected set; }
        #endregion

        public SelectProfilePageViewModel()
        {
            RegisterCommands();

            var t = Getprofiles();
        }

        /// <summary>
        /// Gets the profiles for the user
        /// </summary>
        /// <returns></returns>
        private async Task Getprofiles()
        {
            //call the api and get some data!
            try
            {
                var response = await KryptPadApi.GetProfilesAsync(AccessToken);
                
                //check response
                if (response is ProfileResponse)
                {
                    var profiles = (response as ProfileResponse).Profiles;

                    foreach (var profile in profiles)
                    {
                        //add profile to list
                        Profiles.Add(profile);
                    }
                }
                else
                {
                    throw new Exception();
                }
                
            }
            catch (Exception)
            {
                await DialogHelper.ShowConnectionErrorMessageDialog();

            }
        }

        private void RegisterCommands()
        {
            CreateProfileCommand = new Command(async (p)=> {

                // Create a new profile.
                var profile = new ApiProfile()
                {
                    Name = "New Profile"
                };

                // Call api to create the profile.
                var response = await KryptPadApi.CreateProfile(profile, AccessToken);

            });
        }
    }
}
