using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCPUMiner.Messages
{
    public class MinerOutputMessage
    {
        public string OutputText { get; set; }
        public bool IsError { get; set; }
    }

    public class CpuAffinityMessage
    {
        public string CpuAffinity { get; set; }
    }
}
