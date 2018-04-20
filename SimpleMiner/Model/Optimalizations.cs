using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.ServiceProcess;
using System.Text;

namespace SimpleCPUMiner.Model
{
    public class Optimization
    {
        private bool? isOn;

        public string Name { get; set; }        
        public bool IsNull { get; set; }

        public bool? IsOn {
            get
            {
                return isOn;
            }
            set
            {
                isOn = value;
                if (isOn == true || isOn == false)
                {
                    IsNull = false;
                }
                else
                {
                    IsNull = true;
                }
            }
        }

        public RelayCommand ApplyCommand { get; set; }
    }

    public static class Optimize
    {
        private static Dictionary<int, string> TaskIDToCommand = new Dictionary<int, string>();
        private static string polFilePath = Environment.ExpandEnvironmentVariables("%systemroot%") + "\\System32\\GroupPolicy\\Machine\\Registry.pol";
        private static string windowsUpdateDisabled = ConvertToUTF8String("[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0N\0o\0A\0u\0t\0o\0U\0p\0d\0a\0t\0e\0\0\0;\0\u0004\0\0\0;\0\u0004\0\0\0;\0\u0001\0\0\0]\0[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0*\0*\0d\0e\0l\0.\0A\0U\0O\0p\0t\0i\0o\0n\0s\0\0\0;\0\u0001\0\0\0;\0\u0004\0\0\0;\0 \0\0\0]\0[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0*\0*\0d\0e\0l\0.\0A\0u\0t\0o\0m\0a\0t\0i\0c\0M\0a\0i\0n\0t\0e\0n\0a\0n\0c\0e\0E\0n\0a\0b\0l\0e\0d\0\0\0;\0\u0001\0\0\0;\0\u0004\0\0\0;\0 \0\0\0]\0[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0*\0*\0d\0e\0l\0.\0S\0c\0h\0e\0d\0u\0l\0e\0d\0I\0n\0s\0t\0a\0l\0l\0D\0a\0y\0\0\0;\0\u0001\0\0\0;\0\u0004\0\0\0;\0 \0\0\0]\0[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0*\0*\0d\0e\0l\0.\0S\0c\0h\0e\0d\0u\0l\0e\0d\0I\0n\0s\0t\0a\0l\0l\0T\0i\0m\0e\0\0\0;\0\u0001\0\0\0;\0\u0004\0\0\0;\0 \0\0\0]\0[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0*\0*\0d\0e\0l\0.\0S\0c\0h\0e\0d\0u\0l\0e\0d\0I\0n\0s\0t\0a\0l\0l\0E\0v\0e\0r\0y\0W\0e\0e\0k\0\0\0;\0\u0001\0\0\0;\0\u0004\0\0\0;\0 \0\0\0]\0[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0*\0*\0d\0e\0l\0.\0S\0c\0h\0e\0d\0u\0l\0e\0d\0I\0n\0s\0t\0a\0l\0l\0F\0i\0r\0s\0t\0W\0e\0e\0k\0\0\0;\0\u0001\0\0\0;\0\u0004\0\0\0;\0 \0\0\0]\0[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0*\0*\0d\0e\0l\0.\0S\0c\0h\0e\0d\0u\0l\0e\0d\0I\0n\0s\0t\0a\0l\0l\0S\0e\0c\0o\0n\0d\0W\0e\0e\0k\0\0\0;\0\u0001\0\0\0;\0\u0004\0\0\0;\0 \0\0\0]\0[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0*\0*\0d\0e\0l\0.\0S\0c\0h\0e\0d\0u\0l\0e\0d\0I\0n\0s\0t\0a\0l\0l\0T\0h\0i\0r\0d\0W\0e\0e\0k\0\0\0;\0\u0001\0\0\0;\0\u0004\0\0\0;\0 \0\0\0]\0[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0*\0*\0d\0e\0l\0.\0S\0c\0h\0e\0d\0u\0l\0e\0d\0I\0n\0s\0t\0a\0l\0l\0F\0o\0u\0r\0t\0h\0W\0e\0e\0k\0\0\0;\0\u0001\0\0\0;\0\u0004\0\0\0;\0 \0\0\0]\0[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0*\0*\0d\0e\0l\0.\0A\0l\0l\0o\0w\0M\0U\0U\0p\0d\0a\0t\0e\0S\0e\0r\0v\0i\0c\0e\0\0\0;\0\u0001\0\0\0;\0\u0004\0\0\0;\0 \0\0\0]\0");
        private static string windowsUpdateEnabled = ConvertToUTF8String("[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0N\0o\0A\0u\0t\0o\0U\0p\0d\0a\0t\0e\0\0\0;\0\u0004\0\0\0;\0\u0004\0\0\0;\0\0\0\0\0]\0[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0A\0U\0O\0p\0t\0i\0o\0n\0s\0\0\0;\0\u0004\0\0\0;\0\u0004\0\0\0;\0\u0003\0\0\0]\0[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0*\0*\0d\0e\0l\0.\0A\0u\0t\0o\0m\0a\0t\0i\0c\0M\0a\0i\0n\0t\0e\0n\0a\0n\0c\0e\0E\0n\0a\0b\0l\0e\0d\0\0\0;\0\u0001\0\0\0;\0\u0004\0\0\0;\0 \0\0\0]\0[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0S\0c\0h\0e\0d\0u\0l\0e\0d\0I\0n\0s\0t\0a\0l\0l\0D\0a\0y\0\0\0;\0\u0004\0\0\0;\0\u0004\0\0\0;\0\0\0\0\0]\0[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0S\0c\0h\0e\0d\0u\0l\0e\0d\0I\0n\0s\0t\0a\0l\0l\0T\0i\0m\0e\0\0\0;\0\u0004\0\0\0;\0\u0004\0\0\0;\0\u0003\0\0\0]\0[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0S\0c\0h\0e\0d\0u\0l\0e\0d\0I\0n\0s\0t\0a\0l\0l\0E\0v\0e\0r\0y\0W\0e\0e\0k\0\0\0;\0\u0004\0\0\0;\0\u0004\0\0\0;\0\u0001\0\0\0]\0[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0*\0*\0d\0e\0l\0.\0S\0c\0h\0e\0d\0u\0l\0e\0d\0I\0n\0s\0t\0a\0l\0l\0F\0i\0r\0s\0t\0W\0e\0e\0k\0\0\0;\0\u0001\0\0\0;\0\u0004\0\0\0;\0 \0\0\0]\0[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0*\0*\0d\0e\0l\0.\0S\0c\0h\0e\0d\0u\0l\0e\0d\0I\0n\0s\0t\0a\0l\0l\0S\0e\0c\0o\0n\0d\0W\0e\0e\0k\0\0\0;\0\u0001\0\0\0;\0\u0004\0\0\0;\0 \0\0\0]\0[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0*\0*\0d\0e\0l\0.\0S\0c\0h\0e\0d\0u\0l\0e\0d\0I\0n\0s\0t\0a\0l\0l\0T\0h\0i\0r\0d\0W\0e\0e\0k\0\0\0;\0\u0001\0\0\0;\0\u0004\0\0\0;\0 \0\0\0]\0[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0*\0*\0d\0e\0l\0.\0S\0c\0h\0e\0d\0u\0l\0e\0d\0I\0n\0s\0t\0a\0l\0l\0F\0o\0u\0r\0t\0h\0W\0e\0e\0k\0\0\0;\0\u0001\0\0\0;\0\u0004\0\0\0;\0 \0\0\0]\0[\0S\0o\0f\0t\0w\0a\0r\0e\0\\\0P\0o\0l\0i\0c\0i\0e\0s\0\\\0M\0i\0c\0r\0o\0s\0o\0f\0t\0\\\0W\0i\0n\0d\0o\0w\0s\0\\\0W\0i\0n\0d\0o\0w\0s\0U\0p\0d\0a\0t\0e\0\\\0A\0U\0\0\0;\0*\0*\0d\0e\0l\0.\0A\0l\0l\0o\0w\0M\0U\0U\0p\0d\0a\0t\0e\0S\0e\0r\0v\0i\0c\0e\0\0\0;\0\u0001\0\0\0;\0\u0004\0\0\0;\0 \0\0\0]\0");

