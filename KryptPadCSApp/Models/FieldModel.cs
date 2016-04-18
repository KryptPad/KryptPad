using KryptPadCSApp.API;
using KryptPadCSApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KryptPadCSApp.Models
{
    class FieldModel : BaseModel
    {

        #region Properties
        private ApiField _field;

        /// <summary>
        /// Gets the underlying field that backs this model
        /// </summary>
        public ApiField Field
        {
            get { return _field; }
        }

        /// <summary>
        /// Gets or sets the ID of the field
        /// </summary>
        public int Id
        {
            get { return _field.Id; }
            set { _field.Id = value; }
        }

        /// <summary>
        /// Gets or sets the name of the field. e.g Password
        /// </summary>
        public string Name
        {
            get { return _field.Name; }
            set { _field.Name = value; }
        }

        /// <summary>
        /// Gets the field type
        /// </summary>
        public FieldType FieldType
        {
            get { return _field.FieldType; }
        }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public string Value
        {
            get { return _field.Value; }
            set
            {
                _field.Value = value;
                // Notify change
                OnPropertyChanged(nameof(Value));
            }
        }

        
        #endregion
        
        #region Ctor
        public FieldModel(ApiField field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            _field = field;

            RegisterCommands();
        }
        #endregion

        #region Commands
        private void RegisterCommands()
        {
            
        }
        #endregion

        
    }
}
