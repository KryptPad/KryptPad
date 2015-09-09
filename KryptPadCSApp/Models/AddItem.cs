using KryptPadCSApp.Views;
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
    class AddItem : ItemBase
    {

        #region Properties

        
        #endregion

        public AddItem()
        {
            Name = "Add Item";
            Icon = (char)0xE109;
            //ItemType = Classes.ItemType.AddItem;
        }


    }
}
