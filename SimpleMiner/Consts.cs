using SimpleCPUMiner.Model;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SimpleCPUMiner
{
    public static class Consts
    {
        static Consts()
        {

            #region  Coinok feltöltése

            Coins.Add(new Coin {
                Algorithm = Algorithm.CryptoNight,
                CoinType = CoinTypes.XMR,
                Icon = "coinMonero.png",
                Name = "Monero",
                ShortName = "XMR",
                Webpage = "https://monero.org"
            });

            Coins.Add(new Coin
            {
                Algorithm = Algorithm.CryptoNight,
                CoinType = CoinTypes.XMR,
                Icon = "coinMonero.png",
                Name = "Monero v1 POW",
                ShortName = "XMR",
                Webpage = "https://monero.org"
            });

            Coins.Add(new Coin
            {
                Algorithm = Algorithm.CryptoNight,
                CoinType = CoinTypes.ETN,
                Icon = "coinElectroneum.png",
                Name = "Electroneum",
                ShortName = "ETN",
                Webpage = "https://electroneum.com"
            });

            Coins.Add(new Coin
            {
                Algorithm = Algorithm.CryptoNight,
                CoinType = CoinTypes.SUMO,
                Icon = "coinSumokoin.png",
                Name = "Sumokoin",
                ShortName = "SUMO",
                Webpage = "https://www.sumokoin.org"
            });

            Coins.Add(new Coin
            {
                Algorithm = Algorithm.CryptoNight,
                CoinType = CoinTypes.BCN,
                Icon = "coinBytecoin.png",
                Name = "Bytecoin",
                ShortName = "BCN",
                Webpage = "https://bytecoin.org"
            });

            Coins.Add(new Coin
            {
                Algorithm = Algorithm.CryptoNight,
                CoinType = CoinTypes.KRB,
                Icon = "coinKrb.png",
                Name = "Karbo",
                ShortName = "KRB",
                Webpage = "https://karbo.io"
            });

            Coins.Add(new Coin
            {
                Algorithm = Algorithm.CryptoNight,
                CoinType = CoinTypes.TRTL,
                Icon = "coinTurtle.png",
                Name = "TurtleCoin",
                ShortName = "TRTL",
                Webpage = "https://turtlecoin.lol"
            });

            Coins.Add(new Coin
            {
                Algorithm = Algorithm.CryptoNight,
                CoinType = CoinTypes.NiceHash,
                Icon = "coinNiceHash.png",
                Name = "NiceHash",
                ShortName = "NiceHash",
                Webpage = "https://www.nicehash.com"
            });

            Coins.Add(new Coin
            {
                Algorithm = Algorithm.CryptoNight,
                CoinType = CoinTypes.OTHER,
                Icon = "coinOther.png",
                Name = "Another Cryptonight coin",
                ShortName = "OTHER",
                Webpage = ""
            });
            
            #endregion

        }

        public static string VersionNumber = Assembly.GetAssembly(typeof(Consts)).GetName().Version.ToString();
        public const string ExeFileName = "cpuminer.exe";
        public const string ToolExeFileName = "devcon.exe";
        public static readonly string ExeFileHash = "03-00-a9-00-29-e4-12-fc-4d-ca-8f-02-be-09-0d-1a-3e-33-dc-c4-a8-49-5b-38-32-90-0f-ee-e2-dd-5b-63";
        public const string MinerDownload = "http://cryptomanager.net/#simple_cpu_miner_downloads";
        public static string ProcessName = ExeFileName.Remove(ExeFileName.Length - 4, 4);
        public static string ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
        public static string ExecutablePath = AppDomain.CurrentDomain.BaseDirectory + ApplicationName + ".exe";
        public const string ApplicationName = "SimpleMiner";
        public static string PackFileName = ApplicationPath + "miners.zip";
        public static string ConfigFilePath = ApplicationPath + "config.bin";
        public static string PoolFilePath = ApplicationPath + "pools.xml";
        public static string PoolFilePathOld = ApplicationPath + "pools.bin";
        public static string GpuParameterFile = ApplicationPath + "gpuParameters.ini";
        public static string ApplicationConfigFile = ApplicationPath + "configuration.ini";
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
        public static string WindowsVersionNumber = Utils.GetOSInformation("Version");
        //public static string OSArchitecture = Utils.GetOSInformation("OSArchitecture");
        //public static string WindowsCaption = Utils.GetOSInformation("Caption");
        public static WindowsType OSType = Utils.GetOSType();
        public static List<Coin> Coins = new List<Coin>();

        public enum CoinTypes
        {
            XMR,
            ETN,
            SUMO,
            BCN,
            KRB,
            OTHER,
            TRTL,
            NiceHash
        }

        public class ApplicationMode
        {
            public const string Normal = "Normal";
            public const string Silent = "Silent";
        }

        public enum MinerType
        {
            CPU,
            GPU
        }

        public enum Algorithm
        {
            CryptoNight,
            CryptoNightLite, //Aeon
            CryptoNightHeavy //Sumo
        }

        public enum WindowsType
        {
            _10_or_Server_2016,         //Windows 10 or Windows Server 2016
            _8_1_or_Server_2012_R2,     //Windows 8.1 or Windows Server 2012 R2
            _7_or_Server_2008_R2,       //Windows 7 or Windows Server 2008 R2
            Other                       //Other
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
            public const bool IsNicehashSupport = true;
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
