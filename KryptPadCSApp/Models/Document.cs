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
        public StorageFile SelectedFile { get; set; }

        #endregion

        public Document()
        {
            //listen to category changes
            Categories.CollectionChanged += (sender, e) => {
                //whenever a change occurs, like a new category is added, or
                //a category is deleted, save the document
                Save();

                //each item added will have change tracking on its Name property
                foreach( Category item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            };
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //when a property is changed, call save
            Save();
        }

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
        public static Document Load(string filePath, string password)
        {
            //check if we have a password stored, this is used to encrypt the data
            if (string.IsNullOrWhiteSpace(password))
            {
                //must have a password set
                throw new Exception("Password cannot be null.");
            }

            //check for a working filename
            if (string.IsNullOrWhiteSpace(filePath))
            {
                //must have a password set
                throw new Exception("File name cannot be null.");
            }

            //holds the encrypted data
            string encryptedData;

            //write the document from the disk
            using (var fs = File.OpenRead(filePath))
            using (var sr = new StreamReader(fs))
            {
                //read the encrypted data
                encryptedData = sr.ReadToEnd();
            }

            //try to decrypt the data
            try
            {
                //attempt to decrypt. failure here is likely a password error
                var jsonData = Encryption.Decrypt(encryptedData, password);

                //deserialize
                var document = JsonConvert.DeserializeObject<Document>(jsonData);

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
