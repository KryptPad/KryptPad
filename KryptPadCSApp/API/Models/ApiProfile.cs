﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.API.Models
{
    class ApiProfile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool HasPassword { get; set; }
    }
}
