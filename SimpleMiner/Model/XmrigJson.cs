using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCPUMiner.Model
{
    public class XmrigJson
    {
        public string id { get; set; }
        public string worker_id { get; set; }
        public string version { get; set; }
        public string kind { get; set; }
        public string ua { get; set; }
        public Cpu cpu { get; set; }
        public string algo { get; set; }
        public bool hugepages { get; set; }
        public int donate_level { get; set; }
        public Hashrate hashrate { get; set; }
        public Results results { get; set; }
        public Connection connection { get; set; }

        public class Cpu
        {
            public string brand { get; set; }
            public bool aes { get; set; }
            public bool x64 { get; set; }
            public int sockets { get; set; }
        }

        public class Hashrate
        {
            public List<double> total { get; set; }
            public double highest { get; set; }
            public List<List<double>> threads { get; set; }
        }

        public class Results
        {
            public int diff_current { get; set; }
            public int shares_good { get; set; }
            public int shares_total { get; set; }
            public int avg_time { get; set; }
            public int hashes_total { get; set; }
            public List<int> best { get; set; }
            public List<object> error_log { get; set; }
        }

        public class Connection
        {
            public string pool { get; set; }
            public int uptime { get; set; }
            public int ping { get; set; }
            public int failures { get; set; }
            public List<object> error_log { get; set; }
        }
    }
}
