using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCPUMiner.Model
{
    public class SimpleDevice
    {
        public String Name { get; set; }
        public int ID { get; set; }
        public double Speed { get; set; }
        public int Worksize { get; set; }
        public int Intensity { get; set; }
        public int Threads { get; set; }
        public string Algo { get; set; }
        public int Fan { get; set; }
        public int Activity { get; set; }
        public int Temp { get; set; }
        public string Shares { get; set; }
    }
}
