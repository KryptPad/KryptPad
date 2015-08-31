using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace KryptPadCSApp.Models
{
    interface IItem
    {
        Category Category { get; set; }
        string Name { get; set; }
        char Icon { get; set; }
        Brush Background { get; }
    }
}
