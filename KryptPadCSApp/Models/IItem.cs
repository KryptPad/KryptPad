using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace KryptPadCSApp.Models
{
    interface IItem
    {
        string Name { get; set; }
        char Icon { get; set; }

    }
}
