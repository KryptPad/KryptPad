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
                // Launch the uri
                await Windows.System.Launcher.LaunchUriAsync(
                    new Uri("https://www.paypal.com/us/cgi-bin/webscr?cmd=_flow&SESSION=EHNMjs-f8NSU_h1EkfBswaKbAWOC9Mu-bo1Hvydnh-MrSsDocpenyvZ3pq4&dispatch=5885d80a13c0db1f8e263663d3faee8d6625bf1e8bd269586d425cc639e26c6a"));

            });
        }

    }
}
