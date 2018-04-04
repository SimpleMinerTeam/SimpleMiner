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
    public class CoinType2ImageSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var swCT = (CoinTypes)value;
            switch (swCT)
            {
                case CoinTypes.BCN:
                    return "/SimpleMiner;component/Resources/coinBytecoin.png";
                    break;
                case CoinTypes.ETN:
                    return "/SimpleMiner;component/Resources/coinElectroneum.png";
                    break;
                case CoinTypes.SUMO:
                    return "/SimpleMiner;component/Resources/coinSumokoin.png";
                    break;
                case CoinTypes.XMR:
                    return "/SimpleMiner;component/Resources/coinMonero.png";
                    break;
                case CoinTypes.KRB:
                    return "/SimpleMiner;component/Resources/coinKrb.png";
                    break;
                case CoinTypes.TRTL:
                    return "/SimpleMiner;component/Resources/coinTurtle.png";
                    break;
                case CoinTypes.NiceHash:
                    return "/SimpleMiner;component/Resources/coinNiceHash.png";
                    break;
                default:
                    return "/SimpleMiner;component/Resources/coinOther.png";
                    break;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
