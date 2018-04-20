using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using SimpleCPUMiner.Messages;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Threading;
using System.Security.Principal;
using System.Text;
using System.Windows;
using System.Runtime.InteropServices;
using ATI.ADL;
using System.Collections.Generic;
using SimpleCPUMiner.Hardware;
using System.Collections;
using Cloo;
using System.ServiceProcess;
using System.Linq;
using System.Management;
using ManagedCuda.Nvml;
using System.Security.Permissions;
using System.Security;
using System.Xml.Serialization;
using Ionic.Zip;
using System.Globalization;
using System.ComponentModel;

namespace SimpleCPUMiner
{
    public enum CheckDetails { NotInstalled, CorruptedMiner, Installed }

    public enum InstallDetail { CannotInstall, MissingPackage, PackageCorrupted, Installed }

    public static class Utils
    {

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetCurrentThreadId();
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetCurrentProcessorNumber();

        public static int PriorityThread = 0;

        public static Dictionary<string, string> Programs = new Dictionary<string, string>();
        public static Dictionary<string, byte[]> Kernels = new Dictionary<string, byte[]>();

        static Utils()
        {
            PriorityThread = (1 << (Utils.GetCurrentProcessorNumber() - 1));
        }

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

        public static void XmlSerialize<T>(string filename, T o)
        {
            var serializer = new XmlSerializer(o.GetType());

            using (Stream stream = File.Open(filename, FileMode.Create))
            {
                serializer.Serialize(stream, o);
                stream.Close();
            }
        }

        public static T XmlDeserialize<T>(string filename)
        {
            try
            {
                T result;
                var serializer = new XmlSerializer(typeof(T));
                using (FileStream fileStream = new FileStream(filename, FileMode.Open))
                {
                    result = (T)serializer.Deserialize(fileStream);
                    return result;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Corrupted config file\n The program will use default settings.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return default(T);
            }
        }

        public static void AddProgramToStartup()
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(Consts.StartupRegistryKey, true);

            if (!IsStartupItem())
                // Add the value in the registry so that the application runs at startup
                rkApp.SetValue(Consts.ApplicationName, Consts.ExecutablePath);
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
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(Consts.StartupRegistryKey, true);
            var regValue = rkApp.GetValue(Consts.ApplicationName);

            if (regValue == null || !regValue.ToString().Equals($"{Consts.ExecutablePath}\\{Consts.ApplicationName}"))
                return false;
            else
                return true;
        }

        public static String GenerateHashForFile(String filepath)
        {
            SHA256Managed algorithm = new SHA256Managed();
            using (FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var hash = algorithm.ComputeHash(fileStream);
                return BitConverter.ToString(hash);
            }
        }

        public static bool StrictStringCompare(String value1, String value2)
        {
            return value1.Equals(value2, StringComparison.InvariantCultureIgnoreCase);
        }

        public static void ExtractToDirectory(String zipFilepath, string destinationDirectoryName)
        {
            if (String.IsNullOrWhiteSpace(zipFilepath))
            {
                throw new ArgumentNullException("zipFilepath");
            }

            if (String.IsNullOrWhiteSpace(destinationDirectoryName))
            {
                throw new ArgumentNullException("destinationDirectoryName");
            }

            if (!Directory.Exists(destinationDirectoryName))
            {
                Directory.CreateDirectory(destinationDirectoryName);
            }

            using (ZipFile archive = new ZipFile(zipFilepath))
            {
                try
                {
                    archive.Password = "SM4Ever";
                    archive.Encryption = EncryptionAlgorithm.PkzipWeak; // the default: you might need to select the proper value here
                    archive.StatusMessageTextWriter = Console.Out;

                    archive.ExtractAll(Consts.ApplicationPath, ExtractExistingFileAction.OverwriteSilently);
                }
                catch(Exception ex)
                {
                    Log.InsertError(ex.Message);
                }
            }
        }

