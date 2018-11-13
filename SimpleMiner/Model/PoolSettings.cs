using System;
using System.ComponentModel;
using System.Xml.Serialization;
using static SimpleCPUMiner.Consts;

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
                if(value != null && value.Contains(":"))
                {
                    var st = value.Split(new char[] { ':' },  StringSplitOptions.RemoveEmptyEntries);
                    if(st.Length==2)
                    {
                        if (int.TryParse(st[1], out int port))
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
        public bool IsCPUPool
        {
            get
            {
                return _isCPUPool;
            }
            set
            {
                _isCPUPool = value;
                NotifyPropertyChanged(nameof(IsCPUPool));
            }
        }
        [XmlElement]
        public bool IsGPUPool
        {
            get
            {
                return _isGPUPool;
            }
            set
            {
                _isGPUPool = value;
                NotifyPropertyChanged(nameof(IsGPUPool));
            }
        }
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
        public string Algorithm { get; set; }

        private string _url;
        private bool _isCPUPool;
        private bool _isGPUPool;

        public event PropertyChangedEventHandler PropertyChanged;
        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
