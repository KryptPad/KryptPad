using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using KryptPad.Security;
using System.IO;

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
        public string FileName { get; set; }

        #endregion

        public Document()
        {

        }

        /// <summary>
        /// Saves the current document
        /// </summary>
        public void Save()
        {
            //check if we have a password stored, this is used to encrypt the data
            if (string.IsNullOrWhiteSpace(SessionPassword))
            {
                //must have a password set
                throw new Exception("You cannot save your document without a password.");
            }

            //check for a working filename
            if (string.IsNullOrWhiteSpace(FileName))
            {
                //must have a password set
                throw new Exception("You must specify a file before you can save your document.");
            }
            
            //now that those checks are out of the way, we can serialize the data into a JSON string and encrypt it.
            var jsonData = JsonConvert.SerializeObject(this);

            //encrypt the data
            var encryptedData = Encryption.Encrypt(jsonData, SessionPassword);

            //write the changes to disk
            using (var fs = File.OpenWrite(FileName))
            {

            }

        }
    }
}
