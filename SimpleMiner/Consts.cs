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

            Coins.Add(new Coin
            {
                Algorithm = Algorithm.CryptoNightV7,
                CoinType = CoinTypes.XMR,
                Icon = "coinMonero.png",
                Name = "Monero",
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
                Algorithm = Algorithm.CryptoNightHeavy,
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
                CoinType = CoinTypes.GRFT,
                Icon = "coinGraft.png",
                Name = "Graft",
                ShortName = "GRFT",
                Webpage = "https://www.graft.network"
            });

            Coins.Add(new Coin
            {
                Algorithm = Algorithm.CryptoNightLite,
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
        public static readonly string ExeFileHash = "66-56-83-f3-63-6a-59-1e-81-27-f9-3a-bd-49-de-61-f8-c1-3e-2e-93-e9-48-4a-db-ab-5a-e4-9d-0e-3c-e1";
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
        public static string CustomConfigFile = ApplicationPath + "custom.ini";
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
            NiceHash,
            GRFT
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
            CryptoNightV7, //V7 pow
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
