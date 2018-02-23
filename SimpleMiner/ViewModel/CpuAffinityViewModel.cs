using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using System.Management;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using SimpleCPUMiner.Messages;

namespace SimpleCPUMiner.ViewModel
{
    public class CpuAffinityViewModel : ViewModelBase
    {
        public String ThreadNumber { get; set; }
        public string CpuAffinityOut { get; set; }
        public string CpuAffinityCalc { get; set; }
        public ObservableCollection<CpuInfo> CpuList { get; set; }
        public RelayCommand<Window> CloseWindowCommand { get; private set; }
        public RelayCommand<Window> ApplyCommand { get; private set; }

        public CpuAffinityViewModel()
        {
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow);
            ApplyCommand = new RelayCommand<Window>(ApplyAffinity);
            CpuList = new ObservableCollection<CpuInfo>();
            GetCpuInfo();
        }

        private void ApplyAffinity(Window window)
        {
            var msg = new CpuAffinityMessage() { CpuAffinity = CpuAffinityCalc };
            Messenger.Default.Send<CpuAffinityMessage>(msg);
            CloseWindow(window);
        }

        public void CalcCpuAffinity()
        {
            if (CpuList != null && CpuList.Count > 0)
            {
                char[] bin = new char[99];
                for (int x = 0; x < bin.Length; x++)
                    bin[x] = '0';

                int corePerCpu = (int)CpuList[0].Cores;

                if (!ThreadNumber.Equals("0"))
                {
                    int threadCount = int.Parse(ThreadNumber);

                    for (int i = 0; i < threadCount; i += 2)
                        for (int k = 0; k < CpuList.Count; k++)
                            bin[i + k * corePerCpu] = '1';
                }
                else
                {
                    int threadCount = 0;
                    foreach (var cpu in CpuList)
                    {
                        threadCount += (int)((cpu.L2Cache + cpu.L3Cache) / 2);
                    }

                    for (int i = 0; i < threadCount; i += 2)
                        for (int k = 0; k < CpuList.Count; k++)
                            bin[i + k * corePerCpu] = '1';

                }
                CpuAffinityCalc = $"0x{Convert.ToInt32(new string(bin.Reverse().ToArray()), 2).ToString("X")}{Convert.ToInt32(new string(bin.Reverse().ToArray()), 2).ToString("X")}";
                CpuAffinityOut = $"Your CPU affinity value is: {CpuAffinityCalc}";
                RaisePropertyChanged(nameof(CpuAffinityOut));
            }
        }

        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

        public class CpuInfo
        {
            public string ID { get; internal set; }
            public string Name { get; internal set; }
            public string Description { get; internal set; }
            public uint SpeedMHz { get; internal set; }
            public ushort DataWidth { get; internal set; }
            public ushort AddressWidth { get; internal set; }
            public string Socket { get; internal set; }
            public uint BusSpeedMHz { get; internal set; }
            public ulong L2Cache { get; internal set; }
            public ulong L3Cache { get; internal set; }
            public uint Cores { get; internal set; }
            public uint Threads { get; internal set; }
        }

        private void GetCpuInfo()
        {
            try
            {
                var cpuList = new ManagementObjectSearcher("select * from Win32_Processor").Get().Cast<ManagementObject>();
                if (cpuList != null)
                {
                    foreach (var cpu in cpuList)
                    {
                        var CPU = new CpuInfo();
                        CPU.ID = (string)cpu["ProcessorId"];
                        CPU.Socket = (string)cpu["SocketDesignation"];
                        CPU.Name = (string)cpu["Name"];
                        CPU.Description = (string)cpu["Caption"];
                        CPU.AddressWidth = (ushort)cpu["AddressWidth"];
                        CPU.DataWidth = (ushort)cpu["DataWidth"];
                        CPU.SpeedMHz = (uint)cpu["MaxClockSpeed"];
                        CPU.BusSpeedMHz = (uint)cpu["ExtClock"];
                        CPU.L2Cache = (uint)cpu["L2CacheSize"] / 1024;
                        CPU.L3Cache = (uint)cpu["L3CacheSize"] / 1024;
                        CPU.Cores = (uint)cpu["NumberOfCores"];
                        CPU.Threads = (uint)cpu["NumberOfLogicalProcessors"];

                        CPU.Name =
                            CPU.Name
                            .Replace("(TM)", "™")
                            .Replace("(tm)", "™")
                            .Replace("(R)", "®")
                            .Replace("(r)", "®")
                            .Replace("(C)", "©")
                            .Replace("(c)", "©")
                            .Replace("    ", " ")
                            .Replace("  ", " ");

                        CpuList.Add(CPU);
                    }
                }
                else
                {
                    Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = "Unable to detect installed CPU. (maybe VM in use)" });
                }
            }
            catch
            {
                Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = "Unable to detect installed CPU. (maybe VM in use)" });
            }
        }
    }
}
