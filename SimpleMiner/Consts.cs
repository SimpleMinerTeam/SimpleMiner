using SimpleCPUMiner.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;

namespace SimpleCPUMiner
{
    public static class Consts
    {
        static Consts()
        {

            #region  Coinok feltöltése

            Coins.Add(new Coin
            {
                Algorithm = (int)SupportedAlgos.CryptoNight_V8,
                CoinType = CoinTypes.XMR,
                Icon = "coinMonero.png",
                Name = "Monero",
                ShortName = "XMR",
                Webpage = "https://monero.org"
            });

            Coins.Add(new Coin
            {
                Algorithm = (int)SupportedAlgos.CryptoNight,
                CoinType = CoinTypes.ETN,
                Icon = "coinElectroneum.png",
                Name = "Electroneum",
                ShortName = "ETN",
                Webpage = "https://electroneum.com"
            });

            Coins.Add(new Coin
            {
                Algorithm = (int)SupportedAlgos.CryptoNight_Heavy,
                CoinType = CoinTypes.RYO,
                Icon = "coinRyo.png",
                Name = "Ryo",
                ShortName = "RYO",
                Webpage = "https://ryo-currency.com"
            });

            Coins.Add(new Coin
            {
                Algorithm = (int)SupportedAlgos.CryptoNight,
                CoinType = CoinTypes.BCN,
                Icon = "coinBytecoin.png",
                Name = "Bytecoin",
                ShortName = "BCN",
                Webpage = "https://bytecoin.org"
            });

            Coins.Add(new Coin
            {
                Algorithm = (int)SupportedAlgos.CryptoNight,
                CoinType = CoinTypes.KRB,
                Icon = "coinKrb.png",
                Name = "Karbo",
                ShortName = "KRB",
                Webpage = "https://karbo.io"
            });

            Coins.Add(new Coin
            {
                Algorithm = (int)SupportedAlgos.CryptoNight_V7,
                CoinType = CoinTypes.GRFT,
                Icon = "coinGraft.png",
                Name = "Graft",
                ShortName = "GRFT",
                Webpage = "https://www.graft.network"
            });

            Coins.Add(new Coin
            {
                Algorithm = (int)SupportedAlgos.CryptoNight_BitTube_V2,
                CoinType = CoinTypes.TUBE,
                Icon = "coinTube.png",
                Name = "BitTube",
                ShortName = "TUBE",
                Webpage = "https://coin.bit.tube"
            });

            Coins.Add(new Coin
            {
                Algorithm = (int)SupportedAlgos.CryptoNight_Lite_V7,
                CoinType = CoinTypes.TRTL,
                Icon = "coinTurtle.png",
                Name = "TurtleCoin",
                ShortName = "TRTL",
                Webpage = "https://turtlecoin.lol"
            });

            Coins.Add(new Coin
            {
                Algorithm = (int)SupportedAlgos.CryptoNight,
                CoinType = CoinTypes.NiceHash,
                Icon = "coinNiceHash.png",
                Name = "NiceHash",
                ShortName = "NiceHash",
                Webpage = "https://www.nicehash.com"
            });

            Coins.Add(new Coin
            {
                Algorithm = (int)SupportedAlgos.CryptoNight_Stellite_V4,
                CoinType = CoinTypes.XTL,
                Icon = "coinStellite.png",
                Name = "Stellite",
                ShortName = "XTL",
                Webpage = "https://stellite.cash/"
            });

            Coins.Add(new Coin
            {
                Algorithm = (int)SupportedAlgos.CryptoNight_Heavy,
                CoinType = CoinTypes.LOKI,
                Icon = "coinLoki.png",
                Name = "Loki",
                ShortName = "LOKI",
                Webpage = "https://loki.network/"
            });

            Coins.Add(new Coin
            {
                Algorithm = (int)SupportedAlgos.CryptoNight,
                CoinType = CoinTypes.SUMO,
                Icon = "coinSumokoin.png",
                Name = "Sumokoin",
                ShortName = "SUMO",
                Webpage = "https://www.sumokoin.org"
            });

            Coins.Add(new Coin
            {
                Algorithm = (int)SupportedAlgos.CryptoNight_Haven,
                CoinType = CoinTypes.XHV,
                Icon = "coinHaven.png",
                Name = "Haven",
                ShortName = "XHV",
                Webpage = "https://www.havenprotocol.com"
            });

            Coins.Add(new Coin
            {
                Algorithm = (int)SupportedAlgos.CryptoNight_Alloy,
                CoinType = CoinTypes.XAO,
                Icon = "coinAlloy.png",
                Name = "Alloy",
                ShortName = "XAO",
                Webpage = "https://www.havenprotocol.com"
            });

            Coins.Add(new Coin
            {
                Algorithm = (int)SupportedAlgos.CryptoNight,
                CoinType = CoinTypes.OTHER,
                Icon = "coinOther.png",
                Name = "Another Cryptonight coin",
                ShortName = "OTHER",
                Webpage = ""
            });

            #endregion

            #region Algoritmusok feltöltése

            Algorithms.Add(new Algo() {
                ID = (int)SupportedAlgos.CryptoNight,
                Name = EnumHelper.Description(SupportedAlgos.CryptoNight),
                IsCpuSupport = true,
                IsGpuSupport = true
            });

            Algorithms.Add(new Algo() {
                ID = (int)SupportedAlgos.CryptoNight_V7,
                Name = EnumHelper.Description(SupportedAlgos.CryptoNight_V7),
                IsCpuSupport = true,
                IsGpuSupport = true
            });

            Algorithms.Add(new Algo()
            {
                ID = (int)SupportedAlgos.CryptoNight_Lite,
                Name = EnumHelper.Description(SupportedAlgos.CryptoNight_Lite),
                IsCpuSupport = true,
                IsGpuSupport = true
            });

            Algorithms.Add(new Algo() {
                ID = (int)SupportedAlgos.CryptoNight_Lite_V7,
                Name = EnumHelper.Description(SupportedAlgos.CryptoNight_Lite_V7),
                IsCpuSupport = true,
                IsGpuSupport = false
            });

            Algorithms.Add(new Algo() {
                ID = (int)SupportedAlgos.CryptoNight_Heavy,
                Name = EnumHelper.Description(SupportedAlgos.CryptoNight_Heavy),
                IsCpuSupport = true,
                IsGpuSupport = true
            });

            Algorithms.Add(new Algo() {
                ID = (int)SupportedAlgos.CryptoNight_Fast,
                Name = EnumHelper.Description(SupportedAlgos.CryptoNight_Fast),
                IsCpuSupport = true,
                IsGpuSupport = true
            });

            Algorithms.Add(new Algo() {
                ID = (int)SupportedAlgos.CryptoNight_BitTube_V2,
                Name = EnumHelper.Description(SupportedAlgos.CryptoNight_BitTube_V2),
                IsCpuSupport = true,
                IsGpuSupport = false
            });

            Algorithms.Add(new Algo() {
                ID = (int)SupportedAlgos.CryptoNight_V8,
                Name = EnumHelper.Description(SupportedAlgos.CryptoNight_V8),
                IsCpuSupport = true,
                IsGpuSupport = true
            });

            Algorithms.Add(new Algo() {
                ID = (int)SupportedAlgos.CryptoNight_Stellite_V4,
                Name = EnumHelper.Description(SupportedAlgos.CryptoNight_Stellite_V4),
                IsCpuSupport = true,
                IsGpuSupport = false
            });

            Algorithms.Add(new Algo()
            {
                ID = (int)SupportedAlgos.CryptoNight_Haven,
                Name = EnumHelper.Description(SupportedAlgos.CryptoNight_Haven),
                IsCpuSupport = true,
                IsGpuSupport = false
            });

            Algorithms.Add(new Algo()
            {
                ID = (int)SupportedAlgos.CryptoNight_Alloy,
                Name = EnumHelper.Description(SupportedAlgos.CryptoNight_Alloy),
                IsCpuSupport = true,
                IsGpuSupport = false
            });

            #endregion
        }

