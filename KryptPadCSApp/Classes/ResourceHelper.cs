using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace KryptPadCSApp.Classes
{
    class ResourceHelper
    {
        /// <summary>
        /// Gets the ResourceLoader for the current view
        /// </summary>
        public static ResourceLoader Loader { get { return ResourceLoader.GetForCurrentView(); } }

        /// <summary>
        /// Gets a string from a resource
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static string GetString(string resource)
        {
            return Loader.GetString(resource);
        }
    }
}
