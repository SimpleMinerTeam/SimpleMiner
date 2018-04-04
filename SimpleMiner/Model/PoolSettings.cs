using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static SimpleCPUMiner.Consts;
using static SimpleCPUMiner.ViewModel.MainViewModel;

namespace SimpleCPUMiner.Model
{
    [Serializable]
    public class PoolSettingsXml
    {
        [XmlElement]
        public int ID { get; set; }
        [XmlElement]
        public string URL { get; set; } //URL of mining server
        [XmlElement]
        public int Port { get; set; } // Server port
        [XmlElement]
        public string Username { get; set; } //username for mining server
        [XmlElement]
        public string Password { get; set; } //password for mining server
        [XmlElement]
        public bool IsCPUPool { get; set; }
        [XmlElement]
        public bool IsGPUPool { get; set; }
        [XmlElement]
        public bool IsFailOver { get; set; }
        [XmlElement]
        public bool IsMain { get; set; }
        [XmlElement]
        public bool IsRemoveable { get; set; }
        [XmlElement]
        public int FailOverPriority { get; set; }
        [XmlElement]
        public CoinTypes CoinType { get; set; }
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public string Website { get; set; }
        [XmlElement]
        public bool StatsAvailable { get; set; }
        [XmlElement]
        public string StatUrl { get; set; }
    }
}
