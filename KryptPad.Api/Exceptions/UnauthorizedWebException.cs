using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KryptPad.Api.Exceptions
{
    class UnauthorizedWebException : WebException
    {
        public UnauthorizedWebException() : base ("Your request is not authorized")
        {
           
        }
    }
}
