using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.Models
{
    class BasePageModel : BaseModel
    {

        #region Properties
        private bool _isBusy;

        /// <summary>
        /// Gets or sets that the page is busy doing something
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                //notify change
                OnPropertyChanged(nameof(IsBusy));
                
            }
        }
        #endregion

    }
}
