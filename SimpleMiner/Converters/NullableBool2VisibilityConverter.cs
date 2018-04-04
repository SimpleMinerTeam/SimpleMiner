using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SimpleCPUMiner.Converters
{
    public class NullableBool2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bValue = (bool?)value;

            if (bValue == null)
            {
                return Visibility.Hidden;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
