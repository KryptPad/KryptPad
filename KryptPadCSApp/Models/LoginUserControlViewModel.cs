using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                //notify change
                OnPropertyChanged(nameof(PromptToUnlock));
            }
        }


        /// <summary>
        /// Gets the command to handle unlocking
        /// </summary>
        public Command UnlockCommand { get; protected set; }
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

            //register commands
            RegisterCommands();
        }

        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            UnlockCommand = new Command((p) =>
            {

                //p is the parent framework element
                var page = p as FrameworkElement;

                if (page != null)
                {
                    //the parent is the host dialog
                    var dialog = page.Parent as ContentDialog;
                    var f = Password;
                    if (dialog != null)
                    {
                        dialog.Hide();
                    }
                }



            });
        }
    }
}
