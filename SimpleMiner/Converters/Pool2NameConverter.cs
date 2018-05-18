using SimpleCPUMiner.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SimpleCPUMiner.Converters
{
    public class Pool2NameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pool = value as PoolSettingsXml;

            return string.IsNullOrEmpty(pool.Name) ? pool.URL : pool.Name;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
