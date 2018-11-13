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
                case CoinTypes.ETN:
                    return "/SimpleMiner;component/Resources/coinElectroneum.png";
                case CoinTypes.RYO:
                    return "/SimpleMiner;component/Resources/coinRyo.png";
                case CoinTypes.XMR:
                    return "/SimpleMiner;component/Resources/coinMonero.png";
                case CoinTypes.TUBE:
                    return "/SimpleMiner;component/Resources/coinTube.png";
                case CoinTypes.KRB:
                    return "/SimpleMiner;component/Resources/coinKrb.png";
                case CoinTypes.GRFT:
                    return "/SimpleMiner;component/Resources/coinGraft.png";
                case CoinTypes.XTL:
                    return "/SimpleMiner;component/Resources/coinStellite.png";
                case CoinTypes.TRTL:
                    return "/SimpleMiner;component/Resources/coinTurtle.png";
                case CoinTypes.NiceHash:
                    return "/SimpleMiner;component/Resources/coinNiceHash.png";
                case CoinTypes.XHV:
                    return "/SimpleMiner;component/Resources/coinHaven.png";
                case CoinTypes.LOKI:
                    return "/SimpleMiner;component/Resources/coinLoki.png";
                case CoinTypes.SUMO:
                    return "/SimpleMiner;component/Resources/coinSumokoin.png";
                case CoinTypes.XAO:
                    return "/SimpleMiner;component/Resources/coinAlloy.png";
                default:
                    return "/SimpleMiner;component/Resources/coinOther.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