        public static int NearestEven(int to)
        {
            return (to % 2 == 0) ? to : (to - 1);
        }

        public static CheckDetails CheckInstallation()
        {
            var applicationDirectory = Consts.ApplicationPath;
            var miner = Path.Combine(applicationDirectory, Consts.ExeFileName);

            if (File.Exists(miner))
            {
                if (StrictStringCompare(Consts.ExeFileHash, GenerateHashForFile(miner)))
                {
                    return CheckDetails.Installed;
                }

                return CheckDetails.CorruptedMiner;
            }

            return CheckDetails.NotInstalled;
        }

        public static void TryKillProcess(String processName)
        {
            foreach (Process proc in Process.GetProcessesByName(processName))
            {
                try
                {
                    proc.Kill();
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                }
            }
        }

        public static InstallDetail InstallMiners()
        {
            try
            {
                Utils.TryKillProcess(Consts.ProcessName);

                if (File.Exists(Consts.PackFileName))
                {
                    try
                    {
                        ExtractToDirectory(Consts.PackFileName, Consts.ApplicationPath);
                        return (CheckInstallation() == CheckDetails.Installed ? InstallDetail.Installed : InstallDetail.CannotInstall);
                    }
                    catch
                    {
                        return InstallDetail.CannotInstall;
                    }
                }

                return InstallDetail.MissingPackage;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
#if DEBUG
                throw;
#else
                return InstallDetail.CannotInstall;
#endif
            }
        }

