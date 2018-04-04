#region Copyright

// Copyright 2017 Yurio Miyazawa (a.k.a zawawa) <me@yurio.net>
//
// This file is part of Gateless Gate Sharp.
//
// Gateless Gate Sharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Gateless Gate Sharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Gateless Gate Sharp.  If not, see <http://www.gnu.org/licenses/>.

/*******************************************************************************
 Copyright(c) 2008 - 2009 Advanced Micro Devices, Inc. All Rights Reserved.
 Copyright (c) 2002 - 2006  ATI Technologies Inc. All Rights Reserved.
 
 THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
 ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDED BUT NOT LIMITED TO
 THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
 PARTICULAR PURPOSE.
 
 File:        ADL.cs
 
 Purpose:     Implements ADL interface 
 
 Description: Implements some of the methods defined in ADL interface.
              
 ********************************************************************************/

#endregion Copyright

#region Using

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using FARPROC = System.IntPtr;
using HMODULE = System.IntPtr;

#endregion Using

#region ATI.ADL

namespace ATI.ADL
{
    #region Export Delegates
    /// <summary> ADL Memory allocation function allows ADL to callback for memory allocation</summary>
    /// <param name="size">input size</param>
    /// <returns> retrun ADL Error Code</returns>
    internal delegate IntPtr ADL_Main_Memory_Alloc (int size);

    // ///// <summary> ADL Create Function to create ADL Data</summary>
    /// <param name="callback">Call back functin pointer which is ised to allocate memeory </param>
    /// <param name="enumConnectedAdapters">If it is 1, then ADL will only retuen the physical exist adapters </param>
    ///// <returns> retrun ADL Error Code</returns>
    internal delegate int ADL_Main_Control_Create(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters);
    internal delegate int ADL2_Main_Control_Create(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters, ref IntPtr context);

    /// <summary> ADL Destroy Function to free up ADL Data</summary>
    /// <returns> retrun ADL Error Code</returns>
    internal delegate int ADL_Main_Control_Destroy();
    internal delegate int ADL2_Main_Control_Destroy(IntPtr context);

    /// <summary> ADL Function to get the number of adapters</summary>
    /// <param name="numAdapters">return number of adapters</param>
    /// <returns> retrun ADL Error Code</returns>
    internal delegate int ADL_Adapter_NumberOfAdapters_Get (ref int numAdapters);

    /// <summary> ADL Function to get the GPU adapter information</summary>
    /// <param name="info">return GPU adapter information</param>
    /// <param name="inputSize">the size of the GPU adapter struct</param>
    /// <returns> retrun ADL Error Code</returns>
    internal delegate int ADL_Adapter_AdapterInfo_Get (IntPtr info, int inputSize);

    /// <summary> Function to determine if the adapter is active or not.</summary>
    /// <remarks>The function is used to check if the adapter associated with iAdapterIndex is active</remarks>  
    /// <param name="adapterIndex"> Adapter Index.</param>
    /// <param name="status"> Status of the adapter. True: Active; False: Dsiabled</param>
    /// <returns>Non zero is successfull</returns> 
    internal delegate int ADL_Adapter_Active_Get(int adapterIndex, ref int status);

    /// <summary>Get display information based on adapter index</summary>
    /// <param name="adapterIndex">Adapter Index</param>
    /// <param name="numDisplays">return the total number of supported displays</param>
    /// <param name="displayInfoArray">return ADLDisplayInfo Array for supported displays' information</param>
    /// <param name="forceDetect">force detect or not</param>
    /// <returns>return ADL Error Code</returns>
    internal delegate int ADL_Display_DisplayInfo_Get(int adapterIndex, ref int numDisplays, out IntPtr displayInfoArray, int forceDetect);

    /// <summary>Retrieve thermal controller temperatures.</summary>
    /// <param name="adapterIndex">Adapter Index</param>
    /// <returns>return ADL Error Code</returns>
    internal delegate int ADL_Overdrive5_Temperature_Get(int adapterIndex, int iThermalControllerIndex, IntPtr temperature);

    /// <summary>Retrieve fan speed information.</summary>
    /// <param name="adapterIndex">Adapter Index</param>
    /// <returns>return ADL Error Code</returns>
    internal delegate int ADL_Overdrive5_FanSpeed_Get(int adapterIndex, int iThermalControllerIndex, IntPtr fanSpeedValue);

    /// <summary>Retrieve current power management-related activity.</summary>
    /// <param name="adapterIndex">Adapter Index</param>
    /// <returns>return ADL Error Code</returns>
    internal delegate int ADL_Overdrive5_CurrentActivity_Get(int adapterIndex, IntPtr activity);

    /// <summary>Set fan speed.</summary>
    /// <param name="adapterIndex">Adapter Index</param>
    /// <returns>return ADL Error Code</returns>
    internal delegate int ADL_Overdrive5_FanSpeed_Set(int adapterIndex, int iThermalControllerIndex, IntPtr fanSpeedValue);

    /// <summary>Set fan speed to default value.</summary>
    /// <param name="adapterIndex">Adapter Index</param>
    /// <returns>return ADL Error Code</returns>
    internal delegate int ADL_Overdrive5_FanSpeedToDefault_Set(int adapterIndex, int iThermalControllerIndex);

    internal delegate int ADL_Overdrive5_ODParameters_Get(int iAdapterIndex, IntPtr lpOdParameters);
    internal delegate int ADL_Overdrive5_ODPerformanceLevels_Get(int iAdapterIndex, int iDefault, IntPtr lpODPerformanceLevels);
    internal delegate int ADL_Overdrive5_ODPerformanceLevels_Set(int iAdapterIndex, IntPtr lpODPerformanceLevels);
    internal delegate int 	ADL_Overdrive5_PowerControl_Get (int iAdapterIndex, ref int lpCurrentValue, ref int lpDefaultValue);
    internal delegate int ADL_Overdrive5_PowerControl_Set(int iAdapterIndex, int iValue);
    internal delegate int ADL2_Overdrive_Caps(IntPtr context, int iAdapterIndex, ref int iSupported, ref int iEnabled, ref int iVersion);

    internal delegate int ADL_Overdrive6_CurrentPower_Get(int iAdapterIndex, int iPowerType, ref int lpCurrentValue);
    internal delegate int ADL_Overdrive6_VoltageControl_Get(int iAdapterIndex, ref int lpCurrentValue, ref int lpDefaultValue);
    internal delegate int ADL_Overdrive6_VoltageControl_Set(int iAdapterIndex, int iValue);

    internal delegate int ADL2_Overdrive6_CurrentPower_Get(IntPtr context, int iAdapterIndex, int iPowerType, ref int lpCurrentValue);
    internal delegate int ADL2_Overdrive6_VoltageControl_Get(IntPtr context, int iAdapterIndex, ref int lpCurrentValue, ref int lpDefaultValue);
    internal delegate int ADL2_Overdrive6_VoltageControl_Set(IntPtr context, int iAdapterIndex, int iValue);

