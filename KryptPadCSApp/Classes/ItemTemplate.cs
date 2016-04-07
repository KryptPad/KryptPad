using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace KryptPadCSApp.Classes
{
    /// <summary>
    /// Represents an item
    /// </summary>
    public class ItemTemplate
    {
        /// <summary>
        /// Gets or sets the name of the template
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of fields to add when an item of this type is created
        /// </summary>
        public ItemTemplateField[] Fields { get; set; }

        /// <summary>
        /// Loads the templates from the datasource
        /// </summary>
        /// <returns></returns>
        public static async Task<ItemTemplate[]> LoadTemplatesAsync()
        {
            var filename = $"ms-appx:///Assets/templates.json";
            // Build URI
            Uri appUri = new Uri(filename);//File name should be prefixed with 'ms-appx:///Assets/* 
            // Get the file
            StorageFile templateFile = await StorageFile.GetFileFromApplicationUriAsync(appUri);

            // Read the JSON string
            string jsonText = await FileIO.ReadTextAsync(templateFile);

            // Deserialize the JSON string
            return JsonConvert.DeserializeObject<ItemTemplate[]>(jsonText);
        }

    }
}
