using System;
using SimpleCPUMiner.Model;
using System.IO;
using System.Windows;
using System.Reflection;

namespace SimpleCPUMiner
{
    internal class ConfigurationHandler
    {
        internal static SimpleMinerSettings GetConfig()
        {
            SimpleMinerSettings result = null;

            if (!File.Exists(Consts.ApplicationConfigFile))
            {
                if (File.Exists(Consts.ConfigFilePath))
                {
                    var oldSettings = Utils.DeSerializeObject<UserConfiguration>(Consts.ConfigFilePath);

                    result = new SimpleMinerSettings() {
                        ApplicationMode = Consts.ApplicationMode.Normal,
                        IsCPUMiningEnabled = true,
                        MaxCPUUsage = oldSettings.SettingsList[0].MaxCPUUsage,
                        NumberOfThreads = oldSettings.SettingsList[0].NumberOfThreads,
                        StartingDelayInSec = oldSettings.SettingsList[0].StartingDelayInSec
                    };
                }
                else
                {
                    MessageBoxResult mResult = MessageBox.Show("Do you want to add this folder to Defender exlusions?", "Defender exclusion", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (mResult == MessageBoxResult.Yes)
                    {
                        Utils.ExecutePSScript("Add-MpPreference -ExclusionPath '" + Consts.ApplicationPath + "'");
                    }
                }
            }
            else
            {
                result = new SimpleMinerSettings();
                ReadParameters(result);
            }
             
            if (result == null)
                result = new SimpleMinerSettings() {
                    ApplicationMode = Consts.ApplicationMode.Normal,
                    IsCPUMiningEnabled = true,
                    MaxCPUUsage = Consts.DefaultSettings.MaxCpuUsage,
                    NumberOfThreads = Consts.DefaultSettings.NumberOfThreads,
                    StartingDelayInSec = Consts.DefaultSettings.StartingDelayInSec
                };

            WriteParameters(result);

            return result;
        }

        public static bool ReadParameters(SimpleMinerSettings setting)
        {
            if (setting == null)
                return false;

            if (File.Exists(Consts.ApplicationConfigFile))
            {
                using (StreamReader streamReader = File.OpenText(Consts.ApplicationConfigFile))
                {
                    string str;

                    while ((str = streamReader.ReadLine()) != null)
                    {
                        if (str.Contains("|"))
                            break;

                        string[] strArray = str.Split('=');
                        SetPropertyValue(strArray[0], strArray[1], setting);
                    }
                }
                return true;
            }

            return false;
        }

        public static void WriteParameters(SimpleMinerSettings setting)
        {
            if (setting == null || setting.ApplicationMode.Equals(Consts.ApplicationMode.Silent))
                return;

            using (StreamWriter text = File.CreateText(Consts.ApplicationConfigFile))
            {
                foreach (PropertyInfo property in setting.GetType().GetProperties())
                    text.WriteLine(property.Name + "=" + property.GetValue(setting, null));
            }
        }

        private static void SetPropertyValue(string name, string value, SimpleMinerSettings setting)
        {
            foreach (PropertyInfo property in setting.GetType().GetProperties())
            {
                try
                {
                    if (property.Name == name)
                    {
                        if (property.PropertyType == typeof(bool))
                        {
                            if (value == "False")
                            {
                                property.SetValue(setting, false, null);
                                break;
                            }
                            property.SetValue(setting, true, null);
                            break;
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(setting, value, null);
                            break;
                        }
                        else if (property.PropertyType == typeof(int))
                        {
                            property.SetValue(setting, Convert.ToInt32(value), null);
                            break;
                        }
                        else if (property.PropertyType == typeof(byte))
                        {
                            property.SetValue(setting, Convert.ToByte(value), null);
                            break;
                        }
                        else if (property.PropertyType == typeof(double))
                        {
                            property.SetValue(setting, Convert.ToDouble(value), null);
                            break;
                        }
                        property.SetValue(setting, Convert.ToInt32(value), null);
                        break;
                    }
                }
                catch
                {

                }
            }
        }
    }
}