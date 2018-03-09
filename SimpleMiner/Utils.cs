using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Windows;

namespace SimpleCPUMiner
{
    public enum CheckDetails { NotInstalled, CorruptedMiner, Installed }

    public enum InstallDetail { CannotInstall, MissingPackage, PackageCorrupted, Installed }

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


        public static void AddProgramToStartup()
        {
            //RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(Consts.StartupRegistryKey, true);
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

            using (FileStream archiveFileStream = new FileStream(zipFilepath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ZipArchive archive = new ZipArchive(archiveFileStream);

                foreach (ZipArchiveEntry file in archive.Entries)
                {
                    string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
                    string directory = Path.GetDirectoryName(completeFileName);

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    if (file.Name != "")
                    {
                        file.ExtractToFile(completeFileName, true);
                    }
                }
            }
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
                catch(Exception exception)
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
    }
}
