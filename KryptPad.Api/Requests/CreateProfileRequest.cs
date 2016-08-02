using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPad.Api.Requests
{
    public class CreateProfileRequest
    {
        /// <summary>
        /// Gets or sets the name of the profile
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the passphrase for this profile
        /// </summary>
        public string Passphrase { get; set; }

        /// <summary>
        /// Gets or sets the confirm passphrase for this profile
        /// </summary>
        public string ConfirmPassphrase { get; set; }
    }
}
