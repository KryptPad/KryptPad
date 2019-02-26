using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.Classes
{
    internal static class Constants
    {
        /// <summary>
        /// Resource name for credential locker
        /// </summary>
#if DEBUG
        public const string LOCKER_RESOURCE = "KryptPadTest";
#else
        public const string LOCKER_RESOURCE = "KryptPad";
#endif
    }
}
