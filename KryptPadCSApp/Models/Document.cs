using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.Storage.Pickers;
using KryptPadCSApp.Classes;

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

        #region Initialize change tracking

        /// <summary>
        /// Wires up change event listeners to auto save the document
        /// </summary>
        private void InitializeDocument()
        {

            InitializeCategoryCollectionChangeTracking();

            ////for existing categories, listen to the items collection and properties
            //foreach (var category in Categories)
            //{
            //    InitializeItemCollectionChangeTracking(category);

            //    //listen to items
            //    foreach (var item in category.Items)
            //    {
            //        InitializeFieldCollectionChangeTracking(item);

            //    }
            //}

        }

        /// <summary>
        /// Listen to changed to the categories collection. When something is added or removed, call OnSave()
        /// </summary>
        private void InitializeCategoryCollectionChangeTracking()
        {
            //list to the categories collection for new items
            Categories.CollectionChanged += Categories_CollectionChanged;

        }

        /// <summary>
        /// Listen to the Items collection for a category. Also listen to category properties
        /// </summary>
        private void InitializeItemCollectionChangeTracking(Category category)
        {
            //event handler for collection changed
            category.Items.CollectionChanged += Items_CollectionChanged;

            //handle property changes for category
            category.PropertyChanged += Item_PropertyChanged;

        }

        /// <summary>
        /// Listent to the Fields collection for changes
        /// </summary>
        /// <param name="item"></param>
        private void InitializeFieldCollectionChangeTracking(ItemBase item)
        {

            //event handler for collection changed
            if (item is Profile)
            {
                (item as Profile).Fields.CollectionChanged += Fields_CollectionChanged;
            }
            
            //handle property changes for item
            item.PropertyChanged += Item_PropertyChanged;

        }

        private void Categories_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //whenever a change occurs, like a new category is added, or
            //a category is deleted, save the document
            Save();

            //for each new category, listen to the items collection
            foreach (Category category in e.NewItems)
            {
                InitializeItemCollectionChangeTracking(category);
            }
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //when an item is created or deleted
            Save();

            //for each item in the category, listen to changes
            foreach (ItemBase item in e.NewItems)
            {
                InitializeFieldCollectionChangeTracking(item);
            }
        }

        private void Fields_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //when a field is created or deleted
            Save();

            //listen to changes in the fields
            foreach (Field field in e.NewItems)
            {
                //handle property changes for category
                field.PropertyChanged += Item_PropertyChanged;
            }


        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //when a property is changed, call save
            Save();
        }

        #endregion
        

        /// <summary>
        /// Saves the current document
        /// </summary>
        private async void Save()
        {
            ////create settings
            //var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };
            ////now that those checks are out of the way, we can serialize the data into a JSON string and encrypt it.
            //var jsonData = JsonConvert.SerializeObject(this, settings);

            ////encrypt the data
            ////var encryptedData = Encryption.Encrypt(jsonData, SessionPassword);

            //try
            //{
               
            //    //write the changes to disk
            //    await FileIO.WriteBytesAsync(SelectedFile, encryptedData);
            //}
            //catch (Exception)
            //{

            //    var msg = new MessageDialog("An error occurred while trying to save your document. Your changes have not been saved.", "Error");

            //    await msg.ShowAsync();
            //}


        }

        /// <summary>
        /// Loads the document
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
                ////attempt to decrypt. failure here is likely a password error
                //var jsonData = Encryption.Decrypt(encryptedData, password);

                ////create settings
                //var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };
                ////deserialize
                //var document = JsonConvert.DeserializeObject<Document>(jsonData, settings);

                ////now that the document is loaded, we can wire up change listeners
                //document.InitializeDocument();
                ////set session password
                //document.SessionPassword = password;
                ////set current file
                //document.SelectedFile = selectedFile;

                //return the document
                return null;
            }
            catch (Exception ex)
            {
                //throw a new exception
                throw new Exception("Could not decrypt your document. Make sure you have entered the correct password.", ex);
            }



        }



    }
}