        public enum SupportedAlgos
        {
            CryptoNight,
            CryptoNight_V7,
            CryptoNight_Lite,
            CryptoNight_Lite_V7,
            CryptoNight_Heavy,
            CryptoNight_Fast,
            CryptoNight_BitTube_V2,
            CryptoNight_V8,
            CryptoNight_Stellite_V4,
            CryptoNight_Haven,
            CryptoNight_Alloy
        }

        public static string VersionNumber = Assembly.GetAssembly(typeof(Consts)).GetName().Version.ToString();
        public const string ExeFileName = "cpuminer.exe";
        public const string ToolExeFileName = "devcon.exe";
#if X86
        public static readonly string ExeFileHash = "50-9d-1f-22-cc-bc-a9-c3-f6-8c-b0-b7-21-3e-7c-7e-b0-83-fc-3e-ea-73-ac-d7-f3-d4-2b-8e-0e-06-68-8c";
#else
        public static readonly string ExeFileHash = "f5-e4-55-8a-3f-55-b0-fe-4d-bc-04-45-48-32-73-64-95-59-18-c2-5a-26-55-1d-80-c6-7f-7b-2b-55-c3-a2";
#endif
        public const string MinerDownload = "http://cryptomanager.net/#simple_cpu_miner_downloads";
        public static string ProcessName = ExeFileName.Remove(ExeFileName.Length - 4, 4);
        public static string ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
        public static string ExecutablePath = AppDomain.CurrentDomain.BaseDirectory + ApplicationName + ".exe";
#if SMTU
        public const string ApplicationName = "TUSimpleMiner";
#else
        public const string ApplicationName = "SimpleMiner";
#endif
        public static string PackFileName = ApplicationPath + "miners.zip";
        public static string ConfigFilePath = ApplicationPath + "config.bin";
        public static string PoolFilePath = ApplicationPath + "pools.xml";
        public static string PoolFilePathOld = ApplicationPath + "pools.bin";
        public static string GpuParameterFile = ApplicationPath + "gpuParameters.ini";
        public static string ApplicationConfigFile = ApplicationPath + "configuration.ini";
        public static string CustomConfigFile = ApplicationPath + "custom.ini";
        public static string AutoUpdatePath = ApplicationPath + "SimpleMinerUpdater2.exe";
        public static string AutoUpdatePathOrig = ApplicationPath + "SimpleMinerUpdater.exe";
        public const string StartupRegistryKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        public static bool IsGenerateDefaultPoolList = false;
#if SMTU
        public static string AboutContact = "info@todosunidos.com";
#else
        public static string AboutContact = "simpleminerteam@gmail.com";
#endif

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
        public static List<Algo> Algorithms = new List<Algo>();

