using System;

namespace SimpleCPUMiner
{
    [Serializable]
    public class MinerSettings
    {
        public int ID { get; set; }
        public string Algo { get; set; } //cryptonight (default) or cryptonight-lite
        public string URL { get; set; } //URL of mining server
        public int Port { get; set; } // Server port
        public string Username { get; set; } //username for mining server
        public string Password { get; set; } //password for mining server
        public string NumberOfThreads { get; set; } //number of miner threads
        public string AlgorithmVariation { get; set; } //algorithm variation, 0 auto select
        public bool IsKeepalive { get; set; } //send keepalived for prevent timeout (need pool support)
        public int NumOfRetries { get; set; } //number of times to retry before switch to backup server (default: 5)
        public int RetryPause { get; set; } //number of times to retry before switch to backup server (default: 5)
        public string CPUAffinity { get; set; } //set process affinity to CPU core(s), mask 0x3 for cores 0 and 1
        public bool NoHugePages { get; set; } //disable huge pages support
        public bool NoColor { get; set; } //disable colored output
        public int DonateLevel { get; set; } //donate level, default 5% (5 minutes in 100 minutes)
        public bool UserAgent { get; set; } //set custom user-agent string for pool
        public bool? IsBackgroundMining { get; set; } //run the miner in the background
        public string JSONConfigfile { get; set; } //load a JSON-format configuration file
        public bool? IsLogging { get; set; } //log all output to a file
        public int MaxCPUUsage { get; set; } //maximum CPU usage for automatic threads mode (default 75)
        public bool Safe { get; set; } //safe adjust threads and av settings for current CPU
        public bool? IsNicehashSupport { get; set; } //enable nicehash support
        public int PrintHashRate { get; set; } //print hashrate report every N seconds

        public bool IsLaunchOnWindowsStartup { get; set; } // A miner induljon-e a Windowssal együtt
        public bool IsAutostartMining { get; set; } //Elindulás után a program elkezdjen-e bányászni
        public int StartingDelayInSec { get; set; } //Ha az IsAutostartMining értéke true, akkor meg lehet adni hány mp után kezdjen el bányászni a program
        public bool IsMinimizeToTray { get; set; } //Ha az IsAutostartMining értéke true, akkor megadható, hogy a program minimize módban induljon


        public MinerSettings(){}
    }
}
