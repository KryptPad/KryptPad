using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace KryptPadCSApp.Models
{
    class MenuItem :BaseModel
    {
        #region Properties
        /// <summary>
        /// Gets or sets the symbol for the menu action item
        /// </summary>
        public Symbol Symbol { get; set; }

        /// <summary>
        /// Gets or sets the menu item text
        /// </summary>
        public string Text { get; set; }
        #endregion
    }
}
