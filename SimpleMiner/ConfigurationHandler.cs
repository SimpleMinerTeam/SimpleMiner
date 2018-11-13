using System;
using SimpleCPUMiner.Model;
using System.IO;
using System.Windows;
using System.Reflection;
using GalaSoft.MvvmLight.Messaging;
using SimpleCPUMiner.Messages;

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
                        StartingDelayInSec = oldSettings.SettingsList[0].StartingDelayInSec,
                        IsLaunchOnWindowsStartup = oldSettings.SettingsList[0].IsLaunchOnWindowsStartup
                    };
                }
                else if(Consts.OSType == Consts.WindowsType._10_or_Server_2016 || Consts.OSType == Consts.WindowsType._8_1_or_Server_2012_R2)
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
             
            if (result == null || string.IsNullOrEmpty(result.ApplicationMode))
                result = new SimpleMinerSettings() {
                    ApplicationMode = Consts.ApplicationMode.Normal,
                    IsCPUMiningEnabled = true,
                    MaxCPUUsage = Consts.DefaultSettings.MaxCpuUsage,
                    NumberOfThreads = Consts.DefaultSettings.NumberOfThreads,
                    StartingDelayInSec = Consts.DefaultSettings.StartingDelayInSec,
                    VersionNumber = Consts.VersionNumber
                };

            WriteParameters(result);

            return result;
        }

        internal static Customization GetCustomConfig()
        {
            Customization result = null;

            if (!File.Exists(Consts.CustomConfigFile))
            {
                return null;
            }
            else
            {
                result = new Customization();
                ReadParameters(result);
            }

            return result;
        }

        public static bool ReadParameters(Customization _customization)
        {
            if (_customization == null)
                return false;

            if (File.Exists(Consts.CustomConfigFile))
            {
                try
                {
                    using (StreamReader reader = File.OpenText(Consts.CustomConfigFile))
                    {
                        string str;

                        while ((str = reader.ReadLine()) != null)
                        {
                            if (str.Contains("|"))
                                break;

                            string[] strArray = str.Split('=');
                            setCustomizationValue(strArray[0], strArray[1], _customization);
                        }
                    }
                    return true;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return false;
        }

        public static bool ReadParameters(SimpleMinerSettings setting)
        {
            if (setting == null)
                return false;

            if (File.Exists(Consts.ApplicationConfigFile))
            {
                try
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
                catch
                {
                    Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = "Configuration.ini is corrupted, default values loaded." });
                }
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
            //megvárjuk, hogy kiíródjon a fájl
            System.Threading.Thread.Sleep(300);
        }

        public static void WriteCustomParameters(Customization _customization)
        {
            if (_customization == null)
                return;

            using (StreamWriter text = File.CreateText(Consts.CustomConfigFile))
            {
                foreach (PropertyInfo property in _customization.GetType().GetProperties())
                    text.WriteLine(property.Name + "=" + property.GetValue(_customization, null));
            }
            //megvárjuk, hogy kiíródjon a fájl
            System.Threading.Thread.Sleep(300);
        }

        private static void setCustomizationValue(string _name, string _value, Customization _customization)
        {
            foreach (PropertyInfo property in _customization.GetType().GetProperties())
            {
                try
                {
                    if (property.Name == _name)
                    {
                        if (property.PropertyType == typeof(bool))
                        {
                            if (_value == "False")
                            {
                                property.SetValue(_customization, false, null);
                                break;
                            }
                            property.SetValue(_customization, true, null);
                            break;
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            if (property.Name.Equals("MainWindowTitle") && _value.Length > 60)
                                _value = _value.Substring(0, 60);

                            property.SetValue(_customization, _value, null);
                            break;
                        }
                        else if (property.PropertyType == typeof(int))
                        {
                            property.SetValue(_customization, Convert.ToInt32(_value), null);
                            break;
                        }
                        else if (property.PropertyType == typeof(byte))
                        {
                            property.SetValue(_customization, Convert.ToByte(_value), null);
                            break;
                        }
                        else if (property.PropertyType == typeof(double))
                        {
                            property.SetValue(_customization, Convert.ToDouble(_value), null);
                            break;
                        }
                        property.SetValue(_customization, Convert.ToInt32(_value), null);
                        break;
                    }
                }
                catch{}
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