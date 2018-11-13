using System;

namespace SimpleCPUMiner.Messages
{
    public class MinerOutputMessage
    {
        public string OutputText { get; set; }
        public bool IsError { get; set; }
        public Exception Exception { get; set; }
    }

    public class CpuAffinityMessage
    {
        public string CpuAffinity { get; set; }
    }

    public class StopMinerThreadsMessage
    {
    }

    public class ShowHideMessage
    {
        public bool IsShow { get; set; }
    }

    public class ActivePoolMessage
    {
        public bool IsActiveCPUPool { get; set; }
        public bool IsActiveGPUPool { get; set; }
        public string URL { get; set; }
        public int Port { get; set; }
    }
}
