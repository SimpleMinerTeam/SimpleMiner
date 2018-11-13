using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace SimpleCPUMiner.Converters
{
    public class SupportedDeviceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) { return null;}

            bool result = false;
            var algo = (Model.Algo)value;

            if ((parameter as string).Equals("CPU",StringComparison.InvariantCultureIgnoreCase))
            {
                result = Consts.Algorithms.Where(x => x.ID == algo.ID).FirstOrDefault().IsCpuSupport;
            }
            else if ((parameter as string).Equals("GPU", StringComparison.InvariantCultureIgnoreCase))
            {
                result = Consts.Algorithms.Where(x => x.ID == algo.ID).FirstOrDefault().IsGpuSupport;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
