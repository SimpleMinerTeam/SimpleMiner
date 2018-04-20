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
    public class PoolSettingsXml : INotifyPropertyChanged
    {
        [XmlElement]
        public int ID { get; set; }
        [XmlElement]
        public string URL  //URL of mining server
        {
            get { return _url; }
            set
            {
                if(value.Contains(":"))
                {
                    var st = value.Split(new char[] { ':' },  StringSplitOptions.RemoveEmptyEntries);
                    if(st.Length==2)
                    {
                        int port = 0;
                        if (int.TryParse(st[1], out port))
                        {
                            Port = port;
                            _url = st[0];
                            NotifyPropertyChanged(nameof(Port));
                        }
                        else
                            _url = value;
                    }
                    else
                        _url = value;
                }
                else
                {
                    _url = value;
                }
            }
        }
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
        [XmlElement]
        public Consts.Algorithm? Algorithm { get; set; }

        private string _url;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
