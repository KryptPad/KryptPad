using KryptPadCSApp.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
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
        public ObservableCollection<StorageFile> RecentDocuments { get; protected set; } = new ObservableCollection<StorageFile>();

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

        private StorageFile _selectedDocument;
        /// <summary>
        /// Gets or sets the selected document
        /// </summary>
        public StorageFile SelectedDocument
        {
            get { return _selectedDocument; }
            set
            {
                _selectedDocument = value;
                //notify change
                OnPropertyChanged(nameof(SelectedDocument));
                //open from recent list
                OpenFromRecent(_selectedDocument);
            }
        }

        private StorageFile _mostRecentFile;

        public StorageFile MostRecentFile
        {
            get { return _mostRecentFile; }
            set
            {
                _mostRecentFile = value;
                //notify change
                OnPropertyChanged(nameof(SelectedDocument));
                //change visibility of unlock
                PromptToUnlock = _mostRecentFile != null ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        /// <summary>
        /// Gets the command to handle unlocking
        /// </summary>
        public Command UnlockCommand { get; protected set; }

        public Command NewDocumentCommand { get; protected set; }

        public Command OpenExistingCommand { get; protected set; }

        public Command OpenFromRecentCommand { get; protected set; }

        #endregion

        public LoginUserControlViewModel()
        {
            //logout
            (App.Current as App).Document = null;// = new Document(true);

            //load the recently used list
            PrepareMostRecentlyUsedList();

            //register commands
            RegisterCommands();
        }

        /// <summary>
        /// Prepares the RecentDocuments collection with items from the MRU
        /// </summary>
        private async void PrepareMostRecentlyUsedList()
        {
            //get the most recently used list
            var list = StorageApplicationPermissions.MostRecentlyUsedList;

            foreach (var entry in list.Entries.OrderByDescending((e) => DateTime.Parse(e.Metadata)))
            {
                if (list.ContainsItem(entry.Token)) {
                    try
                    {
                        //get the file info
                        var file = await list.GetFileAsync(entry.Token);
                        //do we have access to the file in the list?
                        if (list.CheckAccess(file))
                        {
                            //add to the recent documents list
                            RecentDocuments.Add(file);
                        }
                        
                    }
                    catch (Exception)
                    {
                        //remove this item
                        list.Remove(entry.Token);
                    }
                    
                }
            }

            //get the most recent and set it
            MostRecentFile = RecentDocuments.FirstOrDefault();

        }

        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            UnlockCommand = new Command(UnlockCommandHandler, false);

            NewDocumentCommand = new Command(NewDocumentCommandHandler);

            OpenExistingCommand = new Command(OpenExistingCommandHandler);

            //OpenFromRecentCommand = new Command(OpenFromRecentCommandHandler);
        }

        private void OpenFromRecent(StorageFile selectedFile)
        {
            //store the file we want to open temorarily while we wait for a password
            //from the user
            (App.Current as App).SelectedFile = selectedFile;

            //update the settings for our recently accessed files
            (App.Current as App).PushRecentFile(selectedFile);

            //close the dialog
            DialogHelper.CloseDialog();

            //load the authentication dialog
            DialogHelper.AuthenticateDialog();
        }

        #region Command handlers
        private async void UnlockCommandHandler(object p)
        {

            try
            {
                //load the most recent file
                (App.Current as App).Document = await Document.Load(MostRecentFile, Password);

                //close the dialog
                DialogHelper.CloseDialog();
            }
            catch (Exception ex)
            {
                //alert the user that something wen't wrong here
                var msgBox = new MessageDialog(ex.Message);
                //show
                await msgBox.ShowAsync();
            }


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
                //create new document
                (App.Current as App).Document = new Document(true);

                //set the filepath
                (App.Current as App).Document.SelectedFile = res;

                //update the settings for our recently accessed files
                (App.Current as App).PushRecentFile(res);

                //close the dialog
                DialogHelper.CloseDialog();

                //open the create password dialog
                DialogHelper.CreatePasswordDialog();
            }
        }

        private async void OpenExistingCommandHandler(object p)
        {
            var picker = new FileOpenPicker();

            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            //add filters for our document type
            picker.FileTypeFilter.Add(".kdf");

            //prompt to save
            var res = await picker.PickSingleFileAsync();

            if (res != null)
            {
                //store the file we want to open temorarily while we wait for a password
                //from the user
                (App.Current as App).SelectedFile = res;

                //update the settings for our recently accessed files
                (App.Current as App).PushRecentFile(res);

                //close the dialog
                DialogHelper.CloseDialog();

                //load the authentication dialog
                DialogHelper.AuthenticateDialog();

            }
        }



        #endregion
    }
}
