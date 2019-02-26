using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPad.Api.Models
{
    public class BaseModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Event Handlers

        /// <summary>
        /// Raises the PropertyChanged event for a property
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

    }
}
