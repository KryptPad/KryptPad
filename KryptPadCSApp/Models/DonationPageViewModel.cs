using KryptPadCSApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KryptPadCSApp.Models
{
    class DonationPageViewModel
    {

        #region Properties

        public ICommand DonateCommand { get; protected set; }

        #endregion

        public DonationPageViewModel()
        {
            // Register commands
            RegisterCommands();
        }

        private void RegisterCommands()
        {

            DonateCommand = new Command(async (p) =>
            {
                try
                {
                    // Launch the uri
                    await Windows.System.Launcher.LaunchUriAsync(
                        new Uri("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=5547784"));
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
