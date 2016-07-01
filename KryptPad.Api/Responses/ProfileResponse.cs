using KryptPad.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPad.Api.Responses
{
    public class ProfileResponse : ApiResponse
    {
        public ApiProfile[] Profiles { get; set; }
    }
}
