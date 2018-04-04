using ATI.ADL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCPUMiner.Hardware
{
    public class OCHandler
    {

        //public int PowerLimit
        //{
        //    get
        //    {
        //        if (ADLAdapterIndex < 0 || null == ADL.ADL_Overdrive5_PowerControl_Get)
        //            return -1;

        //        int currentValue = 0, defaultValue = 0;
        //        if (ADL.ADL_Overdrive5_PowerControl_Get(ADLAdapterIndex, ref currentValue, ref defaultValue) != ADL.ADL_SUCCESS)
        //            return -1;
        //        return 100 + currentValue;
        //    }

        //    set
        //    {
        //        if (ADLAdapterIndex < 0 || null == ADL.ADL_Overdrive5_PowerControl_Set)
        //            return;

        //        ADL.ADL_Overdrive5_PowerControl_Set(ADLAdapterIndex, value < 0 ? 0 : value - 100);
        //    }
        //}

        //public void ResetOverclockingSettings()
        //{
        //    if (ADLAdapterIndex < 0)
        //        return;

        //    // OverDrive 5
        //    ADLODPerformanceLevels OSADLODPerformanceLevelsData = new ADLODPerformanceLevels();
        //    OSADLODPerformanceLevelsData.iReserved = 0;
        //    var levelsBuffer = Marshal.AllocCoTaskMem((int)(OSADLODPerformanceLevelsData.iSize = Marshal.SizeOf(OSADLODPerformanceLevelsData)));
        //    Marshal.StructureToPtr(OSADLODPerformanceLevelsData, levelsBuffer, false);
        //    if (ADL.ADL_Overdrive5_ODPerformanceLevels_Get(ADLAdapterIndex, 1, levelsBuffer) == ADL.ADL_SUCCESS)
        //        ADL.ADL_Overdrive5_ODPerformanceLevels_Set(ADLAdapterIndex, levelsBuffer);
        //    ADL.ADL_Overdrive5_PowerControl_Set(ADLAdapterIndex, 0);

        //    // OverDrive Next
        //    ADLODNPerformanceLevels OSADLODNPerformanceLevelsData = new ADLODNPerformanceLevels();
        //    OSADLODNPerformanceLevelsData.iMode = (int)ADLODNControlType.ODNControlType_Default;
        //    OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels = ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_ODN;
        //    var ODNLevelsBuffer = Marshal.AllocCoTaskMem((int)(OSADLODNPerformanceLevelsData.iSize = Marshal.SizeOf(OSADLODNPerformanceLevelsData)));
        //    Marshal.StructureToPtr(OSADLODNPerformanceLevelsData, ODNLevelsBuffer, false);
        //    ADL.ADL2_OverdriveN_SystemClocks_Set(ADL2Context, ADLAdapterIndex, ODNLevelsBuffer);
        //    ADL.ADL2_OverdriveN_MemoryClocks_Set(ADL2Context, ADLAdapterIndex, ODNLevelsBuffer);
        //}


        //public int CoreClock
        //{
        //    get
        //    {
        //        if (ADLAdapterIndex < 0 || null == ADL.ADL_Overdrive5_CurrentActivity_Get)
        //            return -1;

        //        ADLPMActivity OSADLPMActivityData = new ADLPMActivity();
        //        var activityBuffer = Marshal.AllocCoTaskMem((int)Marshal.SizeOf(OSADLPMActivityData));
        //        Marshal.StructureToPtr(OSADLPMActivityData, activityBuffer, false);
        //        if (ADL.ADL_Overdrive5_CurrentActivity_Get(ADLAdapterIndex, activityBuffer) != ADL.ADL_SUCCESS)
        //            return -1;
        //        OSADLPMActivityData = (ADLPMActivity)Marshal.PtrToStructure(activityBuffer, OSADLPMActivityData.GetType());
        //        return OSADLPMActivityData.iEngineClock / 100;
        //    }

        //    set
        //    {
        //        bool reset = value < 0;
        //        int ret;

        //        if (ADLAdapterIndex < 0)
        //            return;
        //        mCoreClock = value;

        //        // OverDrive 5
        //        ADLODPerformanceLevels OSADLODPerformanceLevelsData = new ADLODPerformanceLevels();
        //        OSADLODPerformanceLevelsData.iReserved = 0;
        //        var levelsBuffer = Marshal.AllocCoTaskMem((int)(OSADLODPerformanceLevelsData.iSize = Marshal.SizeOf(OSADLODPerformanceLevelsData)));
        //        Marshal.StructureToPtr(OSADLODPerformanceLevelsData, levelsBuffer, false);
        //        if ((ret = ADL.ADL_Overdrive5_ODPerformanceLevels_Get(ADLAdapterIndex, reset ? 1 : 0, levelsBuffer)) == ADL.ADL_SUCCESS)
        //        {
        //            OSADLODPerformanceLevelsData = (ADLODPerformanceLevels)Marshal.PtrToStructure(levelsBuffer, OSADLODPerformanceLevelsData.GetType());
        //            //
        //            if (!reset)
        //            {
        //                for (int i = 1; i < ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_OD5; ++i)
        //                {
        //                    OSADLODPerformanceLevelsData.aLevels[i] = OSADLODPerformanceLevelsData.aLevels[ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_OD5 - 1];
        //                    if (mCoreClock >= 0) OSADLODPerformanceLevelsData.aLevels[i].iEngineClock = mCoreClock * 100;
        //                    if (mMemoryClock >= 0) OSADLODPerformanceLevelsData.aLevels[i].iMemoryClock = mMemoryClock * 100;
        //                    if (mCoreVoltage >= 0) OSADLODPerformanceLevelsData.aLevels[i].iVddc = mCoreVoltage;
        //                }
        //            }
        //            Marshal.StructureToPtr(OSADLODPerformanceLevelsData, levelsBuffer, false);
        //            ADL.ADL_Overdrive5_ODPerformanceLevels_Set(ADLAdapterIndex, levelsBuffer);
        //        }

        //        // OverDrive Next
        //        ADLODNPerformanceLevels OSADLODNPerformanceLevelsData = new ADLODNPerformanceLevels();
        //        OSADLODNPerformanceLevelsData.iMode = (int)(reset ? ADLODNControlType.ODNControlType_Default : ADLODNControlType.ODNControlType_Current);
        //        OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels = ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_ODN;
        //        var ODNLevelsBuffer = Marshal.AllocCoTaskMem((int)(OSADLODNPerformanceLevelsData.iSize = Marshal.SizeOf(OSADLODNPerformanceLevelsData)));
        //        Marshal.StructureToPtr(OSADLODNPerformanceLevelsData, ODNLevelsBuffer, false);
        //        if (ADL.ADL2_OverdriveN_SystemClocks_Get(ADL2Context, ADLAdapterIndex, ODNLevelsBuffer) != ADL.ADL_SUCCESS)
        //            return;
        //        OSADLODNPerformanceLevelsData = (ADLODNPerformanceLevels)Marshal.PtrToStructure(ODNLevelsBuffer, OSADLODNPerformanceLevelsData.GetType());
        //        //
        //        OSADLODNPerformanceLevelsData.iMode = (int)ADLODNControlType.ODNControlType_Manual;
        //        if (!reset)
        //        {
        //            int sourceIndex = 1;
        //            for (int i = 1; i < OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels; ++i)
        //                if (OSADLODNPerformanceLevelsData.aLevels[i].iEnabled != 0)
        //                    sourceIndex = i;
        //            for (int i = 1; i < OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels; ++i)
        //            {
        //                OSADLODNPerformanceLevelsData.aLevels[i].iClock = mCoreClock * 100;
        //                OSADLODNPerformanceLevelsData.aLevels[i].iVddc = (mCoreVoltage >= 0) ? mCoreVoltage : OSADLODNPerformanceLevelsData.aLevels[sourceIndex].iVddc;
        //            }
        //        }
        //        Marshal.StructureToPtr(OSADLODNPerformanceLevelsData, ODNLevelsBuffer, false);
        //        ADL.ADL2_OverdriveN_SystemClocks_Set(ADL2Context, ADLAdapterIndex, ODNLevelsBuffer);
        //    }
        //}

        //public int CoreVoltage
        //{
        //    get
        //    {
        //        if (ADLAdapterIndex < 0 || null == ADL.ADL_Overdrive5_CurrentActivity_Get)
        //            return -1;

        //        // activity
        //        ADLPMActivity OSADLPMActivityData = new ADLPMActivity();
        //        var activityBuffer = Marshal.AllocCoTaskMem((int)Marshal.SizeOf(OSADLPMActivityData));
        //        Marshal.StructureToPtr(OSADLPMActivityData, activityBuffer, false);
        //        if (ADL.ADL_Overdrive5_CurrentActivity_Get(ADLAdapterIndex, activityBuffer) == ADL.ADL_SUCCESS)
        //        {
        //            OSADLPMActivityData = (ADLPMActivity)Marshal.PtrToStructure(activityBuffer, OSADLPMActivityData.GetType());
        //            if (OSADLPMActivityData.iVddc > 1)
        //                return OSADLPMActivityData.iVddc;
        //        }

        //        ADLODNPerformanceStatus OSODNPerformanceStatusData = new ADLODNPerformanceStatus();
        //        var statusBuffer = Marshal.AllocCoTaskMem((int)Marshal.SizeOf(OSODNPerformanceStatusData));
        //        Marshal.StructureToPtr(OSODNPerformanceStatusData, statusBuffer, false);
        //        if (ADL.ADL2_OverdriveN_PerformanceStatus_Get(ADL2Context, ADLAdapterIndex, statusBuffer) != ADL.ADL_SUCCESS)
        //            return -1;
        //        OSODNPerformanceStatusData = (ADLODNPerformanceStatus)Marshal.PtrToStructure(statusBuffer, OSODNPerformanceStatusData.GetType());
        //        return (OSODNPerformanceStatusData.iVDDC < 800 || OSODNPerformanceStatusData.iVDDC > 2000) ? -1 : OSODNPerformanceStatusData.iVDDC; // The driver may return garbage.
        //    }

        //    set
        //    {
        //        bool reset = value < 0;
        //        int ret;

        //        if (ADLAdapterIndex < 0)
        //            return;
        //        mCoreVoltage = value;

        //        // OverDrive 5
        //        ADLODPerformanceLevels OSADLODPerformanceLevelsData = new ADLODPerformanceLevels();
        //        OSADLODPerformanceLevelsData.iReserved = 0;
        //        var levelsBuffer = Marshal.AllocCoTaskMem((int)(OSADLODPerformanceLevelsData.iSize = Marshal.SizeOf(OSADLODPerformanceLevelsData)));
        //        Marshal.StructureToPtr(OSADLODPerformanceLevelsData, levelsBuffer, false);
        //        if ((ret = ADL.ADL_Overdrive5_ODPerformanceLevels_Get(ADLAdapterIndex, reset ? 1 : 0, levelsBuffer)) == ADL.ADL_SUCCESS)
        //        {
        //            OSADLODPerformanceLevelsData = (ADLODPerformanceLevels)Marshal.PtrToStructure(levelsBuffer, OSADLODPerformanceLevelsData.GetType());
        //            //
        //            if (!reset)
        //            {
        //                for (int i = 1; i < ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_OD5; ++i)
        //                {
        //                    OSADLODPerformanceLevelsData.aLevels[i] = OSADLODPerformanceLevelsData.aLevels[ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_OD5 - 1];
        //                    if (mCoreClock >= 0) OSADLODPerformanceLevelsData.aLevels[i].iEngineClock = mCoreClock * 100;
        //                    if (mMemoryClock >= 0) OSADLODPerformanceLevelsData.aLevels[i].iMemoryClock = mMemoryClock * 100;
        //                    if (mCoreVoltage >= 0) OSADLODPerformanceLevelsData.aLevels[i].iVddc = mCoreVoltage;
        //                }
        //            }
        //            Marshal.StructureToPtr(OSADLODPerformanceLevelsData, levelsBuffer, false);
        //            ADL.ADL_Overdrive5_ODPerformanceLevels_Set(ADLAdapterIndex, levelsBuffer);
        //        }

        //        // OverDrive Next
        //        ADLODNPerformanceLevels OSADLODNPerformanceLevelsData = new ADLODNPerformanceLevels();
        //        OSADLODNPerformanceLevelsData.iMode = (int)(reset ? ADLODNControlType.ODNControlType_Default : ADLODNControlType.ODNControlType_Current);
        //        OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels = ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_ODN;
        //        var ODNLevelsBuffer = Marshal.AllocCoTaskMem((int)(OSADLODNPerformanceLevelsData.iSize = Marshal.SizeOf(OSADLODNPerformanceLevelsData)));
        //        Marshal.StructureToPtr(OSADLODNPerformanceLevelsData, ODNLevelsBuffer, false);
        //        if (ADL.ADL2_OverdriveN_SystemClocks_Get(ADL2Context, ADLAdapterIndex, ODNLevelsBuffer) != ADL.ADL_SUCCESS)
        //            return;
        //        OSADLODNPerformanceLevelsData = (ADLODNPerformanceLevels)Marshal.PtrToStructure(ODNLevelsBuffer, OSADLODNPerformanceLevelsData.GetType());
        //        //
        //        OSADLODNPerformanceLevelsData.iMode = (int)ADLODNControlType.ODNControlType_Manual;
        //        if (!reset)
        //        {
        //            int sourceIndex = 1;
        //            for (int i = 1; i < OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels; ++i)
        //                if (OSADLODNPerformanceLevelsData.aLevels[i].iEnabled != 0)
        //                    sourceIndex = i;
        //            for (int i = 1; i < OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels; ++i)
        //            {
        //                OSADLODNPerformanceLevelsData.aLevels[i].iClock = (mCoreClock >= 0) ? (mCoreClock * 100) : OSADLODNPerformanceLevelsData.aLevels[sourceIndex].iClock;
        //                OSADLODNPerformanceLevelsData.aLevels[i].iVddc = mCoreVoltage;
        //            }
        //        }
        //        Marshal.StructureToPtr(OSADLODNPerformanceLevelsData, ODNLevelsBuffer, false);
        //        ADL.ADL2_OverdriveN_SystemClocks_Set(ADL2Context, ADLAdapterIndex, ODNLevelsBuffer);
        //    }
        //}

        //public int MemoryClock
        //{
        //    get
        //    {
        //        if (ADLAdapterIndex < 0 || null == ADL.ADL_Overdrive5_CurrentActivity_Get)
        //            return -1;

        //        // activity
        //        ADLPMActivity OSADLPMActivityData;
        //        OSADLPMActivityData = new ADLPMActivity();
        //        var activityBuffer = IntPtr.Zero;
        //        var size = Marshal.SizeOf(OSADLPMActivityData);
        //        activityBuffer = Marshal.AllocCoTaskMem((int)size);
        //        Marshal.StructureToPtr(OSADLPMActivityData, activityBuffer, false);
        //        if (ADL.ADL_Overdrive5_CurrentActivity_Get(ADLAdapterIndex, activityBuffer) != ADL.ADL_SUCCESS)
        //            return -1;
        //        OSADLPMActivityData = (ADLPMActivity)Marshal.PtrToStructure(activityBuffer, OSADLPMActivityData.GetType());
        //        return OSADLPMActivityData.iMemoryClock / 100;
        //    }

        //    set
        //    {
        //        bool reset = value < 0;
        //        int ret;

        //        if (ADLAdapterIndex < 0)
        //            return;
        //        mMemoryClock = value;

        //        // OverDrive 5
        //        ADLODPerformanceLevels OSADLODPerformanceLevelsData = new ADLODPerformanceLevels();
        //        OSADLODPerformanceLevelsData.iReserved = 0;
        //        var levelsBuffer = Marshal.AllocCoTaskMem((int)(OSADLODPerformanceLevelsData.iSize = Marshal.SizeOf(OSADLODPerformanceLevelsData)));
        //        Marshal.StructureToPtr(OSADLODPerformanceLevelsData, levelsBuffer, false);
        //        if ((ret = ADL.ADL_Overdrive5_ODPerformanceLevels_Get(ADLAdapterIndex, reset ? 1 : 0, levelsBuffer)) == ADL.ADL_SUCCESS)
        //        {
        //            OSADLODPerformanceLevelsData = (ADLODPerformanceLevels)Marshal.PtrToStructure(levelsBuffer, OSADLODPerformanceLevelsData.GetType());
        //            //
        //            if (!reset)
        //            {
        //                for (int i = 1; i < ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_OD5; ++i)
        //                {
        //                    OSADLODPerformanceLevelsData.aLevels[i].iVddc = OSADLODPerformanceLevelsData.aLevels[ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_OD5 - 1].iVddc;
        //                    if (mCoreClock >= 0) OSADLODPerformanceLevelsData.aLevels[i].iEngineClock = mCoreClock * 100;
        //                    if (mMemoryClock >= 0) OSADLODPerformanceLevelsData.aLevels[i].iMemoryClock = mMemoryClock * 100;
        //                    if (mCoreVoltage >= 0) OSADLODPerformanceLevelsData.aLevels[i].iVddc = mCoreVoltage;
        //                }
        //            }
        //            Marshal.StructureToPtr(OSADLODPerformanceLevelsData, levelsBuffer, false);
        //            ADL.ADL_Overdrive5_ODPerformanceLevels_Set(ADLAdapterIndex, levelsBuffer);
        //        }

        //        // OverDrive Next
        //        ADLODNPerformanceLevels OSADLODNPerformanceLevelsData = new ADLODNPerformanceLevels();
        //        OSADLODNPerformanceLevelsData.iMode = (int)(reset ? ADLODNControlType.ODNControlType_Default : ADLODNControlType.ODNControlType_Current);
        //        OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels = ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_ODN;
        //        var ODNLevelsBuffer = Marshal.AllocCoTaskMem((int)(OSADLODNPerformanceLevelsData.iSize = Marshal.SizeOf(OSADLODNPerformanceLevelsData)));
        //        Marshal.StructureToPtr(OSADLODNPerformanceLevelsData, ODNLevelsBuffer, false);
        //        if (ADL.ADL2_OverdriveN_MemoryClocks_Get(ADL2Context, ADLAdapterIndex, ODNLevelsBuffer) != ADL.ADL_SUCCESS)
        //            return;
        //        OSADLODNPerformanceLevelsData = (ADLODNPerformanceLevels)Marshal.PtrToStructure(ODNLevelsBuffer, OSADLODNPerformanceLevelsData.GetType());
        //        //
        //        OSADLODNPerformanceLevelsData.iMode = (int)ADLODNControlType.ODNControlType_Manual;
        //        if (!reset)
        //        {
        //            int sourceIndex = 1;
        //            for (int i = 1; i < OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels; ++i)
        //                if (OSADLODNPerformanceLevelsData.aLevels[i].iEnabled != 0)
        //                    sourceIndex = i;
        //            for (int i = 1; i < OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels; ++i)
        //            {
        //                OSADLODNPerformanceLevelsData.aLevels[i].iClock = mMemoryClock * 100;
        //                OSADLODNPerformanceLevelsData.aLevels[i].iVddc = (mMemoryVoltage >= 0) ? mMemoryVoltage : OSADLODNPerformanceLevelsData.aLevels[sourceIndex].iVddc;
        //            }
        //        }
        //        Marshal.StructureToPtr(OSADLODNPerformanceLevelsData, ODNLevelsBuffer, false);
        //        ADL.ADL2_OverdriveN_MemoryClocks_Set(ADL2Context, ADLAdapterIndex, ODNLevelsBuffer);
        //    }
        //}

        //public int MemoryVoltage
        //{
        //    get
        //    {
        //        if (ADLAdapterIndex < 0 || null == ADL.ADL2_OverdriveN_PerformanceStatus_Get)
        //            return -1;

        //        ADLODNPerformanceStatus OSODNPerformanceStatusData = new ADLODNPerformanceStatus();
        //        var statusBuffer = Marshal.AllocCoTaskMem((int)(Marshal.SizeOf(OSODNPerformanceStatusData)));
        //        Marshal.StructureToPtr(OSODNPerformanceStatusData, statusBuffer, false);
        //        if (ADL.ADL2_OverdriveN_PerformanceStatus_Get(ADL2Context, ADLAdapterIndex, statusBuffer) != ADL.ADL_SUCCESS)
        //            return -1;
        //        OSODNPerformanceStatusData = (ADLODNPerformanceStatus)Marshal.PtrToStructure(statusBuffer, OSODNPerformanceStatusData.GetType());
        //        return (OSODNPerformanceStatusData.iVDDC < 800 || OSODNPerformanceStatusData.iVDDC > 2000) ? -1 : OSODNPerformanceStatusData.iVDDC; // The driver may return garbage.
        //    }

        //    set
        //    {
        //        bool reset = value < 0;

        //        if (ADLAdapterIndex < 0)
        //            return;
        //        mMemoryVoltage = value;

        //        // OverDrive Next
        //        ADLODNPerformanceLevels OSADLODNPerformanceLevelsData = new ADLODNPerformanceLevels();
        //        OSADLODNPerformanceLevelsData.iMode = (int)(reset ? ADLODNControlType.ODNControlType_Default : ADLODNControlType.ODNControlType_Current);
        //        OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels = ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_ODN;
        //        var ODNLevelsBuffer = Marshal.AllocCoTaskMem((int)(OSADLODNPerformanceLevelsData.iSize = Marshal.SizeOf(OSADLODNPerformanceLevelsData)));
        //        Marshal.StructureToPtr(OSADLODNPerformanceLevelsData, ODNLevelsBuffer, false);
        //        if (ADL.ADL2_OverdriveN_MemoryClocks_Get(ADL2Context, ADLAdapterIndex, ODNLevelsBuffer) != ADL.ADL_SUCCESS)
        //            return;
        //        OSADLODNPerformanceLevelsData = (ADLODNPerformanceLevels)Marshal.PtrToStructure(ODNLevelsBuffer, OSADLODNPerformanceLevelsData.GetType());
        //        //
        //        OSADLODNPerformanceLevelsData.iMode = (int)ADLODNControlType.ODNControlType_Manual;
        //        if (!reset)
        //        {
        //            int sourceIndex = 1;
        //            for (int i = 1; i < OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels; ++i)
        //                if (OSADLODNPerformanceLevelsData.aLevels[i].iEnabled != 0)
        //                    sourceIndex = i;
        //            for (int i = 1; i < OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels; ++i)
        //            {
        //                OSADLODNPerformanceLevelsData.aLevels[i].iClock = (mMemoryClock >= 0) ? (mMemoryClock * 100) : OSADLODNPerformanceLevelsData.aLevels[sourceIndex].iClock;
        //                OSADLODNPerformanceLevelsData.aLevels[i].iVddc = mMemoryVoltage;
        //            }
        //        }
        //        Marshal.StructureToPtr(OSADLODNPerformanceLevelsData, ODNLevelsBuffer, false);
        //        ADL.ADL2_OverdriveN_MemoryClocks_Set(ADL2Context, ADLAdapterIndex, ODNLevelsBuffer);
        //    }
        //}

        //public int MaxCoreClock
        //{
        //    get
        //    {
        //        // OverDrive 5
        //        var OSADLParametersData = new ADLODParameters();
        //        var parametersBuffer = Marshal.AllocCoTaskMem((int)(OSADLParametersData.iSize = Marshal.SizeOf(OSADLParametersData)));
        //        Marshal.StructureToPtr(OSADLParametersData, parametersBuffer, false);
        //        if (ADL.ADL_Overdrive5_ODParameters_Get(ADLAdapterIndex, parametersBuffer) == ADL.ADL_SUCCESS)
        //        {
        //            OSADLParametersData = (ADLODParameters)Marshal.PtrToStructure(parametersBuffer, OSADLParametersData.GetType());
        //            return OSADLParametersData.sEngineClockRange.iMax / 100;
        //        }

        //        return -1;
        //    }
        //}

        //public int MinCoreClock
        //{
        //    get
        //    {
        //        // OverDrive 5
        //        var OSADLParametersData = new ADLODParameters();
        //        var parametersBuffer = Marshal.AllocCoTaskMem((int)(OSADLParametersData.iSize = Marshal.SizeOf(OSADLParametersData)));
        //        Marshal.StructureToPtr(OSADLParametersData, parametersBuffer, false);
        //        if (ADL.ADL_Overdrive5_ODParameters_Get(ADLAdapterIndex, parametersBuffer) == ADL.ADL_SUCCESS)
        //        {
        //            OSADLParametersData = (ADLODParameters)Marshal.PtrToStructure(parametersBuffer, OSADLParametersData.GetType());
        //            return OSADLParametersData.sEngineClockRange.iMin / 100;
        //        }

        //        return -1;
        //    }
        //}

        //public int CoreClockStep
        //{
        //    get
        //    {
        //        // OverDrive 5
        //        var OSADLParametersData = new ADLODParameters();
        //        var parametersBuffer = Marshal.AllocCoTaskMem((int)(OSADLParametersData.iSize = Marshal.SizeOf(OSADLParametersData)));
        //        Marshal.StructureToPtr(OSADLParametersData, parametersBuffer, false);
        //        if (ADL.ADL_Overdrive5_ODParameters_Get(ADLAdapterIndex, parametersBuffer) == ADL.ADL_SUCCESS)
        //        {
        //            OSADLParametersData = (ADLODParameters)Marshal.PtrToStructure(parametersBuffer, OSADLParametersData.GetType());
        //            return OSADLParametersData.sEngineClockRange.iStep / 100;
        //        }

        //        return -1;
        //    }
        //}


        //public int MaxMemoryClock
        //{
        //    get
        //    {
        //        // OverDrive 5
        //        var OSADLParametersData = new ADLODParameters();
        //        var parametersBuffer = Marshal.AllocCoTaskMem((int)(OSADLParametersData.iSize = Marshal.SizeOf(OSADLParametersData)));
        //        Marshal.StructureToPtr(OSADLParametersData, parametersBuffer, false);
        //        if (ADL.ADL_Overdrive5_ODParameters_Get(ADLAdapterIndex, parametersBuffer) == ADL.ADL_SUCCESS)
        //        {
        //            OSADLParametersData = (ADLODParameters)Marshal.PtrToStructure(parametersBuffer, OSADLParametersData.GetType());
        //            return OSADLParametersData.sMemoryClockRange.iMax / 100;
        //        }

        //        return -1;
        //    }
        //}

        //public int MinMemoryClock
        //{
        //    get
        //    {
        //        // OverDrive 5
        //        var OSADLParametersData = new ADLODParameters();
        //        var parametersBuffer = Marshal.AllocCoTaskMem((int)(OSADLParametersData.iSize = Marshal.SizeOf(OSADLParametersData)));
        //        Marshal.StructureToPtr(OSADLParametersData, parametersBuffer, false);
        //        if (ADL.ADL_Overdrive5_ODParameters_Get(ADLAdapterIndex, parametersBuffer) == ADL.ADL_SUCCESS)
        //        {
        //            OSADLParametersData = (ADLODParameters)Marshal.PtrToStructure(parametersBuffer, OSADLParametersData.GetType());
        //            return OSADLParametersData.sMemoryClockRange.iMin / 100;
        //        }

        //        return -1;
        //    }
        //}

        //public int MemoryClockStep
        //{
        //    get
        //    {
        //        // OverDrive 5
        //        var OSADLParametersData = new ADLODParameters();
        //        var parametersBuffer = Marshal.AllocCoTaskMem((int)(OSADLParametersData.iSize = Marshal.SizeOf(OSADLParametersData)));
        //        Marshal.StructureToPtr(OSADLParametersData, parametersBuffer, false);
        //        if (ADL.ADL_Overdrive5_ODParameters_Get(ADLAdapterIndex, parametersBuffer) == ADL.ADL_SUCCESS)
        //        {
        //            OSADLParametersData = (ADLODParameters)Marshal.PtrToStructure(parametersBuffer, OSADLParametersData.GetType());
        //            return OSADLParametersData.sMemoryClockRange.iStep / 100;
        //        }

        //        return -1;
        //    }
        //}

        //public int DefaultCoreClock
        //{
        //    get
        //    {
        //        // OverDrive 5
        //        ADLODPerformanceLevels OSADLODPerformanceLevelsData = new ADLODPerformanceLevels();
        //        OSADLODPerformanceLevelsData.iReserved = 0;
        //        var levelsBuffer = Marshal.AllocCoTaskMem((int)(OSADLODPerformanceLevelsData.iSize = Marshal.SizeOf(OSADLODPerformanceLevelsData)));
        //        Marshal.StructureToPtr(OSADLODPerformanceLevelsData, levelsBuffer, false);
        //        if (ADL.ADL_Overdrive5_ODPerformanceLevels_Get(ADLAdapterIndex, 1, levelsBuffer) == ADL.ADL_SUCCESS)
        //        {
        //            OSADLODPerformanceLevelsData = (ADLODPerformanceLevels)Marshal.PtrToStructure(levelsBuffer, OSADLODPerformanceLevelsData.GetType());
        //            return OSADLODPerformanceLevelsData.aLevels[ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_OD5 - 1].iEngineClock / 100;
        //        }

        //        // OverDrive Next
        //        ADLODNPerformanceLevels OSADLODNPerformanceLevelsData = new ADLODNPerformanceLevels();
        //        OSADLODNPerformanceLevelsData.iMode = (int)ADLODNControlType.ODNControlType_Default;
        //        OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels = ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_ODN;
        //        var ODNLevelsBuffer = Marshal.AllocCoTaskMem((int)(OSADLODNPerformanceLevelsData.iSize = Marshal.SizeOf(OSADLODNPerformanceLevelsData)));
        //        Marshal.StructureToPtr(OSADLODNPerformanceLevelsData, ODNLevelsBuffer, false);
        //        if (ADL.ADL2_OverdriveN_SystemClocks_Get(ADL2Context, ADLAdapterIndex, ODNLevelsBuffer) != ADL.ADL_SUCCESS)
        //            return -1;
        //        OSADLODNPerformanceLevelsData = (ADLODNPerformanceLevels)Marshal.PtrToStructure(ODNLevelsBuffer, OSADLODNPerformanceLevelsData.GetType());
        //        //
        //        int sourceIndex = -1;
        //        for (int i = 0; i < OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels; ++i)
        //            if (OSADLODNPerformanceLevelsData.aLevels[i].iEnabled != 0)
        //                sourceIndex = i;
        //        return (sourceIndex < 0) ? -1 : OSADLODNPerformanceLevelsData.aLevels[sourceIndex].iClock / 100;
        //    }
        //}

        //public int DefaultMemoryClock
        //{
        //    get
        //    {
        //        // OverDrive 5
        //        ADLODPerformanceLevels OSADLODPerformanceLevelsData = new ADLODPerformanceLevels();
        //        var levelsBuffer = Marshal.AllocCoTaskMem((int)(OSADLODPerformanceLevelsData.iSize = Marshal.SizeOf(OSADLODPerformanceLevelsData)));
        //        Marshal.StructureToPtr(OSADLODPerformanceLevelsData, levelsBuffer, false);
        //        if (ADL.ADL_Overdrive5_ODPerformanceLevels_Get(ADLAdapterIndex, 1, levelsBuffer) == ADL.ADL_SUCCESS)
        //        {
        //            OSADLODPerformanceLevelsData = (ADLODPerformanceLevels)Marshal.PtrToStructure(levelsBuffer, OSADLODPerformanceLevelsData.GetType());
        //            return OSADLODPerformanceLevelsData.aLevels[ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_OD5 - 1].iMemoryClock / 100;
        //        }

        //        // OverDrive Next
        //        ADLODNPerformanceLevels OSADLODNPerformanceLevelsData = new ADLODNPerformanceLevels();
        //        OSADLODNPerformanceLevelsData.iMode = (int)ADLODNControlType.ODNControlType_Default;
        //        OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels = ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_ODN;
        //        var ODNLevelsBuffer = Marshal.AllocCoTaskMem((int)(OSADLODNPerformanceLevelsData.iSize = Marshal.SizeOf(OSADLODNPerformanceLevelsData)));
        //        Marshal.StructureToPtr(OSADLODNPerformanceLevelsData, ODNLevelsBuffer, false);
        //        if (ADL.ADL2_OverdriveN_MemoryClocks_Get(ADL2Context, ADLAdapterIndex, ODNLevelsBuffer) != ADL.ADL_SUCCESS)
        //            return -1;
        //        OSADLODNPerformanceLevelsData = (ADLODNPerformanceLevels)Marshal.PtrToStructure(ODNLevelsBuffer, OSADLODNPerformanceLevelsData.GetType());
        //        //
        //        int sourceIndex = -1;
        //        for (int i = 0; i < OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels; ++i)
        //            if (OSADLODNPerformanceLevelsData.aLevels[i].iEnabled != 0)
        //                sourceIndex = i;
        //        return (sourceIndex < 0) ? -1 : OSADLODNPerformanceLevelsData.aLevels[sourceIndex].iClock / 100;
        //    }
        //}

        //public int DefaultCoreVoltage
        //{
        //    get
        //    {
        //        // OverDrive 5
        //        ADLODPerformanceLevels OSADLODPerformanceLevelsData = new ADLODPerformanceLevels();
        //        OSADLODPerformanceLevelsData.iReserved = 0;
        //        var levelsBuffer = Marshal.AllocCoTaskMem((int)(OSADLODPerformanceLevelsData.iSize = Marshal.SizeOf(OSADLODPerformanceLevelsData)));
        //        Marshal.StructureToPtr(OSADLODPerformanceLevelsData, levelsBuffer, false);
        //        if (ADL.ADL_Overdrive5_ODPerformanceLevels_Get(ADLAdapterIndex, 1, levelsBuffer) == ADL.ADL_SUCCESS)
        //        {
        //            OSADLODPerformanceLevelsData = (ADLODPerformanceLevels)Marshal.PtrToStructure(levelsBuffer, OSADLODPerformanceLevelsData.GetType());
        //            return OSADLODPerformanceLevelsData.aLevels[ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_OD5 - 1].iVddc;
        //        }

        //        // OverDrive Next
        //        ADLODNPerformanceLevels OSADLODNPerformanceLevelsData = new ADLODNPerformanceLevels();
        //        OSADLODNPerformanceLevelsData.iMode = (int)ADLODNControlType.ODNControlType_Default;
        //        OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels = ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_ODN;
        //        var ODNLevelsBuffer = Marshal.AllocCoTaskMem((int)(OSADLODNPerformanceLevelsData.iSize = Marshal.SizeOf(OSADLODNPerformanceLevelsData)));
        //        Marshal.StructureToPtr(OSADLODNPerformanceLevelsData, ODNLevelsBuffer, false);
        //        if (ADL.ADL2_OverdriveN_SystemClocks_Get(ADL2Context, ADLAdapterIndex, ODNLevelsBuffer) != ADL.ADL_SUCCESS)
        //            return -1;
        //        OSADLODNPerformanceLevelsData = (ADLODNPerformanceLevels)Marshal.PtrToStructure(ODNLevelsBuffer, OSADLODNPerformanceLevelsData.GetType());
        //        //
        //        int sourceIndex = -1;
        //        for (int i = 0; i < OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels; ++i)
        //            if (OSADLODNPerformanceLevelsData.aLevels[i].iEnabled != 0)
        //                sourceIndex = i;
        //        return (sourceIndex < 0) ? -1 : OSADLODNPerformanceLevelsData.aLevels[sourceIndex].iVddc;
        //    }
        //}

        //public int DefaultMemoryVoltage
        //{
        //    get
        //    {
        //        // OverDrive Next
        //        ADLODNPerformanceLevels OSADLODNPerformanceLevelsData = new ADLODNPerformanceLevels();
        //        OSADLODNPerformanceLevelsData.iMode = (int)ADLODNControlType.ODNControlType_Default;
        //        OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels = ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_ODN;
        //        var ODNLevelsBuffer = Marshal.AllocCoTaskMem((int)(OSADLODNPerformanceLevelsData.iSize = Marshal.SizeOf(OSADLODNPerformanceLevelsData)));
        //        Marshal.StructureToPtr(OSADLODNPerformanceLevelsData, ODNLevelsBuffer, false);
        //        if (ADL.ADL2_OverdriveN_MemoryClocks_Get(ADL2Context, ADLAdapterIndex, ODNLevelsBuffer) != ADL.ADL_SUCCESS)
        //            return -1;
        //        OSADLODNPerformanceLevelsData = (ADLODNPerformanceLevels)Marshal.PtrToStructure(ODNLevelsBuffer, OSADLODNPerformanceLevelsData.GetType());
        //        //
        //        int sourceIndex = -1;
        //        for (int i = 0; i < OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels; ++i)
        //            if (OSADLODNPerformanceLevelsData.aLevels[i].iEnabled != 0)
        //                sourceIndex = i;
        //        return (sourceIndex < 0) ? -1 : OSADLODNPerformanceLevelsData.aLevels[sourceIndex].iVddc;
        //    }
        //}

        //int mPowerControlBackup;
        //ADLODPerformanceLevels sODPerformanceLevelsBackup;
        //ADLODNPerformanceLevels sODNSystemClocksBackup;
        //ADLODNPerformanceLevels sODNMemoryClocksBackup;

        //public void SaveOverclockingSettings()
        //{
        //    if (ADLAdapterIndex < 0)
        //        return;

        //    int defaultPowerControl = 0;
        //    ADL.ADL_Overdrive5_PowerControl_Get(ADLAdapterIndex, ref mPowerControlBackup, ref defaultPowerControl);

        //    // OverDrive 5
        //    ADLODPerformanceLevels OSADLODPerformanceLevelsData = new ADLODPerformanceLevels();
        //    var levelsBuffer = Marshal.AllocCoTaskMem((int)(OSADLODPerformanceLevelsData.iSize = Marshal.SizeOf(OSADLODPerformanceLevelsData)));
        //    Marshal.StructureToPtr(OSADLODPerformanceLevelsData, levelsBuffer, false);
        //    if (ADL.ADL_Overdrive5_ODPerformanceLevels_Get(ADLAdapterIndex, 0, levelsBuffer) == ADL.ADL_SUCCESS)
        //        sODPerformanceLevelsBackup = (ADLODPerformanceLevels)Marshal.PtrToStructure(levelsBuffer, OSADLODPerformanceLevelsData.GetType());

        //    // OverDrive Next (System Clocks)
        //    ADLODNPerformanceLevels OSADLODNPerformanceLevelsData = new ADLODNPerformanceLevels();
        //    OSADLODNPerformanceLevelsData.iMode = (int)(ADLODNControlType.ODNControlType_Current);
        //    OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels = ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_ODN;
        //    var ODNLevelsBuffer = Marshal.AllocCoTaskMem((int)(OSADLODNPerformanceLevelsData.iSize = Marshal.SizeOf(OSADLODNPerformanceLevelsData)));
        //    Marshal.StructureToPtr(OSADLODNPerformanceLevelsData, ODNLevelsBuffer, false);
        //    if (ADL.ADL2_OverdriveN_SystemClocks_Get(ADL2Context, ADLAdapterIndex, ODNLevelsBuffer) == ADL.ADL_SUCCESS)
        //        sODNSystemClocksBackup = (ADLODNPerformanceLevels)Marshal.PtrToStructure(ODNLevelsBuffer, OSADLODNPerformanceLevelsData.GetType());

        //    // OverDrive Next (Memory Clocks)
        //    OSADLODNPerformanceLevelsData = new ADLODNPerformanceLevels(); // zero clear
        //    OSADLODNPerformanceLevelsData.iMode = (int)(ADLODNControlType.ODNControlType_Current);
        //    OSADLODNPerformanceLevelsData.iNumberOfPerformanceLevels = ADL.ADL_MAX_NUM_PERFORMANCE_LEVELS_ODN;
        //    ODNLevelsBuffer = Marshal.AllocCoTaskMem((int)(OSADLODNPerformanceLevelsData.iSize = Marshal.SizeOf(OSADLODNPerformanceLevelsData)));
        //    Marshal.StructureToPtr(OSADLODNPerformanceLevelsData, ODNLevelsBuffer, false);
        //    if (ADL.ADL2_OverdriveN_MemoryClocks_Get(ADL2Context, ADLAdapterIndex, ODNLevelsBuffer) == ADL.ADL_SUCCESS)
        //        sODNMemoryClocksBackup = (ADLODNPerformanceLevels)Marshal.PtrToStructure(ODNLevelsBuffer, OSADLODNPerformanceLevelsData.GetType());
        //}

        //public void RestoreOverclockingSettings()
        //{
        //    if (ADLAdapterIndex < 0)
        //        return;

        //    ADL.ADL_Overdrive5_PowerControl_Set(ADLAdapterIndex, mPowerControlBackup);

        //    // OverDrive 5
        //    var levelsBuffer = Marshal.AllocCoTaskMem((int)(sODPerformanceLevelsBackup.iSize));
        //    Marshal.StructureToPtr(sODPerformanceLevelsBackup, levelsBuffer, false);
        //    ADL.ADL_Overdrive5_ODPerformanceLevels_Set(ADLAdapterIndex, levelsBuffer);

        //    // OverDrive Next (System Clocks)
        //    var ODNLevelsBuffer = Marshal.AllocCoTaskMem((int)(sODNSystemClocksBackup.iSize));
        //    Marshal.StructureToPtr(sODNSystemClocksBackup, ODNLevelsBuffer, false);
        //    ADL.ADL2_OverdriveN_SystemClocks_Set(ADL2Context, ADLAdapterIndex, ODNLevelsBuffer);

        //    // OverDrive Next (Memory Clocks)
        //    ODNLevelsBuffer = Marshal.AllocCoTaskMem((int)(sODNMemoryClocksBackup.iSize));
        //    Marshal.StructureToPtr(sODNMemoryClocksBackup, ODNLevelsBuffer, false);
        //    ADL.ADL2_OverdriveN_MemoryClocks_Set(ADL2Context, ADLAdapterIndex, ODNLevelsBuffer);
        //}


    }
}
