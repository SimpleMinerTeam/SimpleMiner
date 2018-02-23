using System;
using System.Reflection;

namespace SimpleCPUMiner
{
    public static class Consts
    {
        public static string VersionNumber = Assembly.GetAssembly(typeof(Consts)).GetName().Version.ToString();
        public const string ExeFileName = "cpuminer.exe";
        public const string ToolExeFileName = "devcon.exe";
        public static readonly string ExeFileHash = "63-8a-dd-13-9e-cf-ef-a1-49-9a-81-61-b1-ff-40-cb-8a-3b-e7-b3-29-fa-14-7e-1c-2a-a4-bf-87-8a-80-1a";
        public const string MinerDownload = "http://cryptomanager.net/#simple_cpu_miner_downloads";
        public const string PackFileName = "miners.zip";
        public static string ProcessName = ExeFileName.Remove(ExeFileName.Length - 4, 4);
        public static string ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
        public static string ExecutablePath = AppDomain.CurrentDomain.BaseDirectory + ApplicationName + ".exe";
        public const string ApplicationName = "SimpleMiner";
        public static string ConfigFilePath = ApplicationPath + "config.bin";
        public static string PoolFilePath = ApplicationPath + "pools.bin";
        public static string GpuParameterFile = ApplicationPath + "gpuParameters.ini";
        public const string StartupRegistryKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        public const string AboutContact = "simpleminerteam@gmail.com";
        public static string addToDefenderExclusionBatchFilePath = ApplicationPath + "AddToDefenderExclusion.bat";
        public const string VendorAMD = "Advanced Micro Devices, Inc.";
        public const string VendorIntel = "Intel Corporation";
        public const string VendorIntel2 = "GenuineIntel";
        public const string VendorIntel3 = "Intel(R) Corporation";
        public const string VendorNvidia = "NVIDIA Corporation";
        public const string PlatformAMD = "AMD Accelerated Parallel Processing";
        public static string PowerShellCommandFile = ApplicationPath + "AddToDefenderExclusion.bat";

        public enum CoinTypes
        {
            XMR,
            ETN,
            SUMO,
            BCN,
            KRB,
            OTHER,
            TRTL
        }

        public enum MinerType
        {
            CPU,
            GPU
        }

        public enum Algorithm
        {
            CryptoNight
        }

        public static class DefaultSettings
        {
            public const string UserName = "etnkEKwVnTfcwuBnSKuQgaQetJ7SiqnH3c6TU1HXBgFkSrtwaviEkBijMVrMhGi1aP4hPKJwaaKp5Rqhxi4pyP9i26A9dRJEhW";
            public const string URL = "cryptomanager.net";
            public const int Port = 3333;
            public const string Password = "x";
            public const int DonateLevel = 1;
            public const bool IsKeepalive = true;
            public const bool IsNoColor = false;
            public const int NumOfRetries = 3;
            public const int RetryPause = 10;
            public const bool IsLogging = false;
            public const bool IsNicehashSupport = false;
            public const bool IsBackgroundMining = false;
            public const bool IsLaunchOnWindowsStartup = false;
            public const bool IsAutostartMining = false;
            public const int StartingDelayInSec = 20;
            public const bool IsMinimizeToTray = false;
            public const string NumberOfThreads = "0";
            public const int MaxCpuUsage = 75;
        }
    }
}
