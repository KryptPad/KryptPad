using KryptPadCSApp.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
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
                //update can execute
                UnlockCommand.CommandCanExecute = !string.IsNullOrWhiteSpace(_password);
            }
        }


        /// <summary>
        /// Gets the command to handle unlocking
        /// </summary>
        public Command UnlockCommand { get; protected set; }

        public Command NewDocumentCommand { get; protected set; }

        public Command OpenExistingCommand { get; protected set; }

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
            UnlockCommand = new Command(UnlockCommandHandler, false);

            NewDocumentCommand = new Command(NewDocumentCommandHandler);

            OpenExistingCommand = new Command(OpenExistingCommandHandler);
        }

        #region Command handlers
        private void UnlockCommandHandler(object p)
        {
            //close the dialog
            DialogHelper.CloseDialog(p as FrameworkElement);
        }

        private async void NewDocumentCommandHandler(object p)
        {
            var picker = new FileSavePicker();
            
            //TODO: move to string resource
            picker.DefaultFileExtension = ".kdf";
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add("KryptPad Document Format", new List<string>() { ".kdf" });
            //prompt to save
            var res = await picker.PickSaveFileAsync();

            if (res != null)
            {
                //set the filepath
                (App.Current as App).Document.SelectedFile = res;
                
                //close the dialog
                DialogHelper.CloseDialog(p as FrameworkElement);

                //open the create password dialog
                DialogHelper.CreatePasswordDialog();
            }
        }

        private async void OpenExistingCommandHandler(object p)
        {
            var picker = new FileOpenPicker();

            //add filters for our document type
            picker.FileTypeFilter.Add(".kdf");

            //prompt to save
            var res = await picker.PickSingleFileAsync();

            if (res != null)
            {
                var document = await Document.Load(res, "12345678");
                
                (App.Current as App).Document = document;

                //close the dialog
                DialogHelper.CloseDialog(p as FrameworkElement);
                                
            }
        }


        #endregion
    }
}
