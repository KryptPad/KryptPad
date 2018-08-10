using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.Exceptions
{
    class WarningException : Exception
    {
        public WarningException(string message) : base(message)
        {

        }
    }
}
