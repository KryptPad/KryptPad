using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace KryptPadCSApp.Models
{
    class Profile : ItemBase
    {
        #region Properties

        public FieldCollection Fields { get; protected set; } = new FieldCollection();
        
        #endregion

        public Profile()
        {
            //set background color for profile items
            Background = new SolidColorBrush(Colors.LightBlue);
        }
    }
}
