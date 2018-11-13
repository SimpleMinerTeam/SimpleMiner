using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static SimpleCPUMiner.Consts;

namespace SimpleCPUMiner.Model
{
    [Serializable]
    public class Coin
    {
        [XmlElement]
        public string ShortName { get; set; }
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public int? Algorithm { get; set; }
        [XmlElement]
        public string Icon { get; set; }
        [XmlElement]
        public string Webpage { get; set; }
        [XmlElement]
        public CoinTypes CoinType { get; set; }
    }
}
