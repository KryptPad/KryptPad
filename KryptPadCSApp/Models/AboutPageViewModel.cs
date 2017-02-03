using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.Models
{
    class AboutPageViewModel : BasePageModel
    {
        #region Properties
        /// <summary>
        /// Gets the assembly version
        /// </summary>
        public string Version {  get { return typeof(App).GetTypeInfo().Assembly.GetName().Version.ToString(); } }

        #endregion
    }
}
