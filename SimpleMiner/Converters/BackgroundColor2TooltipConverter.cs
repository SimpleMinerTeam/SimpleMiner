using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SimpleCPUMiner.Converters
{
    public class BackgroundColor2TooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bValue = (Brush)value;

            if (bValue == Consts.PoolListBackgrounds.Purple)    //A proci is és a videókari dolgozik
            {
                return "CPU & GPU working on this pool.";
            }
            else if (bValue == Consts.PoolListBackgrounds.Green) //A videókari dolgozik
            {
                return "Active GPU pool.";
            }
            else if (bValue == Consts.PoolListBackgrounds.Orange) //A proci dolgozik
            {
                return "Active CPU pool.";
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
