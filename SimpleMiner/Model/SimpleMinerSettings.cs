namespace SimpleCPUMiner.Model
{
    public class SimpleMinerSettings
    {
        public string ApplicationMode { get; set; } //alkalmazás futtatási módja Normal, Silent
        public string NumberOfThreads { get; set; } //number of miner threads
        public string CPUAffinity { get; set; } //set process affinity to CPU core(s), mask 0x3 for cores 0 and 1
        public bool IsLogging { get; set; } //log all output to a file
        public int MaxCPUUsage { get; set; } //maximum CPU usage for automatic threads mode (default 75)
        public bool IsLaunchOnWindowsStartup { get; set; } // A miner induljon-e a Windowssal együtt
        public bool IsAutostartMining { get; set; } //Elindulás után a program elkezdjen-e bányászni
        public int StartingDelayInSec { get; set; } //Ha az IsAutostartMining értéke true, akkor meg lehet adni hány mp után kezdjen el bányászni a program
        public bool IsMinimizeToTray { get; set; } //Ha az IsAutostartMining értéke true, akkor megadható, hogy a program minimize módban induljon
        public bool IsCPUMiningEnabled { get; set; } //CPU bányászás engedélyezve van-e
    }
}
