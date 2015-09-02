using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KryptPadCSApp.Models
{
    class LoginUserControlViewModel : BaseModel
    {

        #region Properties
        /// <summary>
        /// Gets a list of recently accessed documents
        /// </summary>
        public ObservableCollection<string> RecentDocuments { get; protected set; } = new ObservableCollection<string>();

        private Visibility _promptToUnlock = Visibility.Collapsed;
        /// <summary>
        /// Gets or sets whether to prompt the user to unlock their last document
        /// </summary>
        public Visibility PromptToUnlock
        {
            get { return _promptToUnlock; }
            set
            {
                _promptToUnlock = value;
                //notify change
                OnPropertyChanged(nameof(PromptToUnlock));
            }
        }


        #endregion

        public LoginUserControlViewModel()
        {
            //listent to changes to the list
            RecentDocuments.CollectionChanged += (sender, e) =>
            {
                //notify that the property is changing
                PromptToUnlock = RecentDocuments.Any() ? Visibility.Visible : Visibility.Collapsed;
            };

            //simulate getting recent documents
            RecentDocuments.Add("my passwords.kdf");
        }
    }
}
