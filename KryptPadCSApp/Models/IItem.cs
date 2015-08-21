using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace KryptPadCSApp.Models
{
    interface IItem
    {
        string Name { get; set; }
        SymbolIcon Symbol { get; set; }
    }
}
