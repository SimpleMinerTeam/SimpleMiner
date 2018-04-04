using SimpleCPUMiner.Hardware;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCPUMiner
{
    public static class GpuParameterHandler
    {
        private static string[] parametersToExclude = {
            "ComputeDevice", "SharesAccepted", "SharesRejected", "IsUseable", "Speed", "ComputeDeviceList",
            "Context","Temperature","FanSpeed","Activity", "Shares"
        };

        public static bool ReadParameters(OpenCLDevice[] devices)
        {
            if (devices == null || devices.Length == 0)
                return false;

            if(File.Exists(Consts.GpuParameterFile))
            {
                using (StreamReader streamReader = File.OpenText(Consts.GpuParameterFile))
                {
                    string str = streamReader.ReadLine();
                    foreach (var device in devices)
                    {
                        while ((str = streamReader.ReadLine()) != null)
                        {
                            if (str.Contains("|"))
                                break;

                            string[] strArray = str.Split('=');
                            SetPropertyValue(strArray[0], strArray[1], device);
                        }
                    }
                }
                return true;
            }

            return false;
        }

        public static void WriteParameters(OpenCLDevice[] devices)
        {
            if (devices == null || devices.Length == 0)
                return;

            using (StreamWriter text = File.CreateText(Consts.GpuParameterFile))
            {
                foreach (var device in devices)
                {
                    text.WriteLine("|");
                    foreach (PropertyInfo property in device.GetType().GetProperties())
                        if (!parametersToExclude.Any(x => x.Equals(property.Name)))
                            text.WriteLine(property.Name + "=" + property.GetValue(device, null));
                }
            }
        }

        private static void SetPropertyValue(string name, string value, OpenCLDevice device)
        {
            foreach (PropertyInfo property in device.GetType().GetProperties())
            {
                try
                {
                    if (property.Name == name)
                    {
                        if (name.Equals(nameof(device.Name)))
                        {
                            if (!device.Name.Equals(value))
                                break;
                        }

                        if (name.Equals(nameof(device.ADLAdapterIndex)))
                            break;

                        if (property.PropertyType == typeof(bool))
                        {
                            if (value == "False")
                            {
                                property.SetValue(device, false, null);
                                break;
                            }
                            property.SetValue(device, true, null);
                            break;
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(device, value, null);
                            break;
                        }
                        else if (property.PropertyType == typeof(int))
                        {
                            property.SetValue(device, Convert.ToInt32(value), null);
                            break;
                        }
                        else if (property.PropertyType == typeof(byte))
                        {
                            property.SetValue(device, Convert.ToByte(value), null);
                            break;
                        }
                        else if (property.PropertyType == typeof(double))
                        {
                            property.SetValue(device, Convert.ToDouble(value), null);
                            break;
                        }
                        property.SetValue(device, Convert.ToInt32(value), null);
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