        public enum Status
        {
            Exists,
            Done,
            Running,
            NA,
            Error
        }
        public enum Type
        {
            ScheduledTask,
            Service
        }

        public static class UAC
        {
            /// <summary>
            /// Ellenőrzi az UAC státuszát
            /// </summary>
            /// <returns>Jelenlegi UAC státusz.</returns>
            public static bool? IsActive()
            {
                if (Consts.OSType == Consts.WindowsType._10_or_Server_2016 ||
                    Consts.OSType == Consts.WindowsType._8_1_or_Server_2012_R2 ||
                    Consts.OSType == Consts.WindowsType._7_or_Server_2008_R2)
                {
                    try
                    {
                        var psi = new Process
                        {

                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "cmd",
                                Arguments = "/c" + "REG QUERY HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\ /v EnableLUA",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true,
                                StandardOutputEncoding = Encoding.GetEncoding(866)
                            }
                        };

                        psi.Start();
                        string line = null;

                        while (!psi.StandardOutput.EndOfStream)
                        {
                            line = Environment.NewLine + psi.StandardOutput.ReadLine();

                            if (line.ToUpper().Contains("ENABLELUA"))
                            {
                                string result = line.Remove(0, line.Length - 3);

                                if (result == "0x1")
                                {
                                    return true;
                                }
                                else if (result == "0x0") return false;
                                else
                                {
                                    Log.InsertError($"UAC state:[{result}]");
                                    return null;
                                }
                            }
                        }

                        psi.WaitForExit();

                        Log.InsertError("UAC checking error!");
                        return null;
                    }
                    catch (Exception ex)
                    {
                        Log.InsertError(ex.Message);
                        return null;
                    }
                }

                return null;
            }

            public static void SetToActive()
            {
                if (Consts.OSType == Consts.WindowsType._10_or_Server_2016 || Consts.OSType == Consts.WindowsType._8_1_or_Server_2012_R2)
                {
                    try
                    {
                        Utils.RunPowerShellScript("New-ItemProperty -Path HKLM:Software\\Microsoft\\Windows\\CurrentVersion\\policies\\system -Name EnableLUA -PropertyType DWord -Value 1 -Force");
                    }
                    catch (Exception ex)
                    {
                        Log.InsertError(ex.Message);
                        return;
                    }
                }
                else if (Consts.OSType == Consts.WindowsType._7_or_Server_2008_R2)
                {
                    try
                    {
                        RegistryKey rkApp = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true);
                        rkApp.SetValue("ConsentPromptBehaviorAdmin", "00000001", RegistryValueKind.DWord);
                        rkApp.SetValue("EnableLUA", "00000001", RegistryValueKind.DWord);
                    }
                    catch (Exception ex)
                    {
                        Log.InsertError(ex.Message);
                        return;
                    }
                }
            }

