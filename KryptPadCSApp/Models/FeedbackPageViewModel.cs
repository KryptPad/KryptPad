using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KryptPadCSApp.Models
{
    class FeedbackPageViewModel : BasePageModel
    {

        #region Properties

        public ICommand ReviewAppCommand { get; protected set; }


        #endregion

        public FeedbackPageViewModel() {

            RegisterCommands();
        }

        private void RegisterCommands()
        {
            ReviewAppCommand = new Command(async (p) => {
                // Get the package family name
                var packageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName;
                // Launch the uri
                await Windows.System.Launcher.LaunchUriAsync(new Uri($"ms-windows-store:REVIEW?PFN={packageFamilyName}"));

            } );
        }
    }
}
