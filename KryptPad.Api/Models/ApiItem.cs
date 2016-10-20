using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace KryptPad.Api.Models
{
    public class ApiItem
    {
        /// <summary>
        /// Gets or sets the ID of the item
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the notes of the item
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the ID of the category
        /// </summary>
        public int CategoryId { get; set; }

        private string _background;

        /// <summary>
        /// Gets or sets the background color of the item tile
        /// </summary>
        [JsonIgnore]
        public SolidColorBrush Background
        {
            get { return new SolidColorBrush(Colors.LightGray); }
            set { _background = value.Color.ToString(); }
        }

        /// <summary>
        /// Gets or set the fields for the item
        /// </summary>
        public ApiField[] Fields { get; set; }
    }
}