    internal delegate int ADL2_OverdriveN_Capabilities_Get(IntPtr context, int iAdapterIndex, IntPtr lpODCapabilities);
    internal delegate int 	ADL2_OverdriveN_SystemClocks_Get (IntPtr context, int iAdapterIndex, IntPtr pODPerformanceLevels);
    internal delegate int 	ADL2_OverdriveN_SystemClocks_Set (IntPtr context, int iAdapterIndex, IntPtr lpODPerformanceLevels);
    internal delegate int 	ADL2_OverdriveN_MemoryClocks_Get (IntPtr context, int iAdapterIndex, IntPtr lpODPerformanceLevels);
    internal delegate int 	ADL2_OverdriveN_MemoryClocks_Set (IntPtr context, int iAdapterIndex, IntPtr lpODPerformanceLevels);
    internal delegate int 	ADL2_OverdriveN_FanControl_Get (IntPtr context, int iAdapterIndex, IntPtr lpODFanSpeed);
    internal delegate int 	ADL2_OverdriveN_FanControl_Set (IntPtr context, int iAdapterIndex, IntPtr lpODFanControl);
    internal delegate int 	ADL2_OverdriveN_PowerLimit_Get (IntPtr context, int iAdapterIndex, IntPtr lpODPowerLimit);
    internal delegate int 	ADL2_OverdriveN_PowerLimit_Set (IntPtr context, int iAdapterIndex, IntPtr lpODPowerLimit);
    internal delegate int 	ADL2_OverdriveN_Temperature_Get (IntPtr context, int iAdapterIndex, int iTemperatureType, ref int iTemperature);
    internal delegate int 	ADL2_OverdriveN_PerformanceStatus_Get (IntPtr context, int iAdapterIndex, IntPtr pODPerformanceStatus);

    #endregion Export Delegates

    #region Export Struct

    #region ADLAdapterInfo
    /// <summary> ADLAdapterInfo Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLAdapterInfo
    {
        /// <summary>The size of the structure</summary>
        int Size;
        /// <summary> Adapter Index</summary>
        internal int AdapterIndex;
        /// <summary> Adapter UDID</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string UDID;
        /// <summary> Adapter Bus Number</summary>
        internal int BusNumber;
        /// <summary> Adapter Driver Number</summary>
        internal int DriverNumber;
        /// <summary> Adapter Function Number</summary>
        internal int FunctionNumber;
        /// <summary> Adapter Vendor ID</summary>
        internal int VendorID;
        /// <summary> Adapter Adapter name</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string AdapterName;
        /// <summary> Adapter Display name</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string DisplayName;
        /// <summary> Adapter Present status</summary>
        internal int Present;
        /// <summary> Adapter Exist status</summary>
        internal int Exist;
        /// <summary> Adapter Driver Path</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string DriverPath;
        /// <summary> Adapter Driver Ext Path</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string DriverPathExt;
        /// <summary> Adapter PNP String</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string PNPString;
        /// <summary> OS Display Index</summary>
        internal int OSDisplayIndex;
    }


    /// <summary> ADLAdapterInfo Array</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLAdapterInfoArray
    {
        /// <summary> ADLAdapterInfo Array </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)ADL.ADL_MAX_ADAPTERS)]
        internal ADLAdapterInfo[] ADLAdapterInfo;
    }
    #endregion ADLAdapterInfo

        
    #region ADLDisplayInfo
    /// <summary> ADLDisplayID Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLDisplayID
    {
        /// <summary> Display Logical Index </summary>
        internal int DisplayLogicalIndex;
        /// <summary> Display Physical Index </summary>
        internal int DisplayPhysicalIndex;
        /// <summary> Adapter Logical Index </summary>
        internal int DisplayLogicalAdapterIndex;
        /// <summary> Adapter Physical Index </summary>
        internal int DisplayPhysicalAdapterIndex;
    }

    /// <summary> ADLDisplayInfo Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLDisplayInfo
    {
        /// <summary> Display Index </summary>
        internal ADLDisplayID DisplayID;
        /// <summary> Display Controller Index </summary>
        internal int DisplayControllerIndex;
        /// <summary> Display Name </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string DisplayName;
        /// <summary> Display Manufacturer Name </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string DisplayManufacturerName;
        /// <summary> Display Type : < The Display type. CRT, TV,CV,DFP are some of display types,</summary>
        internal int DisplayType;
        /// <summary> Display output type </summary>
        internal int DisplayOutputType;
        /// <summary> Connector type</summary>
        internal int DisplayConnector;
        ///<summary> Indicating the display info bits' mask.<summary>
        internal int DisplayInfoMask;
        ///<summary> Indicating the display info value.<summary>
        internal int DisplayInfoValue;
    }
    #endregion ADLDisplayInfo

    /// <summary> ADLTemperature Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLTemperature
    {
        /// <summary>The size of the structure</summary>
        int Size;
        /// <summary> </summary>
        internal int Temperature;
    }

    /// <summary> ADLPMActivity Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLPMActivity
    {
        /// <summary>The size of the structure</summary>
        int Size;
        /// <summary> </summary>
        internal int iEngineClock;
        internal int iMemoryClock;
        internal int iVddc;
        internal int iActivityPercent;
        internal int iCurrentPerformanceLevel;
        internal int iCurrentBusSpeed;
        internal int iCurrentBusLanes;
        internal int iMaximumBusLanes;
        internal int iReserved;
    }

    /// <summary> ADLFanSpeedInfo Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct 	ADLFanSpeedValue
    {
        /// <summary>The size of the structure</summary>
        int Size;
        /// <summary> </summary>
        internal int 	iSpeedType;
        internal int 	iFanSpeed;
        internal int 	iFlags;
    }

