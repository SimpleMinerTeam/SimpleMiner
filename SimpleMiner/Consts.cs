using System;
using System.Reflection;

namespace SimpleCPUMiner
{
    public static class Consts
    {
        public static string VersionNumber = Assembly.GetAssembly(typeof(Consts)).GetName().Version.ToString();
        public const string ExeFileName = "cpuminer.exe";
        public static string ProcessName = ExeFileName.Remove(ExeFileName.Length - 4, 4);
        public static string ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
        public static string ExecutablePath = AppDomain.CurrentDomain.BaseDirectory + ApplicationName + ".exe";
        public const string ApplicationName = "SimpleMiner";
        public static string ConfigFilePath = ApplicationPath + "config.bin";
        public const string StartupRegistryKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        public const string AboutContact = "simpleminerteam@gmail.com";

        public static class DefaultSettings
        {
            public const string UserName = "etnkEKwVnTfcwuBnSKuQgaQetJ7SiqnH3c6TU1HXBgFkSrtwaviEkBijMVrMhGi1aP4hPKJwaaKp5Rqhxi4pyP9i26A9dRJEhW";
            public const string URL = "etnpool.sytes.net";
            public const int Port = 3333;
            public const string Password = "x";
            public const int DonateLevel = 1;
            public const bool IsKeepalive = true;
            public const bool IsNoColor = false;
            public const int NumOfRetries = 100;
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
