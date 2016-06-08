using KryptPadCSApp.Classes;
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

        public ICommand SubmitIssueCommand { get; protected set; }

        public string IssuesUri { get; protected set; } = "https://github.com/NeptuneCenturyStudios/KryptPad/issues";
        #endregion

        public FeedbackPageViewModel()
        {
            // Register commands
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            ReviewAppCommand = new Command(async (p) =>
            {

                try
                {
                    // Get the package family name
                    var packageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName;
                    // Launch the uri
                    await Windows.System.Launcher.LaunchUriAsync(new Uri($"ms-windows-store:REVIEW?PFN={packageFamilyName}"));

                }
                catch (Exception)
                {
                    // Failed
                    await DialogHelper.ShowMessageDialogAsync("Could not launch the requested url.");
                }

                

            });

            SubmitIssueCommand = new Command(async (p) =>
            {
                try
                {
                    // Launch the uri
                    await Windows.System.Launcher.LaunchUriAsync(new Uri(IssuesUri));

                }
                catch (Exception)
                {
                    // Failed
                    await DialogHelper.ShowMessageDialogAsync("Could not launch the requested url.");
                }


            });
        }
    }
}
