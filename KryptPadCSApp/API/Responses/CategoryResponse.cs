﻿using KryptPadCSApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.API.Responses
{
    class CategoryResponse : ApiResponse
    {
        public ApiCategory[] Categories { get; set; }
    }
}
