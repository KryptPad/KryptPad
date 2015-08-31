using KryptPad.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.Models
{
    class Field : BaseModel
    {
        #region Properties
        private string _name;
        /// <summary>
        /// Gets or sets the name of the field
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                //notify change
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _value;
        /// <summary>
        /// Gets or sets the value of the field. Encrypts any value set to it.
        /// </summary>
        public string Value
        {
            get { return Encryption.Decrypt(_value, "MyT3stP@$$w0rd!"); ; }
            set
            {
                //encrypt the value
                _value = Encryption.Encrypt(value, "MyT3stP@$$w0rd!");

                //set the encrypted value
                EncryptedValue = _value;

                //notify change
                OnPropertyChanged(nameof(Value));
            }
        }

        private string _encryptedValue;
        /// <summary>
        /// Gets or sets the value of the field. Assumes data being set to it is already encrypted.
        /// </summary>
        public string EncryptedValue
        {
            get { return _encryptedValue; }
            set
            {
                _encryptedValue = value;
                //notify change
                OnPropertyChanged(nameof(EncryptedValue));
            }
        }

        #endregion
    }
}
