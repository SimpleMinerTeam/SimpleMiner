using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using static SimpleCPUMiner.Consts;

namespace SimpleCPUMiner.Converters
{
    public class Temperature2ImageSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var temp = (int)value;

            if(temp < 55)
                return "/SimpleMiner;component/Resources/iconTempLow.png";
            else if(temp > 54 && temp < 70)
                return "/SimpleMiner;component/Resources/iconTempMed.png";
            else if(temp > 69)
                return "/SimpleMiner;component/Resources/iconTempHigh.png";

            return "/SimpleMiner;component/Resources/iconTempLow.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
