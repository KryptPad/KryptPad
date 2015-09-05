using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using KryptPad.Security;
using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;

namespace KryptPadCSApp.Models
{
    /// <summary>
    /// Represents the working document
    /// </summary>
    class Document : BaseModel
    {

        #region Fields
        //private bool _isLoading =;
        #endregion

        #region Properties

        /// <summary>
        /// Gets the current document version
        /// </summary>
        public int Version { get; } = 0x00000001;

        /// <summary>
        /// Gets the collection of categories
        /// </summary>
        public CategoryCollection Categories { get; private set; } = new CategoryCollection();

        /// <summary>
        /// This is the user's password. This is used to encrypt and decrypt data on the fly. NOT saved with the document.
        /// </summary>
        [JsonIgnore]
        public string SessionPassword { get; set; }

        /// <summary>
        /// Gets or sets the current working file
        /// </summary>
        [JsonIgnore]
        public StorageFile SelectedFile { get; set; }

        #endregion

        public Document() { }

        public Document(bool initialize)
        {
            //if the initialize flag is passed as true, wire up change listeners
            if (initialize)
            {
                InitializeDocument();
            }
        }

        /// <summary>
        /// Wires up change event listeners to auto save the document
        /// </summary>
        private void InitializeDocument()
        {
            //listen to category changes
            Categories.CollectionChanged += (sender, e) =>
            {
                //whenever a change occurs, like a new category is added, or
                //a category is deleted, save the document
                Save();

                //each item added will have change tracking on its Name property
                foreach (Category category in e.NewItems)
                {

                    //event handler for collection changed
                    category.Items.CollectionChanged += Items_CollectionChanged;

                    //handle property changes for category
                    category.PropertyChanged += Item_PropertyChanged;

                }
            };

            InitializeItemCollectionChangeTracking();

        }

        private void InitializeItemCollectionChangeTracking()
        {
            //each item added will have change tracking on its Name property
            foreach (Category category in Categories)
            {

                //event handler for collection changed
                category.Items.CollectionChanged += Items_CollectionChanged;

                //handle property changes for category
                category.PropertyChanged += Item_PropertyChanged;

            }
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //when an item is created or deleted
            Save();

            //for each item in the category, listen to changes
            foreach (ItemBase item in e.NewItems)
            {
                //handle property changes for item
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //when a property is changed, call save
            Save();
        }

        //private void Category_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    //when a property is changed, call save
        //    Save();
        //}

        /// <summary>
        /// Saves the current document
        /// </summary>
        public async void Save()
        {
            //check if we have a password stored, this is used to encrypt the data
            if (string.IsNullOrWhiteSpace(SessionPassword))
            {
                //must have a password set
                throw new Exception("You cannot save your document without a password.");
            }

            //check for a working filename
            if (SelectedFile == null)
            {
                //must have a password set
                throw new Exception("You must specify a file before you can save your document.");
            }

            //now that those checks are out of the way, we can serialize the data into a JSON string and encrypt it.
            var jsonData = JsonConvert.SerializeObject(this);

            //encrypt the data
            var encryptedData = Encryption.Encrypt(jsonData, SessionPassword);

            try
            {
                //write the changes to disk
                await FileIO.WriteBytesAsync(SelectedFile, encryptedData);
            }
            catch (Exception)
            {

                var msg = new MessageDialog("An error occurred while trying to save your document. Your changes have not been saved.", "Error");

                await msg.ShowAsync();
            }


        }

        /// <summary>
        /// Gi
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static async Task<Document> Load(StorageFile selectedFile, string password)
        {
            //check if we have a password stored, this is used to encrypt the data
            if (string.IsNullOrWhiteSpace(password))
            {
                //must have a password set
                throw new Exception("Password cannot be null.");
            }

            //check for a working filename
            if (selectedFile == null)
            {
                //must have a password set
                throw new Exception("File name cannot be null.");
            }

            //read the document from the disk
            var buffer = await FileIO.ReadBufferAsync(selectedFile);
            byte[] encryptedData = new byte[buffer.Length];

            using (var dr = DataReader.FromBuffer(buffer))
            {
                //read the bytes from the buffer
                dr.ReadBytes(encryptedData);
            }

            //try to decrypt the data
            try
            {
                //attempt to decrypt. failure here is likely a password error
                var jsonData = Encryption.Decrypt(encryptedData, password);

                //deserialize
                var document = JsonConvert.DeserializeObject<Document>(jsonData);

                //now that the document is loaded, we can wire up change listeners
                document.InitializeDocument();
                //set session password
                document.SessionPassword = password;
                //set current file
                document.SelectedFile = selectedFile;

                //return the document
                return document;
            }
            catch (Exception ex)
            {
                //throw a new exception
                throw new Exception("Could not decrypt your document. Make sure you have entered the correct password.", ex);
            }



        }



    }
}
