using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace KryptPadCSApp.Converters
{
    class NotVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // If the value is null, show the element
            if (value == null)
            {
                return Visibility.Visible;
            }

            // Check different object types
            if (value is bool && (bool)value)
            {
                return Visibility.Collapsed;
            }
            else if (value is int && (int)value != 0)
            {
                return Visibility.Collapsed;
            }
            else if (value is string && !string.IsNullOrWhiteSpace((string)value))
            {
                return Visibility.Collapsed;
            }

            // Others are considered visible
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ((Visibility)value) == Visibility.Visible ? false : true;
        }
    }
}
