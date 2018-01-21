using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SimpleCPUMiner.Consts;
using static SimpleCPUMiner.ViewModel.MainViewModel;

namespace SimpleCPUMiner.Model
{
    [Serializable]
    public class PoolSettings
    {
        public int ID { get; set; }
        public string URL { get; set; } //URL of mining server
        public int Port { get; set; } // Server port
        public string Username { get; set; } //username for mining server
        public string Password { get; set; } //password for mining server
        public bool IsCPUPool { get; set; }
        public bool IsGPUPool { get; set; }
        public bool IsFailOver { get; set; }
        public bool IsMain { get; set; }
        public bool IsRemoveable { get; set; }
        public int FailOverPriority { get; set; }
        public CoinTypes CoinType { get; set; }
    }
}
