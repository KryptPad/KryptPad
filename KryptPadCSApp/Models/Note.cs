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
    class Note : ItemBase
    {
        #region Properties

        private string _notes;
        /// <summary>
        /// Gets or sets the notes for this item
        /// </summary>
        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                //notify change
                OnPropertyChanged(nameof(Notes));
            }
        }


        #endregion

        public Note()
        {
            Background = new SolidColorBrush(Colors.Lavender);
            //ItemType = Classes.ItemType.Note;
        }
    }
}