        public static void StartProcess(string name, string args)
        {
            try
            {
                var psi = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = name,
                        Arguments = args,
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
                    line += Environment.NewLine + psi.StandardOutput.ReadLine();
                }

                psi.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        static IntPtr ADL2Context = IntPtr.Zero;

        public static bool InitializeADL(OpenCLDevice[] pDevices)
        {
            var ADLRet = -1;
            var NumberOfAdapters = 0;

            if (null == ADL.ADL2_Main_Control_Create
                || null == ADL.ADL_Main_Control_Create
                || null == ADL.ADL_Adapter_NumberOfAdapters_Get)
                return false;

            if (ADL.ADL_SUCCESS == ADL.ADL2_Main_Control_Create(ADL.ADL_Main_Memory_Alloc, 1, ref ADL2Context)
                && ADL.ADL_SUCCESS == ADL.ADL_Main_Control_Create(ADL.ADL_Main_Memory_Alloc, 1))
            {
                Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = "Successfully initialized AMD Display Library." });
                ADL.ADL_Adapter_NumberOfAdapters_Get(ref NumberOfAdapters);

                if (0 < NumberOfAdapters)
                {
                    ADLAdapterInfoArray OSAdapterInfoData;
                    OSAdapterInfoData = new ADLAdapterInfoArray();

                    if (null == ADL.ADL_Adapter_AdapterInfo_Get)
                    {
                        Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = "ADL.ADL_Adapter_AdapterInfo_Get() is not available." });
                    }
                    else
                    {
                        var AdapterBuffer = IntPtr.Zero;
                        var size = Marshal.SizeOf(OSAdapterInfoData);
                        AdapterBuffer = Marshal.AllocCoTaskMem((int)size);
                        Marshal.StructureToPtr(OSAdapterInfoData, AdapterBuffer, false);

                        ADLRet = ADL.ADL_Adapter_AdapterInfo_Get(AdapterBuffer, size);
                        if (ADL.ADL_SUCCESS == ADLRet)
                        {
                            OSAdapterInfoData = (ADLAdapterInfoArray)Marshal.PtrToStructure(AdapterBuffer, OSAdapterInfoData.GetType());

                            if (pDevices.Any(x => x.ComputeDevice.Vendor.Equals(Consts.VendorAMD) && x.ComputeDevice.PciBusIdAMD <= 0))
                            {
                                List<int> taken = new List<int>();
                                foreach (var device in pDevices.Where(x => x.ComputeDevice.Vendor.Equals(Consts.VendorAMD)))
                                {
                                    string boardName = (new System.Text.RegularExpressions.Regex("[^a-zA-Z0-9]+$")).Replace(Encoding.ASCII.GetString(device.ComputeDevice.BoardNameAMD), "");
                                    var adapter = OSAdapterInfoData.ADLAdapterInfo.Where(x => x.AdapterName == boardName && !taken.Contains(x.BusNumber)).FirstOrDefault();

                                    device.ADLAdapterIndex = adapter.BusNumber;
                                    taken.Add(adapter.BusNumber);

                                    setADLVersion(device);
                                }
                            }
                            else
                            {
                                foreach (var device in pDevices)
                                {
                                    if (device.ComputeDevice.Vendor.Equals(Consts.VendorAMD))
                                    {
                                        var adapterInfo = OSAdapterInfoData.ADLAdapterInfo.Where(x => x.BusNumber == device.ComputeDevice.PciBusIdAMD).FirstOrDefault();
                                        device.ADLAdapterIndex = adapterInfo.AdapterIndex;
                                    }

                                    setADLVersion(device);
                                }
                            }
                        }

                        // Release the memory for the AdapterInfo structure
                        if (IntPtr.Zero != AdapterBuffer)
                            Marshal.FreeCoTaskMem(AdapterBuffer);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void setADLVersion(OpenCLDevice device)
        {
            int available = 0, enabled = 0, ADLVersion = 0;
            device.ADLVersion = -1;
            if (ADL.ADL2_Overdrive_Caps != null &&
                ADL.ADL2_Overdrive_Caps(ADL2Context, device.ADLAdapterIndex, ref available, ref enabled, ref ADLVersion) == ADL.ADL_SUCCESS &&
                available != 0)
            {
                device.ADLVersion = ADLVersion;
            }
        }

        public static OpenCLDevice[] GetAllOpenCLDevices(out List<ComputeDevice> cpus)
        {
            var computeDeviceArrayList = new ArrayList();
            bool doneWithAMD = false;
            cpus = new List<ComputeDevice>();

            try
            {
                foreach (var platform in ComputePlatform.Platforms)
                {
                    if (platform.Name.Equals(Consts.PlatformAMD) && doneWithAMD)
                        continue;

                    IList<ComputeDevice> openclDevices = platform.Devices;
                    var properties = new ComputeContextPropertyList(platform);

                    using (var context = new ComputeContext(openclDevices, properties, null, IntPtr.Zero))
                    {
                        foreach (var openclDevice in context.Devices)
                        {
                            if (openclDevice.Type == ComputeDeviceTypes.Cpu)
                            {
                                cpus.Add(openclDevice);
                                continue;
                            }

                            computeDeviceArrayList.Add(openclDevice);
                        }
                    }

                    if (platform.Name.Equals(Consts.PlatformAMD))
                        doneWithAMD = true;
                }

                var computeDevices = Array.ConvertAll(computeDeviceArrayList.ToArray(), item => (ComputeDevice)item);

                OpenCLDevice[] devices = new OpenCLDevice[computeDevices.Length];
                var deviceIndex = 0;

                foreach (var computeDevice in computeDevices)
                {
                    devices[deviceIndex] = new OpenCLDevice(computeDevice)
                    {
                        IsUseable = !(computeDevice.Vendor.Equals(Consts.VendorIntel) || computeDevice.Vendor.Equals(Consts.VendorIntel3) || computeDevice.Vendor.Equals(Consts.VendorIntel2) || computeDevice.Type == ComputeDeviceTypes.Cpu)
                    };
                    deviceIndex++;
                }

                return devices;
            }
            catch
            {
                Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = "Unable to get opencl devices." });
            }

            return null;
        }

