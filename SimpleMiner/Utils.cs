using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace SimpleCPUMiner
{
    public static class Utils
    {
        public static void SerializeObject<T>(string filename, T obj)
        {
            using (Stream stream = File.Open(filename, FileMode.Create))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, obj);
                stream.Close();
            }
        }

        public static T DeSerializeObject<T>(string filename)
        {
            Stream stream = null;
            try
            {
                T objectToBeDeSerialized;
                using (stream = File.Open(filename, FileMode.Open))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    objectToBeDeSerialized = (T)binaryFormatter.Deserialize(stream);
                    stream.Close();
                }
                return objectToBeDeSerialized;
            }
            catch (Exception)
            {
                if (stream != null)
                    stream.Dispose();
                MessageBox.Show("Corrupted config file\n The program will use default settings.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return default(T);
            }

        }

        public static void AddProgramToStartup()
        {
            //RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(Consts.StartupRegistryKey, true);
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(Consts.StartupRegistryKey, true);

            if (!IsStartupItem())
                // Add the value in the registry so that the application runs at startup
                rkApp.SetValue("SimpleMiner", Consts.ExecutablePath);
        }

        public static void RemoveProgramFromStartup()
        {
            // The path to the key where Windows looks for startup applications
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(Consts.StartupRegistryKey, true);

            if (IsStartupItem())
                // Remove the value from the registry so that the application doesn't start
                rkApp.DeleteValue(Consts.ApplicationName, false);
        }

        public static bool IsStartupItem()
        {
            // The path to the key where Windows looks for startup applications
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(Consts.StartupRegistryKey, true);

            if (rkApp.GetValue(Consts.ApplicationName) == null)
                // The value doesn't exist, the application is not set to run at startup
                return false;
            else
                // The value exists, the application is set to run at startup
                return true;
        }
    }
}
