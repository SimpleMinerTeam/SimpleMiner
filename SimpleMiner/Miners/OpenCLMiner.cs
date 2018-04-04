using Cloo;
using GalaSoft.MvvmLight.Messaging;
using SimpleCPUMiner.Hardware;
using SimpleCPUMiner.Messages;
using System;
using System.Collections.Generic;
using System.IO;

namespace SimpleCPUMiner.Miners
{
    class OpenCLMiner : Miner
    {
        public OpenCLDevice OpenCLDevice { get; private set; }
        public ComputeCommandQueue Queue { get; private set; }
        public ComputeDevice ComputeDevice { get { return OpenCLDevice.ComputeDevice; } }

        protected OpenCLMiner(OpenCLDevice pDevice, String pAlgorithmName, String pFirstAlgorithmName = "", String pSecondAlgorithmName = "")
            : base(pDevice, pAlgorithmName, pFirstAlgorithmName, pSecondAlgorithmName)
        {
            OpenCLDevice = pDevice;
            Queue = new ComputeCommandQueue(Context, ComputeDevice, ComputeCommandQueueFlags.OutOfOrderExecution);
        }

        protected ComputeProgram BuildProgram(string programName, long localWorkSize, string optionsAMD, string optionsNVIDIA, string optionsOthers)
        {
            ComputeProgram program;
            string kernelKey = $"{ComputeDevice.Name}_{programName}_{localWorkSize}";
            string savedBinaryFilePath = $@"{Consts.ApplicationPath}Miners\Kernel\Bins\{kernelKey}.bin";
            string sourceFilePath = $@"{Consts.ApplicationPath}Miners\Kernel\{programName}.cl";            
            String buildOptions = (OpenCLDevice.ComputeDevice.Vendor.Equals(Consts.VendorAMD) ? optionsAMD : OpenCLDevice.ComputeDevice.Vendor.Equals(Consts.VendorNvidia) ? optionsNVIDIA : optionsOthers);

            try
            {
                if (File.Exists(savedBinaryFilePath))
                {
                    Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"Loading prebuilt kernel from {savedBinaryFilePath}" });
                    byte[] binary;

                    lock (Utils.Kernels)
                    {
                        if (!Utils.Kernels.TryGetValue(kernelKey, out binary))
                        {
                            binary = System.IO.File.ReadAllBytes(savedBinaryFilePath);
                            Utils.Kernels.Add(kernelKey, binary);
                        }
                    }

                    program = new ComputeProgram(Context, new List<byte[]>() { binary }, new List<ComputeDevice>() { ComputeDevice });
                }
                else if (File.Exists(sourceFilePath))
                {
                    Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"Building kernel from {sourceFilePath}" });
                    String source;

                    lock (Utils.Programs)
                    {
                        if (!Utils.Programs.TryGetValue(programName, out source))
                        {
                            source = System.IO.File.ReadAllText(sourceFilePath);
                            Utils.Programs.Add(programName, source);
                        }
                    }

                    program = new ComputeProgram(Context, source);
                }
                else
                {
                    Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"No suitable kernel found, can't start GPU mining." });
                    return null;
                }
            }
            catch(Exception ex)
            {
                Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"Exception during kernel load: {ex.ToString()}" });
                return null;
            }

            try
            {
                program.Build(OpenCLDevice.ComputeDeviceList, buildOptions, null, IntPtr.Zero);
                try
                {
                    if (program.Binaries.Count > 0 && program.Binaries[0].Length > 0)
                        File.WriteAllBytes(savedBinaryFilePath, program.Binaries[0]);
                }
                catch
                {
                    //hát ha nem sikerült hát nem sikerült az élet megy tovább
                }
            }
            catch (Exception)
            {
                Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = program.GetBuildLog(ComputeDevice), IsError = true });
                program.Dispose();
            }

            return program;
        }
    }
}
