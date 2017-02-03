using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace KryptPadCSApp.Converters
{
    class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // If the value is null, collapse the element
            if (value == null)
            {
                return Visibility.Collapsed;
            }

            // Check different object types
            if (value is bool && (bool)value)
            {
                return Visibility.Visible;
            }
            else if (value is int && (int)value != 0)
            {
                return Visibility.Visible;
            }
            else if (value is string && !string.IsNullOrWhiteSpace((string)value))
            {
                return Visibility.Visible;
            }

            // Others are considered not visible
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ((Visibility)value) == Visibility.Visible ? true : false;
        }
    }
}
