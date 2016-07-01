using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPad.Api.Responses
{
    public class SuccessResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets the Id of the resource affected by the operation
        /// </summary>
        public int Id { get; set; }

        public SuccessResponse() { }
        public SuccessResponse(int id)
        {
            Id = id;
        }
    }
}