    /// <summary> ADLODNPerformanceStatus Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLODNPerformanceStatus {
        /// <summary> </summary>
        internal int iCoreClock;
        internal int iMemoryClock;
        internal int iDCEFClock;
        internal int iGFXClock;
        internal int iUVDClock;
        internal int iVCEClock;
        internal int iGPUActivityPercent;
        internal int iCurrentCorePerformanceLevel;
        internal int iCurrentMemoryPerformanceLevel;
        internal int iCurrentDCEFPerformanceLevel;
        internal int iCurrentGFXPerformanceLevel;
        internal int iUVDPerformanceLevel;
        internal int iVCEPerformanceLevel;
        internal int iCurrentBusSpeed;
        internal int iCurrentBusLanes;
        internal int iMaximumBusLanes;
        internal int iVDDC;
        internal int iVDDCI;
    }

    /// <summary> ADLODNCapabilities Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLODNCapabilities {
        /// <summary> </summary>
        internal int iMaximumNumberOfPerformanceLevels;
        internal ADLODNParameterRange sEngineClockRange;
        internal ADLODNParameterRange sMemoryClockRange;
        internal ADLODNParameterRange svddcRange;
        internal ADLODNParameterRange power;
        internal ADLODNParameterRange powerTuneTemperature;
        internal ADLODNParameterRange fanTemperature;
        internal ADLODNParameterRange fanSpeed;
        internal ADLODNParameterRange minimumPerformanceClock;
    }

    /// <summary> ADLODCapabilities Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    struct ADLODParameters {
        /// <summary> </summary>
        internal int iSize;
        internal int iNumberOfPerformanceLevels;
        internal int iActivityReportingSupported;
        internal int iDiscretePerformanceLevels;
        internal int iReserved;
        internal ADLODParameterRange sEngineClockRange;
        internal ADLODParameterRange sMemoryClockRange;
        internal ADLODParameterRange sVddc;
    }

    enum ADLODNControlType {
        ODNControlType_Current = 0,
        ODNControlType_Default,
        ODNControlType_Auto,
        ODNControlType_Manual
    };

    /// <summary> ADLODNParameterRange Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLODNParameterRange {
        /// <summary> </summary>
        internal int iMode;
        internal int iMin;
        internal int iMax;
        internal int iStep;
        internal int iDefault;
    }

    /// <summary> ADLODNPerformanceLevels Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLODNPerformanceLevels {
        /// <summary> </summary>
        internal int iSize;
        internal int iMode;
        internal int iNumberOfPerformanceLevels;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_ODN)]
        internal ADLODNPerformanceLevel[] aLevels;
    }

    /// <summary> ADLODNPerformanceLevel Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLODNPerformanceLevel {
        /// <summary> </summary>
        internal int iClock;
        internal int iVddc;
        internal int iEnabled;
    }

    /// <summary> ADLODParameterRange Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLODParameterRange {
        /// <summary> </summary>
        internal int iMin;
        internal int iMax;
        internal int iStep;
    }

    /// <summary> ADLODPerformanceLevels Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLODPerformanceLevels {
        /// <summary> </summary>
        internal int iSize;
        internal int iReserved;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_OD5)]
        internal ADLODPerformanceLevel[] aLevels;
    }

    /// <summary> ADLODPerformanceLevel Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLODPerformanceLevel {
        /// <summary> </summary>
        internal int iEngineClock;
        internal int iMemoryClock;
        internal int iVddc;
    }

    #endregion Export Struct

    #region ADL Class
    /// <summary> ADL Class</summary>
    internal static class ADL
    {
        #region Internal Constant
        /// <summary> Define the maximum path</summary>
        internal const int ADL_MAX_PATH = 256;
        /// <summary> Define the maximum adapters</summary>
        internal const int ADL_MAX_ADAPTERS = 1024; // 40 /* 150 */;
        /// <summary> Define the maximum displays</summary>
        internal const int ADL_MAX_DISPLAYS = 1024; // 40 /* 150 */;
        /// <summary> Define the maximum device name length</summary>
        internal const int ADL_MAX_DEVICENAME = 256;
        /// <summary> Define the successful</summary>
        internal const int ADL_SUCCESS = 0;
        /// <summary> Define the failure</summary>
        internal const int ADL_FAIL = -1;
        /// <summary> Define the driver ok</summary>
        internal const int ADL_DRIVER_OK = 0;
        /// <summary> Maximum number of GL-Sync ports on the GL-Sync module </summary>
        internal const int ADL_MAX_GLSYNC_PORTS = 8;
        /// <summary> Maximum number of GL-Sync ports on the GL-Sync module </summary>
        internal const int ADL_MAX_GLSYNC_PORT_LEDS = 8;
        /// <summary> Maximum number of ADLMOdes for the adapter </summary>
        internal const int ADL_MAX_NUM_DISPLAYMODES = 1024;

        internal const int ADL_MAX_NUM_PERFORMANCE_LEVELS_OD5 = 2;
        internal const int ADL_MAX_NUM_PERFORMANCE_LEVELS_ODN = 8;


        #endregion Internal Constant

        #region Class ADLImport
        /// <summary> ADLImport class</summary>
        private static class ADLImport
        {
            #region Internal Constant
            /// <summary> Atiadlxx_FileName </summary>
            internal const string Atiadlxx_FileName = "atiadlxx.dll";
            /// <summary> Kernel32_FileName </summary>
            internal const string Kernel32_FileName = "kernel32.dll";
            #endregion Internal Constant

            #region DLLImport
            [DllImport(Kernel32_FileName)]
            internal static extern HMODULE GetModuleHandle (string moduleName);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Main_Control_Create(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL2_Main_Control_Create(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters, ref IntPtr context);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Main_Control_Destroy();

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL2_Main_Control_Destroy(IntPtr context);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Main_Control_IsFunctionValid (HMODULE module, string procName);

            [DllImport(Atiadlxx_FileName)]
            internal static extern FARPROC ADL_Main_Control_GetProcAddress (HMODULE module, string procName);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Adapter_NumberOfAdapters_Get (ref int numAdapters);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Adapter_AdapterInfo_Get (IntPtr info, int inputSize);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Adapter_Active_Get(int adapterIndex, ref int status);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Display_DisplayInfo_Get(int adapterIndex, ref int numDisplays, out IntPtr displayInfoArray, int forceDetect);


            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Overdrive5_Temperature_Get(int adapterIndex, int thermalControllerIndex, IntPtr temperature);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Overdrive5_CurrentActivity_Get(int adapterIndex, IntPtr activity);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Overdrive5_FanSpeed_Get(int adapterIndex, int iThermalControllerIndex, IntPtr fanSpeedValue);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Overdrive5_FanSpeed_Set(int adapterIndex, int iThermalControllerIndex, IntPtr fanSpeedValue);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Overdrive5_FanSpeedToDefault_Set(int adapterIndex, int iThermalControllerIndex);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Overdrive5_ODParameters_Get(int iAdapterIndex, IntPtr lpOdParameters);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Overdrive5_ODPerformanceLevels_Get(int iAdapterIndex, int iDefault, IntPtr lpODPerformanceLevels);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Overdrive5_ODPerformanceLevels_Set(int iAdapterIndex, IntPtr lpODPerformanceLevels);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Overdrive5_PowerControl_Get(int iAdapterIndex, ref int lpCurrentValue, ref int lpDefaultValue);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Overdrive5_PowerControl_Set(int iAdapterIndex, int iValue);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL2_Overdrive_Caps(IntPtr context, int iAdapterIndex, ref int iSupported, ref int iEnabled, ref int iVersion);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Overdrive6_CurrentPower_Get(int AdapterIndex, int iPowerType, ref int lpCurrentValue);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Overdrive6_VoltageControl_Get(int iAdapterIndex, ref int lpCurrentValue, ref int lpDefaultValue);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL_Overdrive6_VoltageControl_Set(int iAdapterIndex, int iValue);


            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL2_Overdrive6_CurrentPower_Get(IntPtr context, int AdapterIndex, int iPowerType, ref int lpCurrentValue);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL2_Overdrive6_VoltageControl_Get(IntPtr context, int iAdapterIndex, ref int lpCurrentValue, ref int lpDefaultValue);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL2_Overdrive6_VoltageControl_Set(IntPtr context, int iAdapterIndex, int iValue);


            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL2_OverdriveN_Capabilities_Get(IntPtr context, int iAdapterIndex, IntPtr lpODCapabilities);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL2_OverdriveN_SystemClocks_Get(IntPtr context, int iAdapterIndex, IntPtr pODPerformanceLevels);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL2_OverdriveN_SystemClocks_Set(IntPtr context, int iAdapterIndex, IntPtr lpODPerformanceLevels);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL2_OverdriveN_MemoryClocks_Get(IntPtr context, int iAdapterIndex, IntPtr lpODPerformanceLevels);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL2_OverdriveN_MemoryClocks_Set(IntPtr context, int iAdapterIndex, IntPtr lpODPerformanceLevels);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL2_OverdriveN_FanControl_Get(IntPtr context, int iAdapterIndex, IntPtr lpODFanSpeed);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL2_OverdriveN_FanControl_Set(IntPtr context, int iAdapterIndex, IntPtr lpODFanControl);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL2_OverdriveN_PowerLimit_Get(IntPtr context, int iAdapterIndex, IntPtr lpODPowerLimit);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL2_OverdriveN_PowerLimit_Set(IntPtr context, int iAdapterIndex, IntPtr lpODPowerLimit);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL2_OverdriveN_Temperature_Get(IntPtr context, int iAdapterIndex, int iTemperatureType, ref int iTemperature);

            [DllImport(Atiadlxx_FileName)]
            internal static extern int ADL2_OverdriveN_PerformanceStatus_Get(IntPtr context, int iAdapterIndex, IntPtr pODPerformanceStatus);
            
            #endregion DLLImport
        }
        #endregion Class ADLImport

        #region Class ADLCheckLibrary
        /// <summary> ADLCheckLibrary class</summary>
        private class ADLCheckLibrary
        {
            #region Private Members
            private HMODULE ADLLibrary = System.IntPtr.Zero;
            #endregion Private Members

            #region Static Members
            /// <summary> new a private instance</summary>
            private static ADLCheckLibrary ADLCheckLibrary_ = new ADLCheckLibrary();
            #endregion Static Members

            #region Constructor
            /// <summary> Constructor</summary>
            private ADLCheckLibrary ()
            {
                try
                {
                    if (1 == ADLImport.ADL_Main_Control_IsFunctionValid(IntPtr.Zero, "ADL_Main_Control_Create"))
                    {
                        ADLLibrary = ADLImport.GetModuleHandle(ADLImport.Atiadlxx_FileName);
                    }
                }
                catch (DllNotFoundException) { }
                catch (EntryPointNotFoundException) { }
                catch (Exception) { }
            }
            #endregion Constructor

            #region Destructor
            /// <summary> Destructor to force calling ADL Destroy function before free up the ADL library</summary>
            ~ADLCheckLibrary ()
            {
                if (System.IntPtr.Zero != ADLCheckLibrary_.ADLLibrary)
                {
                    ADLImport.ADL_Main_Control_Destroy();
                }
            }
            #endregion Destructor

            #region Static IsFunctionValid
            /// <summary> Check the import function to see it exists or not</summary>
            /// <param name="functionName"> function name</param>
            /// <returns>return true, if function exists</returns>
            internal static bool IsFunctionValid (string functionName)
            {
                bool result = false;
                if (System.IntPtr.Zero != ADLCheckLibrary_.ADLLibrary)
                {
                    if (1 == ADLImport.ADL_Main_Control_IsFunctionValid(ADLCheckLibrary_.ADLLibrary, functionName))
                    {
                        result = true;
                    }
                }
                return result;
            }
            #endregion Static IsFunctionValid

            #region Static GetProcAddress
            /// <summary> Get the unmanaged function pointer </summary>
            /// <param name="functionName"> function name</param>
            /// <returns>return function pointer, if function exists</returns>
            internal static FARPROC GetProcAddress (string functionName)
            {
                FARPROC result = System.IntPtr.Zero;
                if (System.IntPtr.Zero != ADLCheckLibrary_.ADLLibrary)
                {
                    result = ADLImport.ADL_Main_Control_GetProcAddress(ADLCheckLibrary_.ADLLibrary, functionName);
                }
                return result;
            }
            #endregion Static GetProcAddress
        }
        #endregion Class ADLCheckLibrary

        #region Export Functions

        #region ADL_Main_Memory_Alloc
        /// <summary> Build in memory allocation function</summary>
        internal static ADL_Main_Memory_Alloc ADL_Main_Memory_Alloc = ADL_Main_Memory_Alloc_;
        /// <summary> Build in memory allocation function</summary>
        /// <param name="size">input size</param>
        /// <returns>return the memory buffer</returns>
        private static IntPtr ADL_Main_Memory_Alloc_ (int size)
        {
            IntPtr result = Marshal.AllocCoTaskMem(size);
            return result;
        }
        #endregion ADL_Main_Memory_Alloc

        #region ADL_Main_Memory_Free
        /// <summary> Build in memory free function</summary>
        /// <param name="buffer">input buffer</param>
        internal static void ADL_Main_Memory_Free (IntPtr buffer)
        {
            if (IntPtr.Zero != buffer)
            {
                Marshal.FreeCoTaskMem(buffer);
            }
        }
        #endregion ADL_Main_Memory_Free

        #region ADL_Main_Control_Create
        /// <summary> ADL_Main_Control_Create Delegates</summary>
        internal static ADL_Main_Control_Create ADL_Main_Control_Create {
            get {
                if (!ADL_Main_Control_Create_Check && null == ADL_Main_Control_Create_) {
                    ADL_Main_Control_Create_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Main_Control_Create")) {
                        ADL_Main_Control_Create_ = ADLImport.ADL_Main_Control_Create;
                    }
                }
                return ADL_Main_Control_Create_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Main_Control_Create ADL_Main_Control_Create_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Main_Control_Create_Check = false;
        /// <summary> ADL2_Main_Control_Create Delegates</summary>
        internal static ADL2_Main_Control_Create ADL2_Main_Control_Create {
            get {
                if (!ADL2_Main_Control_Create_Check && null == ADL2_Main_Control_Create_) {
                    ADL2_Main_Control_Create_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL2_Main_Control_Create")) {
                        ADL2_Main_Control_Create_ = ADLImport.ADL2_Main_Control_Create;
                    }
                }
                return ADL2_Main_Control_Create_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL2_Main_Control_Create ADL2_Main_Control_Create_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL2_Main_Control_Create_Check = false;
        #endregion ADL_Main_Control_Create

        #region ADL_Main_Control_Destroy
        /// <summary> ADL_Main_Control_Destroy Delegates</summary>
        internal static ADL_Main_Control_Destroy ADL_Main_Control_Destroy {
            get {
                if (!ADL_Main_Control_Destroy_Check && null == ADL_Main_Control_Destroy_) {
                    ADL_Main_Control_Destroy_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Main_Control_Destroy")) {
                        ADL_Main_Control_Destroy_ = ADLImport.ADL_Main_Control_Destroy;
                    }
                }
                return ADL_Main_Control_Destroy_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Main_Control_Destroy ADL_Main_Control_Destroy_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Main_Control_Destroy_Check = false;
        /// <summary> ADL2_Main_Control_Destroy Delegates</summary>
        internal static ADL2_Main_Control_Destroy ADL2_Main_Control_Destroy {
            get {
                if (!ADL2_Main_Control_Destroy_Check && null == ADL2_Main_Control_Destroy_) {
                    ADL2_Main_Control_Destroy_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL2_Main_Control_Destroy")) {
                        ADL2_Main_Control_Destroy_ = ADLImport.ADL2_Main_Control_Destroy;
                    }
                }
                return ADL2_Main_Control_Destroy_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL2_Main_Control_Destroy ADL2_Main_Control_Destroy_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL2_Main_Control_Destroy_Check = false;
        #endregion ADL_Main_Control_Destroy

        #region ADL_Adapter_NumberOfAdapters_Get
        /// <summary> ADL_Adapter_NumberOfAdapters_Get Delegates</summary>
        internal static ADL_Adapter_NumberOfAdapters_Get ADL_Adapter_NumberOfAdapters_Get
        {
            get
            {
                if (!ADL_Adapter_NumberOfAdapters_Get_Check && null == ADL_Adapter_NumberOfAdapters_Get_)
                {
                    ADL_Adapter_NumberOfAdapters_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Adapter_NumberOfAdapters_Get"))
                    {
                        ADL_Adapter_NumberOfAdapters_Get_ = ADLImport.ADL_Adapter_NumberOfAdapters_Get;
                    }
                }
                return ADL_Adapter_NumberOfAdapters_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Adapter_NumberOfAdapters_Get ADL_Adapter_NumberOfAdapters_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Adapter_NumberOfAdapters_Get_Check = false;
        #endregion ADL_Adapter_NumberOfAdapters_Get

        #region ADL_Adapter_AdapterInfo_Get
        /// <summary> ADL_Adapter_AdapterInfo_Get Delegates</summary>
        internal static ADL_Adapter_AdapterInfo_Get ADL_Adapter_AdapterInfo_Get
        {
            get
            {
                if (!ADL_Adapter_AdapterInfo_Get_Check && null == ADL_Adapter_AdapterInfo_Get_)
                {
                    ADL_Adapter_AdapterInfo_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Adapter_AdapterInfo_Get"))
                    {
                        ADL_Adapter_AdapterInfo_Get_ = ADLImport.ADL_Adapter_AdapterInfo_Get;
                    }
                }
                return ADL_Adapter_AdapterInfo_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Adapter_AdapterInfo_Get ADL_Adapter_AdapterInfo_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Adapter_AdapterInfo_Get_Check = false;
        #endregion ADL_Adapter_AdapterInfo_Get

        #region ADL_Adapter_Active_Get
        /// <summary> ADL_Adapter_Active_Get Delegates</summary>
        internal static ADL_Adapter_Active_Get ADL_Adapter_Active_Get
        {
            get
            {
                if (!ADL_Adapter_Active_Get_Check && null == ADL_Adapter_Active_Get_)
                {
                    ADL_Adapter_Active_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Adapter_Active_Get"))
                    {
                        ADL_Adapter_Active_Get_ = ADLImport.ADL_Adapter_Active_Get;
                    }
                }
                return ADL_Adapter_Active_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Adapter_Active_Get ADL_Adapter_Active_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Adapter_Active_Get_Check = false;
        #endregion ADL_Adapter_Active_Get

        #region ADL_Display_DisplayInfo_Get
        /// <summary> ADL_Display_DisplayInfo_Get Delegates</summary>
        internal static ADL_Display_DisplayInfo_Get ADL_Display_DisplayInfo_Get
        {
            get
            {
                if (!ADL_Display_DisplayInfo_Get_Check && null == ADL_Display_DisplayInfo_Get_)
                {
                    ADL_Display_DisplayInfo_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Display_DisplayInfo_Get"))
                    {
                        ADL_Display_DisplayInfo_Get_ = ADLImport.ADL_Display_DisplayInfo_Get;
                    }
                }
                return ADL_Display_DisplayInfo_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Display_DisplayInfo_Get ADL_Display_DisplayInfo_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Display_DisplayInfo_Get_Check = false;
        #endregion ADL_Display_DisplayInfo_Get

        #region ADL_Overdrive5_Temperature_Get
        /// <summary> ADL_Overdrive5_Temperature_Get Delegates</summary>
        internal static ADL_Overdrive5_Temperature_Get ADL_Overdrive5_Temperature_Get
        {
            get
            {
                if (!ADL_Overdrive5_Temperature_Get_Check && null == ADL_Overdrive5_Temperature_Get_)
                {
                    ADL_Overdrive5_Temperature_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Overdrive5_Temperature_Get"))
                    {
                        ADL_Overdrive5_Temperature_Get_ = ADLImport.ADL_Overdrive5_Temperature_Get;
                    }
                }
                return ADL_Overdrive5_Temperature_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Overdrive5_Temperature_Get ADL_Overdrive5_Temperature_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Overdrive5_Temperature_Get_Check = false;
        #endregion ADL_Overdrive5_Temperature_Get

        #region ADL_Overdrive5_CurrentActivity_Get
        /// <summary> ADL_Overdrive5_CurrentActivity_Get Delegates</summary>
        internal static ADL_Overdrive5_CurrentActivity_Get ADL_Overdrive5_CurrentActivity_Get
        {
            get
            {
                if (!ADL_Overdrive5_CurrentActivity_Get_Check && null == ADL_Overdrive5_CurrentActivity_Get_)
                {
                    ADL_Overdrive5_CurrentActivity_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Overdrive5_CurrentActivity_Get"))
                    {
                        ADL_Overdrive5_CurrentActivity_Get_ = ADLImport.ADL_Overdrive5_CurrentActivity_Get;
                    }
                }
                return ADL_Overdrive5_CurrentActivity_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Overdrive5_CurrentActivity_Get ADL_Overdrive5_CurrentActivity_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Overdrive5_CurrentActivity_Get_Check = false;
        #endregion ADL_Overdrive5_CurrentActivity_Get

        #region ADL_Overdrive5_FanSpeed_Get
        /// <summary> ADL_Overdrive5_FanSpeed_Get Delegates</summary>
        internal static ADL_Overdrive5_FanSpeed_Get ADL_Overdrive5_FanSpeed_Get {
            get {
                if (!ADL_Overdrive5_FanSpeed_Get_Check && null == ADL_Overdrive5_FanSpeed_Get_) {
                    ADL_Overdrive5_FanSpeed_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Overdrive5_FanSpeed_Get")) {
                        ADL_Overdrive5_FanSpeed_Get_ = ADLImport.ADL_Overdrive5_FanSpeed_Get;
                    }
                }
                return ADL_Overdrive5_FanSpeed_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Overdrive5_FanSpeed_Get ADL_Overdrive5_FanSpeed_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Overdrive5_FanSpeed_Get_Check = false;
        #endregion ADL_Overdrive5_FanSpeed_Get

        #region ADL_Overdrive5_FanSpeed_Set
        /// <summary> ADL_Overdrive5_FanSpeed_Set Delegates</summary>
        internal static ADL_Overdrive5_FanSpeed_Set ADL_Overdrive5_FanSpeed_Set {
            get {
                if (!ADL_Overdrive5_FanSpeed_Set_Check && null == ADL_Overdrive5_FanSpeed_Set_) {
                    ADL_Overdrive5_FanSpeed_Set_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Overdrive5_FanSpeed_Set")) {
                        ADL_Overdrive5_FanSpeed_Set_ = ADLImport.ADL_Overdrive5_FanSpeed_Set;
                    }
                }
                return ADL_Overdrive5_FanSpeed_Set_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Overdrive5_FanSpeed_Set ADL_Overdrive5_FanSpeed_Set_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Overdrive5_FanSpeed_Set_Check = false;
        #endregion ADL_Overdrive5_FanSpeed_Set

        #region ADL_Overdrive5_PowerControl_Get
        /// <summary> ADL_Overdrive5_PowerControl_Get Delegates</summary>
        internal static ADL_Overdrive5_PowerControl_Get ADL_Overdrive5_PowerControl_Get {
            get {
                if (!ADL_Overdrive5_PowerControl_Get_Check && null == ADL_Overdrive5_PowerControl_Get_) {
                    ADL_Overdrive5_PowerControl_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Overdrive5_PowerControl_Get")) {
                        ADL_Overdrive5_PowerControl_Get_ = ADLImport.ADL_Overdrive5_PowerControl_Get;
                    }
                }
                return ADL_Overdrive5_PowerControl_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Overdrive5_PowerControl_Get ADL_Overdrive5_PowerControl_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Overdrive5_PowerControl_Get_Check = false;
        #endregion ADL_Overdrive5_PowerControl_Get

        #region ADL_Overdrive5_PowerControl_Set
        /// <summary> ADL_Overdrive5_PowerControl_Set Delegates</summary>
        internal static ADL_Overdrive5_PowerControl_Set ADL_Overdrive5_PowerControl_Set {
            get {
                if (!ADL_Overdrive5_PowerControl_Set_Check && null == ADL_Overdrive5_PowerControl_Set_) {
                    ADL_Overdrive5_PowerControl_Set_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Overdrive5_PowerControl_Set")) {
                        ADL_Overdrive5_PowerControl_Set_ = ADLImport.ADL_Overdrive5_PowerControl_Set;
                    }
                }
                return ADL_Overdrive5_PowerControl_Set_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Overdrive5_PowerControl_Set ADL_Overdrive5_PowerControl_Set_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Overdrive5_PowerControl_Set_Check = false;
        #endregion ADL_Overdrive5_PowerControl_Set

        #region ADL_Overdrive5_ODParameters_Get
        /// <summary> ADL_Overdrive5_ODParameters_Get Delegates</summary>
        internal static ADL_Overdrive5_ODParameters_Get ADL_Overdrive5_ODParameters_Get {
            get {
                if (!ADL_Overdrive5_ODParameters_Get_Check && null == ADL_Overdrive5_ODParameters_Get_) {
                    ADL_Overdrive5_ODParameters_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Overdrive5_ODParameters_Get")) {
                        ADL_Overdrive5_ODParameters_Get_ = ADLImport.ADL_Overdrive5_ODParameters_Get;
                    }
                }
                return ADL_Overdrive5_ODParameters_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Overdrive5_ODParameters_Get ADL_Overdrive5_ODParameters_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Overdrive5_ODParameters_Get_Check = false;
        #endregion ADL_Overdrive5_ODParameters_Get

        #region ADL_Overdrive5_ODPerformanceLevels_Get
        /// <summary> ADL_Overdrive5_ODPerformanceLevels_Get Delegates</summary>
        internal static ADL_Overdrive5_ODPerformanceLevels_Get ADL_Overdrive5_ODPerformanceLevels_Get {
            get {
                if (!ADL_Overdrive5_ODPerformanceLevels_Get_Check && null == ADL_Overdrive5_ODPerformanceLevels_Get_) {
                    ADL_Overdrive5_ODPerformanceLevels_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Overdrive5_ODPerformanceLevels_Get")) {
                        ADL_Overdrive5_ODPerformanceLevels_Get_ = ADLImport.ADL_Overdrive5_ODPerformanceLevels_Get;
                    }
                }
                return ADL_Overdrive5_ODPerformanceLevels_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Overdrive5_ODPerformanceLevels_Get ADL_Overdrive5_ODPerformanceLevels_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Overdrive5_ODPerformanceLevels_Get_Check = false;
        #endregion ADL_Overdrive5_ODPerformanceLevels_Get

        #region ADL_Overdrive5_ODPerformanceLevels_Set
        /// <summary> ADL_Overdrive5_ODPerformanceLevels_Set Delegates</summary>
        internal static ADL_Overdrive5_ODPerformanceLevels_Set ADL_Overdrive5_ODPerformanceLevels_Set {
            get {
                if (!ADL_Overdrive5_ODPerformanceLevels_Set_Check && null == ADL_Overdrive5_ODPerformanceLevels_Set_) {
                    ADL_Overdrive5_ODPerformanceLevels_Set_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Overdrive5_ODPerformanceLevels_Set")) {
                        ADL_Overdrive5_ODPerformanceLevels_Set_ = ADLImport.ADL_Overdrive5_ODPerformanceLevels_Set;
                    }
                }
                return ADL_Overdrive5_ODPerformanceLevels_Set_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Overdrive5_ODPerformanceLevels_Set ADL_Overdrive5_ODPerformanceLevels_Set_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Overdrive5_ODPerformanceLevels_Set_Check = false;
        #endregion ADL_Overdrive5_ODPerformanceLevels_Set

        #region ADL_Overdrive5_FanSpeedToDefault_Set
        /// <summary> ADL_Overdrive5_FanSpeedToDefault_Set Delegates</summary>
        internal static ADL_Overdrive5_FanSpeedToDefault_Set ADL_Overdrive5_FanSpeedToDefault_Set {
            get {
                if (!ADL_Overdrive5_FanSpeedToDefault_Set_Check && null == ADL_Overdrive5_FanSpeedToDefault_Set_) {
                    ADL_Overdrive5_FanSpeedToDefault_Set_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Overdrive5_FanSpeedToDefault_Set")) {
                        ADL_Overdrive5_FanSpeedToDefault_Set_ = ADLImport.ADL_Overdrive5_FanSpeedToDefault_Set;
                    }
                }
                return ADL_Overdrive5_FanSpeedToDefault_Set_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Overdrive5_FanSpeedToDefault_Set ADL_Overdrive5_FanSpeedToDefault_Set_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Overdrive5_FanSpeedToDefault_Set_Check = false;
        #endregion ADL_Overdrive5_FanSpeedToDefault_Set

        #region ADL2_Overdrive_Caps
        /// <summary> ADL2_Overdrive_Caps Delegates</summary>
        internal static ADL2_Overdrive_Caps ADL2_Overdrive_Caps {
            get {
                if (!ADL2_Overdrive_Caps_Check && null == ADL2_Overdrive_Caps_) {
                    ADL2_Overdrive_Caps_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL2_Overdrive_Caps")) {
                        ADL2_Overdrive_Caps_ = ADLImport.ADL2_Overdrive_Caps;
                    }
                }
                return ADL2_Overdrive_Caps_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL2_Overdrive_Caps ADL2_Overdrive_Caps_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL2_Overdrive_Caps_Check = false;
        #endregion ADL2_Overdrive_Caps

        #region ADL_Overdrive6_CurrentPower_Get
        /// <summary> ADL_Overdrive6_CurrentPower_Get Delegates</summary>
        internal static ADL_Overdrive6_CurrentPower_Get ADL_Overdrive6_CurrentPower_Get {
            get {
                if (!ADL_Overdrive6_CurrentPower_Get_Check && null == ADL_Overdrive6_CurrentPower_Get_) {
                    ADL_Overdrive6_CurrentPower_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Overdrive6_CurrentPower_Get")) {
                        ADL_Overdrive6_CurrentPower_Get_ = ADLImport.ADL_Overdrive6_CurrentPower_Get;
                    }
                }
                return ADL_Overdrive6_CurrentPower_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Overdrive6_CurrentPower_Get ADL_Overdrive6_CurrentPower_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Overdrive6_CurrentPower_Get_Check = false;
        #endregion ADL_Overdrive6_CurrentPower_Get

        #region ADL_Overdrive6_VoltageControl_Get
        /// <summary> ADL_Overdrive6_VoltageControl_Get Delegates</summary>
        internal static ADL_Overdrive6_VoltageControl_Get ADL_Overdrive6_VoltageControl_Get {
            get {
                if (!ADL_Overdrive6_VoltageControl_Get_Check && null == ADL_Overdrive6_VoltageControl_Get_) {
                    ADL_Overdrive6_VoltageControl_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Overdrive6_VoltageControl_Get")) {
                        ADL_Overdrive6_VoltageControl_Get_ = ADLImport.ADL_Overdrive6_VoltageControl_Get;
                    }
                }
                return ADL_Overdrive6_VoltageControl_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Overdrive6_VoltageControl_Get ADL_Overdrive6_VoltageControl_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Overdrive6_VoltageControl_Get_Check = false;
        #endregion ADL_Overdrive6_VoltageControl_Get

        #region ADL_Overdrive6_VoltageControl_Set
        /// <summary> ADL_Overdrive6_VoltageControl_Set Delegates</summary>
        internal static ADL_Overdrive6_VoltageControl_Set ADL_Overdrive6_VoltageControl_Set {
            get {
                if (!ADL_Overdrive6_VoltageControl_Set_Check && null == ADL_Overdrive6_VoltageControl_Set_) {
                    ADL_Overdrive6_VoltageControl_Set_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Overdrive6_VoltageControl_Set")) {
                        ADL_Overdrive6_VoltageControl_Set_ = ADLImport.ADL_Overdrive6_VoltageControl_Set;
                    }
                }
                return ADL_Overdrive6_VoltageControl_Set_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Overdrive6_VoltageControl_Set ADL_Overdrive6_VoltageControl_Set_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Overdrive6_VoltageControl_Set_Check = false;
        #endregion ADL_Overdrive6_VoltageControl_Set
        #region ADL2_Overdrive6_CurrentPower_Get
        /// <summary> ADL2_Overdrive6_CurrentPower_Get Delegates</summary>
        internal static ADL2_Overdrive6_CurrentPower_Get ADL2_Overdrive6_CurrentPower_Get {
            get {
                if (!ADL2_Overdrive6_CurrentPower_Get_Check && null == ADL2_Overdrive6_CurrentPower_Get_) {
                    ADL2_Overdrive6_CurrentPower_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL2_Overdrive6_CurrentPower_Get")) {
                        ADL2_Overdrive6_CurrentPower_Get_ = ADLImport.ADL2_Overdrive6_CurrentPower_Get;
                    }
                }
                return ADL2_Overdrive6_CurrentPower_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL2_Overdrive6_CurrentPower_Get ADL2_Overdrive6_CurrentPower_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL2_Overdrive6_CurrentPower_Get_Check = false;
        #endregion ADL2_Overdrive6_CurrentPower_Get

        #region ADL2_Overdrive6_VoltageControl_Get
        /// <summary> ADL2_Overdrive6_VoltageControl_Get Delegates</summary>
        internal static ADL2_Overdrive6_VoltageControl_Get ADL2_Overdrive6_VoltageControl_Get {
            get {
                if (!ADL2_Overdrive6_VoltageControl_Get_Check && null == ADL2_Overdrive6_VoltageControl_Get_) {
                    ADL2_Overdrive6_VoltageControl_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL2_Overdrive6_VoltageControl_Get")) {
                        ADL2_Overdrive6_VoltageControl_Get_ = ADLImport.ADL2_Overdrive6_VoltageControl_Get;
                    }
                }
                return ADL2_Overdrive6_VoltageControl_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL2_Overdrive6_VoltageControl_Get ADL2_Overdrive6_VoltageControl_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL2_Overdrive6_VoltageControl_Get_Check = false;
        #endregion ADL2_Overdrive6_VoltageControl_Get

        #region ADL2_Overdrive6_VoltageControl_Set
        /// <summary> ADL2_Overdrive6_VoltageControl_Set Delegates</summary>
        internal static ADL2_Overdrive6_VoltageControl_Set ADL2_Overdrive6_VoltageControl_Set {
            get {
                if (!ADL2_Overdrive6_VoltageControl_Set_Check && null == ADL2_Overdrive6_VoltageControl_Set_) {
                    ADL2_Overdrive6_VoltageControl_Set_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL2_Overdrive6_VoltageControl_Set")) {
                        ADL2_Overdrive6_VoltageControl_Set_ = ADLImport.ADL2_Overdrive6_VoltageControl_Set;
                    }
                }
                return ADL2_Overdrive6_VoltageControl_Set_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL2_Overdrive6_VoltageControl_Set ADL2_Overdrive6_VoltageControl_Set_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL2_Overdrive6_VoltageControl_Set_Check = false;
        #endregion ADL2_Overdrive6_VoltageControl_Set

        #region ADL2_OverdriveN_PerformanceStatus_Get
        /// <summary> ADL2_OverdriveN_PerformanceStatus_Get Delegates</summary>
        internal static ADL2_OverdriveN_PerformanceStatus_Get ADL2_OverdriveN_PerformanceStatus_Get {
            get {
                if (!ADL2_OverdriveN_PerformanceStatus_Get_Check && null == ADL2_OverdriveN_PerformanceStatus_Get_) {
                    ADL2_OverdriveN_PerformanceStatus_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL2_OverdriveN_PerformanceStatus_Get")) {
                        ADL2_OverdriveN_PerformanceStatus_Get_ = ADLImport.ADL2_OverdriveN_PerformanceStatus_Get;
                    }
                }
                return ADL2_OverdriveN_PerformanceStatus_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL2_OverdriveN_PerformanceStatus_Get ADL2_OverdriveN_PerformanceStatus_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL2_OverdriveN_PerformanceStatus_Get_Check = false;
        #endregion ADL2_OverdriveN_PerformanceStatus_Get

        #region ADL2_OverdriveN_Capabilities_Get
        /// <summary> ADL2_OverdriveN_Capabilities_Get Delegates</summary>
        internal static ADL2_OverdriveN_Capabilities_Get ADL2_OverdriveN_Capabilities_Get {
            get {
                if (!ADL2_OverdriveN_Capabilities_Get_Check && null == ADL2_OverdriveN_Capabilities_Get_) {
                    ADL2_OverdriveN_Capabilities_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL2_OverdriveN_Capabilities_Get")) {
                        ADL2_OverdriveN_Capabilities_Get_ = ADLImport.ADL2_OverdriveN_Capabilities_Get;
                    }
                }
                return ADL2_OverdriveN_Capabilities_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL2_OverdriveN_Capabilities_Get ADL2_OverdriveN_Capabilities_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL2_OverdriveN_Capabilities_Get_Check = false;
        #endregion ADL2_OverdriveN_Capabilities_Get

        #region ADL2_OverdriveN_MemoryClocks_Get
        /// <summary> ADL2_OverdriveN_MemoryClocks_Get Delegates</summary>
        internal static ADL2_OverdriveN_MemoryClocks_Get ADL2_OverdriveN_MemoryClocks_Get {
            get {
                if (!ADL2_OverdriveN_MemoryClocks_Get_Check && null == ADL2_OverdriveN_MemoryClocks_Get_) {
                    ADL2_OverdriveN_MemoryClocks_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL2_OverdriveN_MemoryClocks_Get")) {
                        ADL2_OverdriveN_MemoryClocks_Get_ = ADLImport.ADL2_OverdriveN_MemoryClocks_Get;
                    }
                }
                return ADL2_OverdriveN_MemoryClocks_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL2_OverdriveN_MemoryClocks_Get ADL2_OverdriveN_MemoryClocks_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL2_OverdriveN_MemoryClocks_Get_Check = false;
        #endregion ADL2_OverdriveN_MemoryClocks_Get

        #region ADL2_OverdriveN_MemoryClocks_Set
        /// <summary> ADL2_OverdriveN_MemoryClocks_Set Delegates</summary>
        internal static ADL2_OverdriveN_MemoryClocks_Set ADL2_OverdriveN_MemoryClocks_Set {
            get {
                if (!ADL2_OverdriveN_MemoryClocks_Set_Check && null == ADL2_OverdriveN_MemoryClocks_Set_) {
                    ADL2_OverdriveN_MemoryClocks_Set_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL2_OverdriveN_MemoryClocks_Set")) {
                        ADL2_OverdriveN_MemoryClocks_Set_ = ADLImport.ADL2_OverdriveN_MemoryClocks_Set;
                    }
                }
                return ADL2_OverdriveN_MemoryClocks_Set_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL2_OverdriveN_MemoryClocks_Set ADL2_OverdriveN_MemoryClocks_Set_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL2_OverdriveN_MemoryClocks_Set_Check = false;
        #endregion ADL2_OverdriveN_MemoryClocks_Set

        #region ADL2_OverdriveN_SystemClocks_Get
        /// <summary> ADL2_OverdriveN_SystemClocks_Get Delegates</summary>
        internal static ADL2_OverdriveN_SystemClocks_Get ADL2_OverdriveN_SystemClocks_Get {
            get {
                if (!ADL2_OverdriveN_SystemClocks_Get_Check && null == ADL2_OverdriveN_SystemClocks_Get_) {
                    ADL2_OverdriveN_SystemClocks_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL2_OverdriveN_SystemClocks_Get")) {
                        ADL2_OverdriveN_SystemClocks_Get_ = ADLImport.ADL2_OverdriveN_SystemClocks_Get;
                    }
                }
                return ADL2_OverdriveN_SystemClocks_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL2_OverdriveN_SystemClocks_Get ADL2_OverdriveN_SystemClocks_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL2_OverdriveN_SystemClocks_Get_Check = false;
        #endregion ADL2_OverdriveN_SystemClocks_Get

        #region ADL2_OverdriveN_SystemClocks_Set
        /// <summary> ADL2_OverdriveN_SystemClocks_Set Delegates</summary>
        internal static ADL2_OverdriveN_SystemClocks_Set ADL2_OverdriveN_SystemClocks_Set {
            get {
                if (!ADL2_OverdriveN_SystemClocks_Set_Check && null == ADL2_OverdriveN_SystemClocks_Set_) {
                    ADL2_OverdriveN_SystemClocks_Set_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL2_OverdriveN_SystemClocks_Set")) {
                        ADL2_OverdriveN_SystemClocks_Set_ = ADLImport.ADL2_OverdriveN_SystemClocks_Set;
                    }
                }
                return ADL2_OverdriveN_SystemClocks_Set_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL2_OverdriveN_SystemClocks_Set ADL2_OverdriveN_SystemClocks_Set_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL2_OverdriveN_SystemClocks_Set_Check = false;
        #endregion ADL2_OverdriveN_SystemClocks_Set

        #endregion Export Functions
    }
    #endregion ADL Class
}

#endregion ATI.ADL