            public static void SetToInactive()
            {
                if (Consts.OSType == Consts.WindowsType._10_or_Server_2016 || Consts.OSType == Consts.WindowsType._8_1_or_Server_2012_R2)
                {
                    try
                    {
                        Utils.RunPowerShellScript("New-ItemProperty -Path HKLM:Software\\Microsoft\\Windows\\CurrentVersion\\policies\\system -Name EnableLUA -PropertyType DWord -Value 0 -Force");
                    }
                    catch (Exception ex)
                    {
                        Log.InsertError(ex.Message);
                    }
                }
                else if (Consts.OSType == Consts.WindowsType._7_or_Server_2008_R2)
                {
                    try
                    {
                        RegistryKey rkApp = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true);
                        rkApp.SetValue("ConsentPromptBehaviorAdmin", "00000000", RegistryValueKind.DWord);
                        rkApp.SetValue("EnableLUA", "00000000", RegistryValueKind.DWord);
                    }
                    catch (Exception ex)
                    {
                        Log.InsertError(ex.Message);
                        return;
                    }
                }
            }
        }

        public static class DefenderRealTimeProtection
        {
            /// <summary>
            /// Jelenleg aktív-e a Defender Real-time védelme?
            /// </summary>
            public static bool? IsActive()
            {
                RegistryKey OurKey = Registry.LocalMachine;

                if (Consts.OSType == Consts.WindowsType._10_or_Server_2016 || Consts.OSType == Consts.WindowsType._8_1_or_Server_2012_R2)
                {
                    try
                    {
                        OurKey = OurKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows Defender\Real-Time Protection\");

                        if (OurKey != null)
                        {
                            var data = OurKey.GetValue("DisableRealtimeMonitoring");

                            if (data == null || Convert.ToInt32(data) == 0)
                            {
                                return true;
                            }
                            else return false;
                        }
                    }
                    catch(Exception ex)
                    {
                        Log.InsertError("DefenderRealTimeProtection: " + ex.Message);
                        return null;
                    }
                }
                //else if (Consts.OSType == Consts.WindowsType._7_or_Server_2008_R2)
                //{
                //    try
                //    {
                //        OurKey = OurKey.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft Antimalware\Real-Time Protection\");

                //        RegistryPermission perm1 = new RegistryPermission(RegistryPermissionAccess.Write, @"SOFTWARE\Microsoft\Microsoft Antimalware\Real-Time Protection");
                //        perm1.Demand();
                //    }
                //    catch
                //    {
                //        return null;
                //    }

                //    if (OurKey != null)
                //    {
                //        var data = OurKey.GetValue("DisableRealtimeMonitoring");

                //        if (Convert.ToInt32(data) == 0) return true;
                //        else if (Convert.ToInt32(data) == 1) return false;
                //        else return null;
                //    }
                //}

                return null;
            }

            /// <summary>
            /// Bekapcsolja a Defender Real-time védelmét.
            /// </summary>
            public static void SetToActive()
            {
                if (Consts.OSType == Consts.WindowsType._10_or_Server_2016 || Consts.OSType == Consts.WindowsType._8_1_or_Server_2012_R2)
                {
                    Utils.RunPowerShellScript("Set-MpPreference -DisableRealtimeMonitoring $false");
                }
                //else if (Consts.OSType == Consts.WindowsType._7_or_Server_2008_R2)
                //{
                //    try
                //    {
                //        RegistrySecurity rs = new RegistrySecurity();
                //        string currentUserStr = Environment.UserDomainName + "\\" + Environment.UserName;
                //        rs.AddAccessRule(new RegistryAccessRule(currentUserStr, RegistryRights.WriteKey | RegistryRights.ReadKey | RegistryRights.Delete | RegistryRights.FullControl, AccessControlType.Allow));

                //        RegistryKey rkApp = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Microsoft Antimalware\\Real-Time Protection", true);

                //        if (rkApp != null)
                //        {
                //            rkApp.SetValue("ConsentPromptBehaviorAdmin", "00000000", RegistryValueKind.DWord);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        Log.InsertError(ex.Message);
                //        return;
                //    }
                //}
            }

            /// <summary>
            /// Kikapcsolja a Defender Real-time védelmét.
            /// </summary>
            public static void SetToInactive()
            {
                if (Consts.OSType == Consts.WindowsType._10_or_Server_2016 || Consts.OSType == Consts.WindowsType._8_1_or_Server_2012_R2)
                {
                    Utils.RunPowerShellScript("Set-MpPreference -DisableRealtimeMonitoring $true");
                }
                //else if (Consts.OSType == Consts.WindowsType._7_or_Server_2008_R2)
                //{
                //    try
                //    {
                //        RegistryKey rkApp = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Microsoft Antimalware\\Real-Time Protection", true);
                //        RegistryPermission perm1 = new RegistryPermission(RegistryPermissionAccess.Write, @"SOFTWARE\Microsoft\Microsoft Antimalware\Real-Time Protection");
                //        perm1.Demand();

                //        if (rkApp != null)
                //        {
                //            rkApp.SetValue("ConsentPromptBehaviorAdmin", "00000001", RegistryValueKind.DWord);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        Log.InsertError(ex.Message);
                //        return;
                //    }
                //}
            }
        }

        public static class DefenderSmartScreen
        {
            private const string regValue = "SmartScreenEnabled";
            private static RegistryKey rkApp;

            public static bool? IsEnabled()
            {
                try
                {
                    if (Consts.OSType == Consts.WindowsType._10_or_Server_2016 || Consts.OSType == Consts.WindowsType._8_1_or_Server_2012_R2)
                    {
                        rkApp = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer", true);
                    }
                    else
                    {
                        return null;
                    }

                    if (rkApp.GetValue(regValue).ToString().Equals("Off", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Log.InsertError("DefenderSmartScreen: " + ex.Message);
                    return null;
                }
            }

            public static void SetToEnabled()
            {
                try
                {
                    rkApp.SetValue(regValue, "RequireAdmin");
                }
                catch (Exception ex)
                {
                    Log.InsertError(ex.Message);
                    return;
                }
            }

            public static void SetToDisabled()
            {
                try
                {
                    rkApp.SetValue(regValue, "Off");
                }
                catch (Exception ex)
                {
                    Log.InsertError(ex.Message);
                    return;
                }
            }
        }

        public static class SleepMode
        {
            public static bool? IsEnabled()
            {
                if (Consts.OSType == Consts.WindowsType._10_or_Server_2016 || Consts.OSType == Consts.WindowsType._8_1_or_Server_2012_R2)
                {
                    try
                    {
                        if (PowerSettings.GetActiveSchemaStandByTimeout() == 0 && PowerSettings.GetActiveSchemaHibernateTimeout() == 0)
                            return false;
                        else if (PowerSettings.GetActiveSchemaStandByTimeout() > 0 || PowerSettings.GetActiveSchemaHibernateTimeout() > 0)
                            return true;
                        else
                            return null;
                    }
                    catch (Exception ex)
                    {
                        Log.InsertError("SleepMode: " + ex.Message);
                        return null;
                    }
                }
                else if (Consts.OSType == Consts.WindowsType._7_or_Server_2008_R2)
                {
                    try
                    {
                        RegistryKey OurKey = Registry.LocalMachine;

                        OurKey = OurKey.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Power", true);

                        if (OurKey != null)
                        {
                            var data = OurKey.GetValue("HibernateEnabled");

                            if (Convert.ToInt32(data) == 1)
                            {
                                return true;
                            }
                            else return false;
                        }
                        else return null;
                    }
                    catch (Exception ex)
                    {
                        Log.InsertError("SleepMode: " + ex.Message);
                        return null;
                    }
                }
                else return null;
            }

            public static void SetToDisable()
            {
                if (Consts.OSType == Consts.WindowsType._10_or_Server_2016 || Consts.OSType == Consts.WindowsType._8_1_or_Server_2012_R2)
                {
                    PowerSettings.SetActiveSchemaStandByTimeout(0);
                    PowerSettings.SetActiveSchemaHibernateTimeout(0);
                }
                else if (Consts.OSType == Consts.WindowsType._7_or_Server_2008_R2)
                {
                    RegistryKey rkApp = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Power", true);

                    try
                    {
                        rkApp.SetValue("HibernateEnabled", "0", RegistryValueKind.DWord);
                        rkApp.SetValue("HiberFileSizePercent", "0", RegistryValueKind.DWord);
                    }
                    catch (Exception ex)
                    {
                        Log.InsertError(ex.Message);
                        return;
                    }
                }
            }

            public static void SetToEnabled()
            {
                if (Consts.OSType == Consts.WindowsType._10_or_Server_2016 || Consts.OSType == Consts.WindowsType._8_1_or_Server_2012_R2)
                {
                    PowerSettings.SetActiveSchemaStandByTimeout(14400);
                    PowerSettings.SetActiveSchemaHibernateTimeout(10800);
                }
                else if (Consts.OSType == Consts.WindowsType._7_or_Server_2008_R2)
                {
                    RegistryKey rkApp = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Power", true);

                    try
                    {
                        rkApp.SetValue("HibernateEnabled", "1", RegistryValueKind.DWord);
                        rkApp.SetValue("HiberFileSizePercent", "75", RegistryValueKind.DWord);
                    }
                    catch (Exception ex)
                    {
                        Log.InsertError(ex.Message);
                        return;
                    }
                }
            }
        }

        public static class WindowsUpdate
        {
            public static bool? IsEnabled()
            {
                if (Consts.OSType == Consts.WindowsType._10_or_Server_2016)
                {
                    return !isWindowsUpdateDisabled();
                }
                else if (Consts.OSType == Consts.WindowsType._7_or_Server_2008_R2)
                {
                    try
                    {
                        RegistryKey rkApp = Registry.LocalMachine.OpenSubKey("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WindowsUpdate\\Auto Update", false);
                        var result = rkApp.GetValue("AUOptions").ToString();

                        if (result == "0") return false;
                        else return true;
                    }
                    catch(Exception ex)
                    {
                        Log.InsertError(ex.Message);
                        return null;
                    }
                    
                    //Ha 0 akkor ki  van kapcsolva, különben be van
                }
                else return null;
            }

            //Set to Not configured
            public static void SetToEnable()
            {
                if (Consts.OSType == Consts.WindowsType._10_or_Server_2016 || Consts.OSType == Consts.WindowsType._8_1_or_Server_2012_R2)
                {
                    setToNotConfigured();
                }
                else if (Consts.OSType == Consts.WindowsType._7_or_Server_2008_R2)
                {
                    try
                    {
                        var psi = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "cmd",
                                Arguments = "/c" + "add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WindowsUpdate\\Auto Update\" /v AUOptions /t REG_DWORD /d 2 /f",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true,
                                StandardOutputEncoding = Encoding.GetEncoding(866)
                            }
                        };

                        psi.Start();
                    }
                    catch(Exception ex)
                    {
                        Log.InsertError(ex.Message);
                        return;
                    }
                }
            }

            public static void SetToDisable()
            {
                if (Consts.OSType == Consts.WindowsType._10_or_Server_2016 || Consts.OSType == Consts.WindowsType._8_1_or_Server_2012_R2)
                {
                    setToDisabled();
                }
                else if (Consts.OSType == Consts.WindowsType._7_or_Server_2008_R2)
                {
                    try
                    {
                        var psi = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "cmd",
                                Arguments = "/c" + "add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WindowsUpdate\\Auto Update\" /v AUOptions /t REG_DWORD /d 0 /f",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true,
                                StandardOutputEncoding = Encoding.GetEncoding(866)
                            }
                        };

                        psi.Start();
                    }
                    catch (Exception ex)
                    {
                        Log.InsertError(ex.Message);
                        return;
                    }
                }
            }

            private static bool isWindowsUpdateEnabled()
            {
                var fileContent = File.ReadAllBytes(polFilePath);
                string fileContentString = Encoding.UTF8.GetString(fileContent);

                return IsGPFunctionApplied(fileContentString, windowsUpdateEnabled);
            }

            private static bool isWindowsUpdateDisabled()
            {
                var fileContent = File.ReadAllBytes(polFilePath);
                string fileContentString = Encoding.UTF8.GetString(fileContent);

                return IsGPFunctionApplied(fileContentString, windowsUpdateDisabled);
            }

            /// <summary>
            /// Not configured-re állítja (azaz visszakapcsolja) a Windows Update-et.
            /// </summary>
            private static void setToNotConfigured()
            {
                var fileContent = File.ReadAllBytes(polFilePath);
                string fileContentString = Encoding.UTF8.GetString(fileContent);
                int temp = 0;
                int match = 0;

                if (isWindowsUpdateEnabled()) //Ha az Enabled benne van a fájlban, akkor töröljük
                {
                    int enabledLength = windowsUpdateEnabled.Length;

                    for (int i = 0; i < fileContentString.Length - enabledLength + 1; i++)
                    {
                        temp = i;

                        for (int j = 0; j < enabledLength; j++)
                        {
                            if (fileContentString[temp] == windowsUpdateEnabled[j])
                            {
                                match++;
                                temp++;
                            }
                            else
                            {
                                match = 0;
                                break;
                            }

                            if (match == windowsUpdateEnabled.Length)
                            {
                                break;
                            }
                        }

                        if (match == windowsUpdateEnabled.Length)
                        {
                            fileContentString = fileContentString.Remove(i, enabledLength);
                            break;
                        }
                    }
                }

                //Töröljük a Disabledet

                int disabledLength = windowsUpdateDisabled.Length;

                for (int i = 0; i < fileContentString.Length - disabledLength + 1; i++)
                {
                    temp = i;

                    for (int j = 0; j < disabledLength; j++)
                    {
                        if (fileContentString[temp] == windowsUpdateDisabled[j])
                        {
                            match++;
                            temp++;
                        }
                        else
                        {
                            match = 0;
                            break;
                        }

                        if (match == windowsUpdateDisabled.Length)
                        {
                            break;
                        }
                    }

                    if (match == windowsUpdateDisabled.Length)
                    {
                        fileContentString = fileContentString.Remove(i, disabledLength);
                        break;
                    }
                }

                File.WriteAllText(polFilePath, fileContentString);
            }

            private static void setToDisabled()
            {
                var fileContent = File.ReadAllBytes(polFilePath);
                string fileContentString = Encoding.UTF8.GetString(fileContent);

                fileContentString += windowsUpdateDisabled;

                File.WriteAllText(polFilePath, fileContentString);
            }
        }

        /// <summary>
        /// Megvizsgáljuk, hogy a fileban benne van-e már a funkció
        /// </summary>
        /// <param name="_content">A fájl beolvasott tartalma.</param>
        /// <param name="_gpFunction">A ki/be kapcsolandó funkció.</param>
        /// <returns>Szerepel már a funkció a fájlban?</returns>
        public static bool IsGPFunctionApplied(string _content, string _gpFunction)
        {
            byte[] bytesGpFunction = Encoding.Default.GetBytes(_gpFunction);
            var utf8GpFunction = Encoding.UTF8.GetString(bytesGpFunction);

            //Megnézzük, hogy a tartalom van-e olyan hosszú, mint a funkció amit keresünk
            if (_content.Length < _gpFunction.Length + 8)
            {
                return false;
            }
            else
            {
                int match = 0;
                int temp = 0;

                for (int i = 0; i < _content.Length - _gpFunction.Length + 1; i++)
                {
                    temp = i;

                    for (int j = 0; j < _gpFunction.Length; j++)
                    {
                        if (_content[temp] == _gpFunction[j])
                        {
                            match++;
                            temp++;
                        }
                        else
                        {
                            match = 0;
                            break;
                        }

                        if (match == _gpFunction.Length)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        internal static ObservableCollection<Optimization> GetOptList()
        {
            var OptList = new ObservableCollection<Optimization>();

            OptList.Add(new Optimization
            {
                Name = "User Account Control (UAC)",
                IsOn = UAC.IsActive(),
                ApplyCommand = new RelayCommand(() => {
                    if (UAC.IsActive() == true)
                        UAC.SetToInactive();
                    else
                        UAC.SetToActive();
                })
            });
            OptList.Add(new Optimization
            {
                Name = "Defender Real Time Protection",
                IsOn = DefenderRealTimeProtection.IsActive(),
                ApplyCommand = new RelayCommand(() =>
                {
                    if (DefenderRealTimeProtection.IsActive() == true)
                        DefenderRealTimeProtection.SetToInactive();
                    else
                        DefenderRealTimeProtection.SetToActive();
                })
            });
            OptList.Add(new Optimization
            {
                Name = "Defender Smart Screen",
                IsOn = DefenderSmartScreen.IsEnabled(),
                ApplyCommand = new RelayCommand(() =>
                {
                    if (DefenderSmartScreen.IsEnabled() == true)
                        DefenderSmartScreen.SetToDisabled();
                    else
                        DefenderSmartScreen.SetToEnabled();
                })
            });
            OptList.Add(new Optimization
            {
                Name = "Sleep mode",
                IsOn = SleepMode.IsEnabled(),
                ApplyCommand = new RelayCommand(() =>
                {
                    if (SleepMode.IsEnabled() == true)
                        SleepMode.SetToDisable();
                    else
                        SleepMode.SetToEnabled();
                })
            });
            OptList.Add(new Optimization
            {
                Name = "Windows update",
                IsOn = WindowsUpdate.IsEnabled(),
                ApplyCommand = new RelayCommand(() =>
                {
                    if (WindowsUpdate.IsEnabled() == true)
                        WindowsUpdate.SetToDisable();
                    else
                        WindowsUpdate.SetToEnable();
                })
            });

            return OptList;
        }

        //public static void ApplyChanges()
        //{
        //    foreach (var topLevelItem in OptList)
        //    {
        //        foreach (var subItem in topLevelItem.OptList)
        //        {
        //            if (subItem.IsSelected == false) continue;

        //            switch (subItem.ItemType)
        //            {
        //                case Type.ScheduledTask:
        //                    subItem.CurrentStatus = checkScheduledTaskStatus(subItem);
        //                    break;
        //                case Type.Service:
        //                    subItem.CurrentStatus = Status.Done;
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //    }
        //}

        //private static Status checkScheduledTaskStatus(Optimization _opt)
        //{
        //    if (isScheduledTaskExists(TaskIDToCommand[_opt.ID]))
        //    {
        //        return Status.Exists;
        //    }
        //    else
        //    {
        //        return Status.NA;
        //    }
        //}

        //private static bool isScheduledTaskExists(string _taskName)
        //{
        //    Process p = new Process();
        //    p.StartInfo.UseShellExecute = false;
        //    p.StartInfo.FileName = "SCHTASKS.exe";
        //    p.StartInfo.RedirectStandardError = true;
        //    p.StartInfo.RedirectStandardOutput = true;
        //    p.StartInfo.CreateNoWindow = true;
        //    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

        //    p.StartInfo.Arguments = String.Format("/Query /TN \"{0}\" /FO TABLE /NH", _taskName);

        //    p.Start();
        //    // Read the error stream
        //    string error = p.StandardError.ReadToEnd();

        //    //Read the output string
        //    p.StandardOutput.ReadLine();
        //    string tbl = p.StandardOutput.ReadToEnd();

        //    //Then wait for it to finish
        //    p.WaitForExit();

        //    //Check for an error
        //    if (!String.IsNullOrWhiteSpace(error))
        //    {
        //        throw new Exception(error);
        //    }
        //    Console.WriteLine(tbl);
        //    //Parse output
        //    return tbl.Split(new String[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim().EndsWith("Running");
        //}

        //public static void FillOptList()
        //{
        //    #region Office Scheduled Tasks

        //    if (OptList.Count > 0)
        //    {
        //        return;
        //    }

        //    OptList.Insert((int)TopListItems.OfficeScheduledTasks, new Optimization
        //    {
        //        ID = (int)TopListItems.OfficeScheduledTasks,
        //        Name = "Disable Office scheduled tasks",
        //        Description = "",
        //        IsSelected = false
        //    });

        //    OptList[(int)TopListItems.OfficeScheduledTasks].OptList = new Optimization[] {
        //        new Optimization { ID = 1, IsSelected = false, Name = "ClickToRun Service Monitor", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.OfficeScheduledTasks] },
        //        new Optimization { ID = 2, IsSelected = false, Name = "TelemetryAgentFallBack2016", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.OfficeScheduledTasks] },
        //        new Optimization { ID = 3, IsSelected = false, Name = "TelemetryAgentLogOn2016", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.OfficeScheduledTasks] },
        //        new Optimization { ID = 4, IsSelected = false, Name = "AgentFallBack2016", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.OfficeScheduledTasks] },
        //        new Optimization { ID = 5, IsSelected = false, Name = "TelemetryAgentLogOn2016", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.OfficeScheduledTasks] },
        //    };

        //    TaskIDToCommand.Add(1, @"Microsoft\Office\Office ClickToRun Service Monitor");
        //    TaskIDToCommand.Add(2, @"Microsoft\Office\OfficeTelemetryAgentFallBack2016");
        //    TaskIDToCommand.Add(3, @"Microsoft\Office\OfficeTelemetryAgentLogOn2016");
        //    TaskIDToCommand.Add(4, @"Microsoft\Office\OfficeTelemetry\AgentFallBack2016");
        //    TaskIDToCommand.Add(5, @"Microsoft\Office\OfficeTelemetry\OfficeTelemetryAgentLogOn2016");

        //    #endregion


        //    #region Customer & application experiences 

        //    OptList.Insert((int)TopListItems.ExperiencesScheduledTasks,
        //            new Optimization
        //            {
        //                ID = (int)TopListItems.ExperiencesScheduledTasks,
        //                Name = "Disable Experience sched. tasks",
        //                Description = "",
        //                IsSelected = false,
        //            });

        //    OptList[(int)TopListItems.ExperiencesScheduledTasks].OptList = new Optimization[] {
        //        new Optimization { ID = 101, IsSelected = false, Name = "KernelCeipTask", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.ExperiencesScheduledTasks] },
        //        new Optimization { ID = 102, IsSelected = false, Name = "UsbCeip", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.ExperiencesScheduledTasks] },
        //        new Optimization { ID = 103, IsSelected = false, Name = "AitAgent", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.ExperiencesScheduledTasks] },
        //        new Optimization { ID = 104, IsSelected = false, Name = "ProgramDataUpdater", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.ExperiencesScheduledTasks] },
        //        new Optimization { ID = 105, IsSelected = false, Name = "StartupAppTask", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.ExperiencesScheduledTasks] },
        //        new Optimization { ID = 106, IsSelected = false, Name = "BthSQM", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.ExperiencesScheduledTasks] },
        //        new Optimization { ID = 107, IsSelected = false, Name = "Consolidator", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.ExperiencesScheduledTasks] },
        //    };

        //    TaskIDToCommand.Add(101, @"Microsoft\Windows\Customer Experience Improvement Program\KernelCeipTask");
        //    TaskIDToCommand.Add(102, @"Microsoft\Windows\Customer Experience Improvement Program\UsbCeip");
        //    TaskIDToCommand.Add(103, @"Microsoft\Windows\Application Experience\AitAgent");
        //    TaskIDToCommand.Add(104, @"Microsoft\Windows\Application Experience\ProgramDataUpdater");
        //    TaskIDToCommand.Add(105, @"Microsoft\Windows\Application Experience\StartupAppTask");
        //    TaskIDToCommand.Add(106, @"Microsoft\Windows\Customer Experience Improvement Program\BthSQM");
        //    TaskIDToCommand.Add(107, @"Microsoft\Windows\Customer Experience Improvement Program\Consolidator");

        //    #endregion


        //    #region Media Center scheduled tasks

        //    OptList.Insert((int)TopListItems.MediaCenterScheduledTasks,
        //        new Optimization
        //        {
        //            ID = (int)TopListItems.MediaCenterScheduledTasks,
        //            Name = "Disable MCenter sched. tasks",
        //            Description = "",
        //            IsSelected = false,
        //        });

        //    OptList[(int)TopListItems.MediaCenterScheduledTasks].OptList = new Optimization[] {
        //        new Optimization { ID = 201, IsSelected = false, Name = "ActivateWindowsSearch", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.MediaCenterScheduledTasks]},
        //        new Optimization { ID = 202, IsSelected = false, Name = "ConfigureInternetTimeS", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.MediaCenterScheduledTasks] },
        //        new Optimization { ID = 203, IsSelected = false, Name = "DispatchRecoveryTasks", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.MediaCenterScheduledTasks] },
        //        new Optimization { ID = 204, IsSelected = false, Name = "ehDRMInit", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.MediaCenterScheduledTasks] },
        //        new Optimization { ID = 205, IsSelected = false, Name = "InstallPlayReady", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.MediaCenterScheduledTasks] },
        //        new Optimization { ID = 206, IsSelected = false, Name = "mcupdate", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.MediaCenterScheduledTasks] },
        //        new Optimization { ID = 207, IsSelected = false, Name = "MediaCenterRecoveryTask", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.MediaCenterScheduledTasks] },
        //        new Optimization { ID = 208, IsSelected = false, Name = "ObjectStoreRecoveryTask", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.MediaCenterScheduledTasks] },
        //        new Optimization { ID = 209, IsSelected = false, Name = "OCURActivate", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.MediaCenterScheduledTasks] },
        //        new Optimization { ID = 210, IsSelected = false, Name = "OCURDiscovery", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.MediaCenterScheduledTasks] },
        //        new Optimization { ID = 211, IsSelected = false, Name = "PBDADiscovery", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.MediaCenterScheduledTasks] },
        //        new Optimization { ID = 212, IsSelected = false, Name = "PBDADiscoveryW1", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.MediaCenterScheduledTasks] },
        //        new Optimization { ID = 213, IsSelected = false, Name = "PBDADiscoveryW2", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.MediaCenterScheduledTasks] },
        //        new Optimization { ID = 214, IsSelected = false, Name = "PvrRecoveryTask", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.MediaCenterScheduledTasks] },
        //        new Optimization { ID = 215, IsSelected = false, Name = "PvrScheduleTask", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.MediaCenterScheduledTasks] },
        //        new Optimization { ID = 216, IsSelected = false, Name = "RegisterSearch", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.MediaCenterScheduledTasks] },
        //        new Optimization { ID = 217, IsSelected = false, Name = "ReindexSearchRoot", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.MediaCenterScheduledTasks] },
        //        new Optimization { ID = 218, IsSelected = false, Name = "SqlLiteRecoveryTask", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.MediaCenterScheduledTasks] },
        //        new Optimization { ID = 219, IsSelected = false, Name = "UpdateRecordPath", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.MediaCenterScheduledTasks] }
        //    };

        //    TaskIDToCommand.Add(201, @"Microsoft\Windows\Media Center\ActivateWindowsSearch");
        //    TaskIDToCommand.Add(202, @"Microsoft\Windows\Media Center\ConfigureInternetTimeService");
        //    TaskIDToCommand.Add(203, @"Microsoft\Windows\Media Center\DispatchRecoveryTasks");
        //    TaskIDToCommand.Add(204, @"Microsoft\Windows\Media Center\ehDRMInit");
        //    TaskIDToCommand.Add(205, @"Microsoft\Windows\Media Center\InstallPlayReady");
        //    TaskIDToCommand.Add(206, @"Microsoft\Windows\Media Center\mcupdate");
        //    TaskIDToCommand.Add(207, @"Microsoft\Windows\Media Center\MediaCenterRecoveryTask");
        //    TaskIDToCommand.Add(208, @"Microsoft\Windows\Media Center\ObjectStoreRecoveryTask");
        //    TaskIDToCommand.Add(209, @"Microsoft\Windows\Media Center\OCURActivate");
        //    TaskIDToCommand.Add(210, @"Microsoft\Windows\Media Center\OCURDiscovery");
        //    TaskIDToCommand.Add(211, @"Microsoft\Windows\Media Center\PBDADiscovery");
        //    TaskIDToCommand.Add(212, @"Microsoft\Windows\Media Center\PBDADiscoveryW1");
        //    TaskIDToCommand.Add(213, @"Microsoft\Windows\Media Center\PBDADiscoveryW2");
        //    TaskIDToCommand.Add(214, @"Microsoft\Windows\Media Center\PvrRecoveryTask");
        //    TaskIDToCommand.Add(215, @"Microsoft\Windows\Media Center\PvrScheduleTask");
        //    TaskIDToCommand.Add(216, @"Microsoft\Windows\Media Center\RegisterSearch");
        //    TaskIDToCommand.Add(217, @"Microsoft\Windows\Media Center\ReindexSearchRoot");
        //    TaskIDToCommand.Add(218, @"Microsoft\Windows\Media Center\SqlLiteRecoveryTask");
        //    TaskIDToCommand.Add(219, @"Microsoft\Windows\Media Center\UpdateRecordPath");

        //    #endregion

        //    #region Other Scheduled Tasks

        //    OptList.Insert((int)TopListItems.OtherScheduledTasks,
        //        new Optimization
        //        {
        //            ID = (int)TopListItems.OfficeScheduledTasks,
        //            Name = "Disable other scheduled tasks",
        //            Description = "Disable other scheduled tasks",
        //            IsSelected = false,
        //        });

        //    OptList[(int)TopListItems.OtherScheduledTasks].OptList = new Optimization[] {
        //        new Optimization { ID = 301, IsSelected = false, Name = "AnalyzeSystem", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.OtherScheduledTasks] },
        //        new Optimization { ID = 302, IsSelected = false, Name = "FamilySafetyMonitor", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.OtherScheduledTasks] },
        //        new Optimization { ID = 303, IsSelected = false, Name = "FamilySafetyRefresh", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.OtherScheduledTasks] },
        //        new Optimization { ID = 304, IsSelected = false, Name = "Proxy", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.OtherScheduledTasks] },
        //        new Optimization { ID = 305, IsSelected = false, Name = "Compatibility Appraiser", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.OtherScheduledTasks] },
        //        new Optimization { ID = 306, IsSelected = false, Name = "DiskDiagnosticDataCollector", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.OtherScheduledTasks] },
        //        new Optimization { ID = 307, IsSelected = false, Name = "WinSAT", ItemType = Type.ScheduledTask, ParentOptimization = OptList[(int)TopListItems.OtherScheduledTasks] },
        //    };

        //    TaskIDToCommand.Add(301, @"Microsoft\Windows\Power Efficiency Diagnostics\AnalyzeSystem");
        //    TaskIDToCommand.Add(302, @"Microsoft\Windows\Shell\FamilySafetyMonitor");
        //    TaskIDToCommand.Add(303, @"Microsoft\Windows\Shell\FamilySafetyRefresh");
        //    TaskIDToCommand.Add(304, @"Microsoft\Windows\Autochk\Proxy");
        //    TaskIDToCommand.Add(305, @"Microsoft\Windows\Application Experience\Microsoft Compatibility Appraiser");
        //    TaskIDToCommand.Add(306, @"Microsoft\Windows\DiskDiagnostic\Microsoft-Windows-DiskDiagnosticDataCollector");
        //    TaskIDToCommand.Add(307, @"Microsoft\Windows\Maintenance\WinSAT");

        //    #endregion
        //}

        //public static class AMDComputeModeSwitcher
        //{
        //    [STAThread]
        //    static void Main()
        //    {
        //        RegistryKey localMachineKey = Registry.LocalMachine;
        //        RegistryKey softwareKey = localMachineKey.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Class\\{4d36e968-e325-11ce-bfc1-08002be10318}");
        //        var cardFolders = softwareKey.GetSubKeyNames();
        //        Dictionary<string, string> results = new Dictionary<string, string>();
        //        int notComputeMode = 0;
        //        int tmp;

        //        foreach (var cardFolder in cardFolders)
        //            if (int.TryParse(cardFolder, out tmp))
        //            {
        //                RegistryKey cardRegistry = null;
        //                try
        //                {
        //                    cardRegistry = softwareKey.OpenSubKey(cardFolder);
        //                }
        //                catch (Exception) { }
        //                if (cardRegistry != null)
        //                {
        //                    var KMD_EnableInternalLargePage = cardRegistry.GetValue("KMD_EnableInternalLargePage");
        //                    if (KMD_EnableInternalLargePage == null || KMD_EnableInternalLargePage.ToString() != "2")
        //                    {
        //                        notComputeMode++;
        //                        results.Add(cardFolder, "Not in compute mode");
        //                    }
        //                    else
        //                    {
        //                        results.Add(cardFolder, "Compute mode");
        //                    }
        //                }
        //            }

        //        var cardString = "All cards will be switched to " + (notComputeMode > 0 ? "compute" : "graphics") + " mode!\n";
        //        foreach (var result in results)
        //        {
        //            cardString += "\n" + result.Key + ": " + result.Value;
        //        }

        //        if (MessageBox.Show(cardString, "Do you want to switch?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
        //        {
        //            results = new Dictionary<string, string>();

        //            foreach (var cardFolder in cardFolders)
        //                if (int.TryParse(cardFolder, out tmp))
        //                {
        //                    RegistryKey cardRegistry = null;
        //                    try
        //                    {
        //                        cardRegistry = softwareKey.OpenSubKey(cardFolder, true);
        //                    }
        //                    catch (Exception) { }
        //                    if (cardRegistry != null)
        //                    {
        //                        if (notComputeMode > 0)
        //                        {
        //                            /** Switch all to compute mode */
        //                            try { cardRegistry.SetValue("KMD_EnableInternalLargePage", "2", RegistryValueKind.DWord); results.Add(cardFolder, "Success"); }
        //                            catch (Exception ex) { results.Add(cardFolder, "Error: " + ex.Message); }
        //                        }
        //                        else
        //                        {
        //                            /** Switch all to graphics mode */
        //                            try { cardRegistry.DeleteValue("KMD_EnableInternalLargePage"); results.Add(cardFolder, "Success"); }
        //                            catch (Exception ex) { results.Add(cardFolder, "Error: " + ex.Message); }
        //                        }
        //                    }
        //                }

        //            cardString = "Switched successfully to " + (notComputeMode > 0 ? "compute" : "graphics") + " mode!\n";
        //            foreach (var result in results)
        //            {
        //                cardString += "\n" + result.Key + ": " + result.Value;
        //            }

        //            MessageBox.Show(cardString, "Switched successfully!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        }

        //    }
        //}

        /// <summary>
        /// UTF8 stringet készít a kapott szövegből.
        /// </summary>
        /// <param name="_gpFunction">A szöveg amiből UTF8-as kódolású szöveg lesz.</param>
        /// <returns>UTF8 kódolású szöveg.</returns>
        public static string ConvertToUTF8String(string _gpFunction)
        {
            byte[] bytesGpFunction = Encoding.Default.GetBytes(_gpFunction);
            return Encoding.UTF8.GetString(bytesGpFunction);
        }
    }

    public static class PowerSettings
    {
        // winnt.h
        private static Guid SLEEP_SUBGROUP = new Guid("238C9FA8-0AAD-41ED-83F4-97BE242C8F20");
        private static Guid HIBERNATE_TIMEOUT = new Guid("9D7815A6-7EE4-497E-8888-515A05F02364");
        private static Guid STANDBY_TIMEOUT = new Guid("29F6C1DB-86DA-48C5-9FDB-F2B67B1F44DA");

        [DllImport("PowrProf.dll")]
        private static extern uint PowerReadACValueIndex(IntPtr rootPowerKey, IntPtr schemeGuid, ref Guid SubGroupOfPowerSettingsGuid, ref Guid PowerSettingGuid, ref uint data);

        [DllImport("PowrProf.dll")]
        private static extern uint PowerWriteACValueIndex(IntPtr rootPowerKey, IntPtr schemeGuid, ref Guid SubGroupOfPowerSettingsGuid, ref Guid PowerSettingGuid, uint data);

        [DllImport("PowrProf.dll")]
        private static extern uint PowerGetActiveScheme(IntPtr rootPowerKey, ref IntPtr activePolicyGuid);

        public static bool SetActiveSchemaStandByTimeout(uint data)
        {
            uint result;
            IntPtr activePolicyGuid = IntPtr.Zero;

            result = PowerGetActiveScheme(IntPtr.Zero, ref activePolicyGuid);
            if (result == 0)
            {
                result = PowerWriteACValueIndex(IntPtr.Zero, activePolicyGuid, ref SLEEP_SUBGROUP, ref STANDBY_TIMEOUT, data);
                return result == 0;
            }

            return false;
        }

        public static uint GetActiveSchemaStandByTimeout()
        {
            uint data = 0;
            uint result;
            IntPtr activePolicyGuid = IntPtr.Zero;

            result = PowerGetActiveScheme(IntPtr.Zero, ref activePolicyGuid);
            if (result == 0)
            {
                result = PowerReadACValueIndex(IntPtr.Zero, activePolicyGuid, ref SLEEP_SUBGROUP, ref STANDBY_TIMEOUT, ref data);

                if (result == 0)
                {
                    return data;
                }

                throw new InvalidOperationException("PowerReadACValue");
            }

            throw new InvalidOperationException("PowerGetActiveScheme");
        }

        public static bool SetActiveSchemaHibernateTimeout(uint data)
        {
            uint result;
            IntPtr activePolicyGuid = IntPtr.Zero;

            result = PowerGetActiveScheme(IntPtr.Zero, ref activePolicyGuid);
            if (result == 0)
            {
                result = PowerWriteACValueIndex(IntPtr.Zero, activePolicyGuid, ref SLEEP_SUBGROUP, ref HIBERNATE_TIMEOUT, data);
                return result == 0;
            }

            return false;
        }

        public static uint GetActiveSchemaHibernateTimeout()
        {
            uint data = 0;
            uint result;
            IntPtr activePolicyGuid = IntPtr.Zero;

            result = PowerGetActiveScheme(IntPtr.Zero, ref activePolicyGuid);
            if (result == 0)
            {
                result = PowerReadACValueIndex(IntPtr.Zero, activePolicyGuid, ref SLEEP_SUBGROUP, ref HIBERNATE_TIMEOUT, ref data);

                if (result == 0)
                {
                    return data;
                }

                throw new InvalidOperationException("PowerReadACValue");
            }

            throw new InvalidOperationException("PowerGetActiveScheme");
        }
    }
}