        public enum CoinTypes
        {
            XMR,        // Monero
            ETN,        // Electroneum
            SUMO,       // Sumokoin
            BCN,        // ByteCoin
            KRB,        // Karbo
            OTHER,      // Other
            TRTL,       // TurtleCoin
            NiceHash,   // NiceHash
            GRFT,       // Graft
            XTL,        // Stellite
            TUBE,       // Tube
            XHV,        // Haven
            LOKI,       // Loki
            IPBC,       // IPBC - obsoleted
            RYO,        // Ryo
            XAO         // Alloy
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

        public static SupportedAlgos[] V7Coins =
        {
            SupportedAlgos.CryptoNight_Lite,
            SupportedAlgos.CryptoNight_V7,
            SupportedAlgos.CryptoNight_BitTube_V2,
            SupportedAlgos.CryptoNight_Fast
        };

        //        public static Algorithm[] HeavyCoins =
        //        {
        //            Algorithm.CryptoNightHeavy,
        ////            Algorithm.CryptoNightHeavy,
        //            Algorithm.CryptoNightIpbc,

        //        };

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

        public static class PoolListBackgrounds
        {
            public static Brush Purple = new SolidColorBrush(Color.FromArgb(100, 128, 0, 128));     //Dolgozik a videókari is és a proci is
            public static Brush Orange = new SolidColorBrush(Color.FromArgb(100, 255, 180, 125));   //Csak a proci dolgozik
            public static Brush Green = new SolidColorBrush(Color.FromArgb(100, 0, 255, 0));        //Csak a videókari dolgozik
            public static Brush Transparent = Brushes.Transparent;
        }
    }
}
