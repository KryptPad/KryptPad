using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KryptPad.Api.Responses
{
    public class WebExceptionResponse : ApiWebExceptionResponse
    {
        public string Message { get; set; }
        public IDictionary<string, string[]> ModelState { get; set; }
        public string[] Errors { get; set; }

        /// <summary>
        /// Converts this WebExceptionResponse to an Exception object
        /// </summary>
        /// <returns></returns>
        public override WebException ToException()
        {
            var modelErrors = new List<string>();
            var msg = Message;

            // Check for model state errors
            if (ModelState != null)
            {
                // Build string of model state errors
                foreach(var ms in ModelState)
                {
                    // Each model state error can have multiple errors
                    foreach (var error in ms.Value)
                    {
                        modelErrors.Add(error);
                    }
                }

                // Set errors to msg
                msg = string.Join("\n", modelErrors);

            }else if(Errors != null)
            {
                // Handle errors that come in as an array under "Errors"
                // Each model state error can have multiple errors
                foreach (var error in Errors)
                {
                    modelErrors.Add(error);
                }

                // Set errors to msg
                msg = string.Join("\n", modelErrors);
            }

            return new WebException(msg);
        }
    }
}
