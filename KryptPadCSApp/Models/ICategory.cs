using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace KryptPadCSApp.Models
{
    interface ICategory
    {
        string Name { get; set; }
        SymbolIcon Symbol { get; set; }
        ObservableCollection<IItem> Items { get; }
    }
}
