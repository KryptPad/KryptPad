using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KryptPad.Api.Responses
{
    public abstract class ApiWebExceptionResponse : ApiResponse
    {

        /// <summary>
        /// Converts this WebExceptionResponse to an Exception object
        /// </summary>
        /// <returns></returns>
        public abstract WebException ToException();

    }
}
