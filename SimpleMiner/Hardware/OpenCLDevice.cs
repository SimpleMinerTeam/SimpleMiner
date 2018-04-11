using ATI.ADL;
using Cloo;
using ManagedCuda.Nvml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace SimpleCPUMiner.Hardware
{
    public class OpenCLDevice
    {
        public ComputeDevice ComputeDevice { get; set; }

        public string Name { get; set; }
        public int ADLAdapterIndex { get; set; }
        public int ADLVersion { get; set; }
        public string Shares { get; set; }
        public int WorkSize { get; set; }
        public int Intensity { get; set; }
        public int Threads { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsUseable { get; set; }
        public double Speed { get; set; }
        public nvmlDevice CudaDevice { get; set; }
        private ComputeContext _context = null;
        private Mutex _mutex = new Mutex();
        public List<ComputeDevice> ComputeDeviceList { get; set; }
        PerformanceCounter _cpuCounter;
        private int _shareAccepted;
        private int _shareRejected;

        public int SharesAccepted
        {
            get
            {
                return _shareAccepted;
            }
            set
            {
                _shareAccepted = value;
                Shares = $"({SharesAccepted}/{SharesAccepted+SharesRejected})";
            }
        }

        public int SharesRejected
        {
            get
            {
                return _shareRejected;
            }
            set
            {
                _shareRejected = value;
                Shares = $"({SharesAccepted}/{SharesAccepted + SharesRejected})";
            }
        }

        public ComputeContext Context
        {
            get
            {
                try
                {
                    _mutex.WaitOne(5000);

                    if (_context == null)
                    {
                        ComputeDeviceList = new List<ComputeDevice>();
                        ComputeDeviceList.Add(ComputeDevice);
                        var contextProperties = new ComputeContextPropertyList(ComputeDevice.Platform);
                        _context = new ComputeContext(ComputeDeviceList, contextProperties, null, IntPtr.Zero);
                    }

                    _mutex.ReleaseMutex();
                }
                catch (Exception ex)
                {

                }

                return _context;
            }
        }

        public OpenCLDevice(ComputeDevice pDevice)
        {
            ADLAdapterIndex = -1;
            ComputeDevice = pDevice;
            if (ComputeDevice.Vendor.Equals(Consts.VendorAMD))
            {
                Name = System.Text.Encoding.ASCII.GetString(ComputeDevice.BoardNameAMD)
                    .Replace("AMD ", "")
                    .Replace("(TM)", "")
                    .Replace(" Series", "")
                    .Replace(" Graphics", "")
                    .Replace("  ", " ");
                Name = (new Regex("[^a-zA-Z0-9]+$")).Replace(Name, "");

                if (Name.Equals("Radeon HD 7700") && ComputeDevice.MaxComputeUnits == 6) { Name = "Radeon HD 7730"; }
                else if (Name.Equals("Radeon HD 7700") && ComputeDevice.MaxComputeUnits == 8) { Name = "Radeon HD 7750"; }
                else if (Name.Equals("Radeon HD 7700") && ComputeDevice.MaxComputeUnits == 10) { Name = "Radeon HD 7770"; }
                else if (Name.Equals("Radeon HD 7700") && ComputeDevice.MaxComputeUnits == 14) { Name = "Radeon HD 7790"; }
                else if (Name.Equals("Radeon HD 7800") && ComputeDevice.MaxComputeUnits == 16) { Name = "Radeon HD 7850"; }
                else if (Name.Equals("Radeon HD 7800") && ComputeDevice.MaxComputeUnits == 20) { Name = "Radeon HD 7870"; }
                else if (Name.Equals("Radeon HD 7800") && ComputeDevice.MaxComputeUnits == 24) { Name = "Radeon HD 7870 XT"; }
                else if (Name.Equals("Radeon HD 7900") && ComputeDevice.MaxComputeUnits == 28)
                {
                    Name = "Radeon HD 7950";
                    Threads = 2;
                    WorkSize = 8;
                    Intensity = 88;
                }
                else if (Name.Equals("Radeon HD 7900") && ComputeDevice.MaxComputeUnits == 32)
                {
                    Name = "Radeon HD 7970";
                    Threads = 2;
                    WorkSize = 8;
                    Intensity = 88;
                }
                else if (Name.Equals("Radeon R5 200") && ComputeDevice.MaxComputeUnits == 320 / 64) { Name = "Radeon R5 240"; }
                else if (Name.Equals("Radeon R7 200") && ComputeDevice.MaxComputeUnits == 320 / 64) { Name = "Radeon R7 240"; }
                else if (Name.Equals("Radeon R7 200") && ComputeDevice.MaxComputeUnits == 384 / 64) { Name = "Radeon R7 250"; }
                else if (Name.Equals("Radeon R7 200") && ComputeDevice.MaxComputeUnits == 512 / 64) { Name = "Radeon R7 250E"; }
                else if (Name.Equals("Radeon R7 200") && ComputeDevice.MaxComputeUnits == 640 / 64) { Name = "Radeon R7 250X"; }
                else if (Name.Equals("Radeon R7 200") && ComputeDevice.MaxComputeUnits == 768 / 64) { Name = "Radeon R7 260"; }
                else if (Name.Equals("Radeon R7 200") && ComputeDevice.MaxComputeUnits == 896 / 64) { Name = "Radeon R7 260X"; }
                else if (Name.Equals("Radeon R7 200") && ComputeDevice.MaxComputeUnits == 1024 / 64) { Name = "Radeon R7 265"; }
                else if (Name.Equals("Radeon R9 200") && ComputeDevice.MaxComputeUnits == 1280 / 64) { Name = "Radeon R9 270"; }
                else if (Name.Equals("Radeon R9 200") && ComputeDevice.MaxComputeUnits == 1280 / 64) { Name = "Radeon R9 270X"; }
                else if (Name.Equals("Radeon R9 200") && ComputeDevice.MaxComputeUnits == 1792 / 64)
                {
                    Name = "Radeon R9 280";
                    Threads = 2;
                    WorkSize = 8;
                    Intensity = 88;
                }
                else if (Name.Equals("Radeon R9 200") && ComputeDevice.MaxComputeUnits == 2048 / 64)
                {
                    Name = "Radeon R9 280X";
                    Threads = 2;
                    WorkSize = 8;
                    Intensity = 88;
                }
                else if (Name.Equals("Radeon R9 200") && ComputeDevice.MaxComputeUnits == 1792 / 64) { Name = "Radeon R9 285"; }
                else if (Name.Equals("Radeon R9 200") && ComputeDevice.MaxComputeUnits == 2560 / 64) { Name = "Radeon R9 290"; }
                else if (Name.Equals("Radeon R9 200") && ComputeDevice.MaxComputeUnits == 2816 / 64) { Name = "Radeon R9 290X"; }
                else if (Name.Equals("Radeon R5 300") && ComputeDevice.MaxComputeUnits == 320 / 64) { Name = "Radeon R5 330"; }
                else if (Name.Equals("Radeon R5 300") && ComputeDevice.MaxComputeUnits == 384 / 64) { Name = "Radeon R5 340"; }
                else if (Name.Equals("Radeon R7 300") && ComputeDevice.MaxComputeUnits == 384 / 64) { Name = "Radeon R7 340"; }
                else if (Name.Equals("Radeon R7 300") && ComputeDevice.MaxComputeUnits == 384 / 64) { Name = "Radeon R7 350"; }
                else if (Name.Equals("Radeon R7 300") && ComputeDevice.MaxComputeUnits == 512 / 64) { Name = "Radeon R7 350"; }
                else if (Name.Equals("Radeon R7 300") && ComputeDevice.MaxComputeUnits == 768 / 64) { Name = "Radeon R7 360"; }
                else if (Name.Equals("Radeon R9 300") && ComputeDevice.MaxComputeUnits == 768 / 64) { Name = "Radeon R9 360"; }
                else if (Name.Equals("Radeon R7 300") && ComputeDevice.MaxComputeUnits == 1024 / 64) { Name = "Radeon R7 370"; }
                else if (Name.Equals("Radeon R9 300") && ComputeDevice.MaxComputeUnits == 1024 / 64) { Name = "Radeon R9 370"; }
                else if (Name.Equals("Radeon R9 300") && ComputeDevice.MaxComputeUnits == 1280 / 64) { Name = "Radeon R9 370X"; }
                else if (Name.Equals("Radeon R9 300") && ComputeDevice.MaxComputeUnits == 1792 / 64)
                {
                    Name = "Radeon R9 380";
                    Threads = 2;
                    WorkSize = 8;
                    if((ComputeDevice.GlobalMemorySize / 1073741824)>3)
                        Intensity = 64;
                    else
                        Intensity = 54;
                }
                else if (Name.Equals("Radeon R9 300") && ComputeDevice.MaxComputeUnits == 2048 / 64) { Name = "Radeon R9 380X"; }
                else if (Name.Equals("Radeon R9 300") && ComputeDevice.MaxComputeUnits == 2560 / 64) { Name = "Radeon R9 390"; }
                else if (Name.Equals("Radeon R9 300") && ComputeDevice.MaxComputeUnits == 2816 / 64) { Name = "Radeon R9 390X"; }
                else if (Name.Equals("Radeon R9 Fury") && ComputeDevice.MaxComputeUnits == 3584 / 64) { Name = "Radeon R9 Fury"; }
                else if (Name.Equals("Radeon R9 Fury") && ComputeDevice.MaxComputeUnits == 4096 / 64) { Name = "Radeon R9 Nano"; }
                else if (Name.Equals("Radeon R9 Fury") && ComputeDevice.MaxComputeUnits == 4096 / 64) { Name = "Radeon R9 Fury X"; }
                else if (Name.Equals("687F:C3")) { Name = "RX Vega"; }

                if(Name.Contains("480") || Name.Contains("470") || Name.Contains("580") || Name.Contains("570"))
                {
                    Threads = 2;
                    WorkSize = 8;
                    if ((ComputeDevice.GlobalMemorySize / 1073741824) > 5)
                        Intensity = 128;
                    else
                        Intensity = 108;
                }

                if (Name.Contains("460") || Name.Contains("560"))
                {
                    Threads = 2;
                    WorkSize = 8;
                    if ((ComputeDevice.GlobalMemorySize / 1073741824) > 3)
                        Intensity = 88;
                    else
                        Intensity = 44;
                }

                if (Name.Contains("Vega"))
                {
                    Threads = 2;
                    WorkSize = 8;
                    Intensity = 28 * (int)(ComputeDevice.GlobalMemorySize / 1073741824);
                }
            }
            else
            {
                Name = ComputeDevice.Name;
                if(Name.Contains("GTX 1070"))
                {
                    Threads = 1;
                    WorkSize = 8;
                    Intensity = 160;
                }

                if (Intensity == 0)
                {
                    Threads = 1;
                    WorkSize = 8;
                    Intensity = (int)(ComputeDevice.GlobalMemorySize*(160d/214d) / 39768215);
                }
            }

            if (Intensity == 0)
            {
                Threads = 2;
                WorkSize = 8;
                Intensity = (int)(ComputeDevice.GlobalMemorySize / 39768215);
            }

            if (ComputeDevice.Type == ComputeDeviceTypes.Gpu)
                Name = $"{Name} ({ComputeDevice.GlobalMemorySize / 1073741824} GB)";
        }

        public int Temperature
        {
            get
            {
                switch(ComputeDevice.Vendor)
                {
                    case Consts.VendorAMD:
                        if (ADLAdapterIndex < 0 || null == ADL.ADL_Overdrive5_Temperature_Get)
                            return -1;

                        ADLTemperature OSADLTemperatureData = new ADLTemperature();
                        var tempBuffer = Marshal.AllocCoTaskMem((int)Marshal.SizeOf(OSADLTemperatureData));
                        Marshal.StructureToPtr(OSADLTemperatureData, tempBuffer, false);
                        if (ADL.ADL_Overdrive5_Temperature_Get(ADLAdapterIndex, 0, tempBuffer) != ADL.ADL_SUCCESS)
                            return -1;
                        OSADLTemperatureData = (ADLTemperature)Marshal.PtrToStructure(tempBuffer, OSADLTemperatureData.GetType());
                        return (OSADLTemperatureData.Temperature / 1000);
                        break;
                    case Consts.VendorNvidia:
                        uint temp = 0;

                        if (CudaDevice.Pointer != null)
                        {
                            NvmlNativeMethods.nvmlDeviceGetTemperature(CudaDevice, nvmlTemperatureSensors.Gpu, ref temp);
                            return (int)temp;
                        }
                        else
                            return -1;

                        break;
                    default:
                        return -1;
                }

                return -1;
            }
        }

        public int FanSpeed
        {
            get
            {
                switch (ComputeDevice.Vendor)
                {
                    case Consts.VendorAMD:
                        if (ADLAdapterIndex < 0 || null == ADL.ADL_Overdrive5_FanSpeed_Get)
                            return -1;

                        ADLFanSpeedValue OSADLFanSpeedValueData = new ADLFanSpeedValue();
                        OSADLFanSpeedValueData.iSpeedType = 1;
                        var fanSpeedValueBuffer = Marshal.AllocCoTaskMem((int)Marshal.SizeOf(OSADLFanSpeedValueData));
                        Marshal.StructureToPtr(OSADLFanSpeedValueData, fanSpeedValueBuffer, false);
                        if (ADL.ADL_Overdrive5_FanSpeed_Get(ADLAdapterIndex, 0, fanSpeedValueBuffer) != ADL.ADL_SUCCESS)
                            return -1;
                        OSADLFanSpeedValueData = (ADLFanSpeedValue)Marshal.PtrToStructure(fanSpeedValueBuffer, OSADLFanSpeedValueData.GetType());
                        return OSADLFanSpeedValueData.iFanSpeed;
                        break;
                    case Consts.VendorNvidia:
                        uint fan = 0;

                        if (CudaDevice.Pointer != null)
                        {
                            NvmlNativeMethods.nvmlDeviceGetFanSpeed(CudaDevice, ref fan);
                            return (int)fan;
                        }
                        else
                            return -1;

                        break;
                    default:
                        return -1;
                }

                return -1;
            }

            set
            {
                if (ADLAdapterIndex < 0
                    || null == ADL.ADL_Overdrive5_FanSpeed_Set
                    || null == ADL.ADL_Overdrive5_FanSpeedToDefault_Set)
                    return;

                if (value < 0)
                {
                    ADL.ADL_Overdrive5_FanSpeedToDefault_Set(ADLAdapterIndex, 0);
                }
                else
                {
                    ADLFanSpeedValue OSADLFanSpeedValueData = new ADLFanSpeedValue();
                    OSADLFanSpeedValueData.iSpeedType = 1;
                    OSADLFanSpeedValueData.iFanSpeed = value;
                    OSADLFanSpeedValueData.iFlags = 0;
                    var fanSpeedValueBuffer = Marshal.AllocCoTaskMem((int)Marshal.SizeOf(OSADLFanSpeedValueData));
                    Marshal.StructureToPtr(OSADLFanSpeedValueData, fanSpeedValueBuffer, false);
                    ADL.ADL_Overdrive5_FanSpeed_Set(ADLAdapterIndex, 0, fanSpeedValueBuffer);
                }
            }
        }

        public int Activity
        {
            get
            {
                if (ADLAdapterIndex == -2)
                {
                    if (_cpuCounter == null)
                        _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

                    return Convert.ToInt32(_cpuCounter.NextValue());
                }

                switch (ComputeDevice.Vendor)
                {
                    case Consts.VendorAMD:
                        if (ADLAdapterIndex < 0 || null == ADL.ADL_Overdrive5_CurrentActivity_Get)
                            return -1;

                        ADLPMActivity OSADLPMActivityData = new ADLPMActivity();
                        var activityBuffer = Marshal.AllocCoTaskMem((int)Marshal.SizeOf(OSADLPMActivityData));
                        Marshal.StructureToPtr(OSADLPMActivityData, activityBuffer, false);
                        if (ADL.ADL_Overdrive5_CurrentActivity_Get(ADLAdapterIndex, activityBuffer) != ADL.ADL_SUCCESS)
                            return -1;
                        OSADLPMActivityData = (ADLPMActivity)Marshal.PtrToStructure(activityBuffer, OSADLPMActivityData.GetType());
                        return OSADLPMActivityData.iActivityPercent;

                        break;
                    case Consts.VendorNvidia:
                        var activity = new nvmlUtilization();
                        if (CudaDevice.Pointer != null)
                        {
                            NvmlNativeMethods.nvmlDeviceGetUtilizationRates(CudaDevice, ref activity);
                            return (int)activity.gpu;
                        }
                        else
                            return -1;

                        break;
                    default:
                        return -1;
                }

                return -1;







            }
        }
    }
}
