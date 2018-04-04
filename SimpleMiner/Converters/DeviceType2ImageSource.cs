using Cloo;
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
    public class DeviceType2ImageSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var swCT = (ComputeDeviceTypes)value;
            switch (swCT)
            {
                case ComputeDeviceTypes.Gpu:
                    return "/SimpleMiner;component/Resources/iconGPU.png";
                    break;
                case ComputeDeviceTypes.Cpu:
                default:
                    return "/SimpleMiner;component/Resources/iconCPU.png";
                    break;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