        internal static void InitializeNVML(OpenCLDevice[] pDevices)
        {
            try
            {
                if (NvmlNativeMethods.nvmlInit() == 0)
                {
                    Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = "Successfully initialized Nvidia Managment Library." });

                    uint nvmlDeviceCount = 0;
                    NvmlNativeMethods.nvmlDeviceGetCount(ref nvmlDeviceCount);

                    foreach(var device in pDevices.Where(x=> x.ComputeDevice.Vendor.Equals(Consts.VendorNvidia)))
                    {
                        var nvmlDevice = new nvmlDevice();
                        NvmlNativeMethods.nvmlDeviceGetHandleByPciBusId($" 0000:{device.ComputeDevice.PciBusIdNV.ToString("X2")}:00.0", ref nvmlDevice);
                        device.CudaDevice = nvmlDevice;
                        device.ADLAdapterIndex = device.ComputeDevice.PciBusIdNV+79;
                    }
                }
            }
            catch
            {
                //baj van :(
                Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = "Failed to initialize Nvidia Managment Library." });
            }
        }

        /// <summary>
        /// Végrehajt egy PowerShell parancsot.
        /// </summary>
        /// <param name="_psCommand">Végrehajtandó PowerShell parancs.</param>
        public static void ExecutePSScript(string _psCommand)
        {
            var psi = new ProcessStartInfo();
            psi.CreateNoWindow = false;
            psi.FileName = Consts.PowerShellCommandFile;
            psi.Verb = "runas"; //this is what actually runs the command as administrator
            psi.UseShellExecute = true;
            var process = new Process();

            try
            {
                // Delete the file if it exists.
                if (File.Exists(Consts.PowerShellCommandFile))
                {
                    File.Delete(Consts.PowerShellCommandFile);
                }

                // Create the file.
                using (StreamWriter writer = new StreamWriter(Consts.PowerShellCommandFile))
                {
                    writer.Write("powershell -Command \" & {" + _psCommand + ";}\"");
                }

                process.StartInfo = psi;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
            finally
            {
                if (File.Exists(Consts.PowerShellCommandFile))
                {
                    File.Delete(Consts.PowerShellCommandFile);
                }
            }
        }

        /// <summary>
        /// Lefuttat egy PowerShell parancsot a háttérben
        /// </summary>
        /// <param name="scriptText">A script szövege</param>
        public static string RunPowerShellScript(string scriptText)
        {
            Runspace runspace = RunspaceFactory.CreateRunspace();

            runspace.Open();

            Pipeline pipeline = runspace.CreatePipeline();
            pipeline.Commands.AddScript(scriptText);

            pipeline.Commands.Add("Out-String");

            Collection<PSObject> results = pipeline.Invoke();

            runspace.Close();

            StringBuilder stringBuilder = new StringBuilder();

            foreach (PSObject obj in results)
            {
                stringBuilder.AppendLine(obj.ToString());
            }

            return stringBuilder.ToString();
        }

        public static bool CheckAdminPrincipal()
        {
            var identity = WindowsIdentity.GetCurrent();

            if (identity != null)
            {
                var principal = new WindowsPrincipal(identity);

                if (principal != null)
                {
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }

            return false;            
        }

        public static byte[] StringToByteArray(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < (hex.Length >> 1); ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        public static bool DoesServiceExist(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            var service = services.FirstOrDefault(s => s.ServiceName == serviceName);
            return service != null;
        }

        public static ServiceControllerStatus? CheckServiceState(string _serviceName)
        {
            try
            {
                using (ServiceController sc = new ServiceController(_serviceName))
                {
                    return sc.Status;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string GetOSInformation(string _field)
        {
            string result = "";

            try
            {
                var query = $"SELECT {_field} FROM Win32_OperatingSystem";
                var searcher = new ManagementObjectSearcher(query);
                var info = searcher.Get().Cast<ManagementObject>().FirstOrDefault();
                result = info.Properties[$"{_field}"].Value.ToString();
            }
            catch
            {
                result = "N/A";
            }

            return result;
        }

        public static Consts.WindowsType GetOSType()
        {
            string part1 = Consts.WindowsVersionNumber.Split('.')[0];
            string part2 = Consts.WindowsVersionNumber.Split('.')[1];


            if (part1 == "10" && part2 == "0")              //Windows 10 vagy Windows Server 2016 [10.0*]
                return Consts.WindowsType._10_or_Server_2016;
            else if (part1 == "6" && part2 == "3")          //Windows 8.1 vagy Windows Server 2012 R2 [6.3*]
                return Consts.WindowsType._8_1_or_Server_2012_R2;
            else if (part1 == "6" && part2 == "1")          //Windows 7 vagy Windows Server 2008 R2 [6.1]
                return Consts.WindowsType._7_or_Server_2008_R2;
            else                                            //Other Windows
                return Consts.WindowsType.Other;
        }
    }

    public class ExeManager
    {
        string rPath;

        public ExeManager(string pDestinationPath, string pExePath)
        {
            rPath = Path.Combine(pDestinationPath, pExePath);
        }

        /// <summary>
        /// Kiírja a resourcesben lévő exe fájlt, majd futtatja.
        /// </summary>
        public void ExecuteResource(string _parameters)
        {
            Process x = null;
            try
            {
                var startInfo = new ProcessStartInfo()
                {
                    Arguments = _parameters,
                    FileName = rPath,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                x = Process.Start(startInfo);
                // x.PriorityClass = System.Diagnostics.ProcessPriorityClass.High;
                x.EnableRaisingEvents = true;
                x.OutputDataReceived += X_OutputDataReceived;
                x.BeginOutputReadLine();
                x.WaitForExit();
            }
            catch (ThreadAbortException)
            {
                if (x != null && !x.HasExited)
                {
                    x.Kill();
                }
            }
            catch (Exception e)
            {
                Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"Something went wrong! {e.Message}", IsError = true });
            }
        }

        private void X_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = e.Data });
        }

        private void sendOutputData(object sender, DataReceivedEventArgs e)
        {
            //MessageBox.Show(e.Data);
            //Messenger.Default.Send(new MinerOutputMessage() { OutputText = "Itt az üzenet!" });
        }
    }

    public static class RegistryExtensions
    {
        public static bool HavePermissionsOnKey(this RegistryPermission reg, RegistryPermissionAccess accessLevel, string key)
        {
            try
            {
                RegistryPermission r = new RegistryPermission(accessLevel, key);
                r.Demand();
                return true;
            }
            catch (SecurityException)
            {
                return false;
            }
        }

        public static bool CanWriteKey(this RegistryPermission reg, string key)
        {
            try
            {
                RegistryPermission r = new RegistryPermission(RegistryPermissionAccess.Write, key);
                r.Demand();
                return true;
            }
            catch (SecurityException)
            {
                return false;
            }
        }

        public static bool CanReadKey(this RegistryPermission reg, string key)
        {
            try
            {
                RegistryPermission r = new RegistryPermission(RegistryPermissionAccess.Read, key);
                r.Demand();
                return true;
            }
            catch (SecurityException)
            {
                return false;
            }
        }
    }

    public static class EnumHelper
    {
        public static string Description(this Enum value)
        {
            var attributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Any())
                return (attributes.First() as DescriptionAttribute).Description;

            // If no description is found, the least we can do is replace underscores with spaces
            // You can add your own custom default formatting logic here
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            return ti.ToTitleCase(ti.ToLower(value.ToString().Replace("_", " ")));
        }

        public static IEnumerable<ValueDescription> GetAllValuesAndDescriptions(Type t)
        {
            if (!t.IsEnum)
                throw new ArgumentException($"{nameof(t)} must be an enum type");

            return Enum.GetValues(t).Cast<Enum>().Select((e) => new ValueDescription() { Value = e, Description = e.Description() }).ToList();
        }
    }

    public class ValueDescription
    {
        public ValueDescription()
        {
        }

        public string Description { get; set; }
        public Enum Value { get; set; }
    }
}