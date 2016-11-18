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

        /// <summary>
        /// Gets or sets the background color
        /// </summary>
        public string BackgroundColor { get; set; }


        private SolidColorBrush _brush;
        /// <summary>
        /// Gets or sets the background color of the item tile
        /// </summary>
        [JsonIgnore]
        public SolidColorBrush Background
        {
            get
            {

                // Is there a color set?
                if (string.IsNullOrWhiteSpace(BackgroundColor))
                {
                    BackgroundColor = Colors.LightGray.ToString();
                }

                // Create a brush if one not already exists
                if (_brush == null)
                {
                    _brush = GetSolidColorBrush(BackgroundColor);
                }
                
                // Return the color as a SolidColorBrush
                return _brush;
            }
            set
            {
                BackgroundColor = value.Color.ToString();
                // Update the brush
                _brush = GetSolidColorBrush(BackgroundColor);
            }
        }

        /// <summary>
        /// Gets or set the fields for the item
        /// </summary>
        public ApiField[] Fields { get; set; }

        private SolidColorBrush GetSolidColorBrush(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
            SolidColorBrush myBrush = new SolidColorBrush(Color.FromArgb(a, r, g, b));
            return myBrush;
        }
    }
}
