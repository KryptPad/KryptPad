using KryptPadCSApp.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        event PropertyChangedEventHandler PropertyChanged;

        [JsonIgnore]
        Category Category { get; set; }
        //ItemType ItemType { get; }
        string Name { get; set; }
        char Icon { get; set; }
        [JsonIgnore]
        Brush Background { get; }
    }
}
