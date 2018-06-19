using Cloo;
using GalaSoft.MvvmLight.Messaging;
using SimpleCPUMiner.Hardware;
using SimpleCPUMiner.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SimpleCPUMiner.Miners
{
    class OpenCLMiner : Miner
    {
        public OpenCLDevice OpenCLDevice { get; private set; }
        public ComputeCommandQueue Queue { get; private set; }
        public ComputeDevice ComputeDevice { get { return OpenCLDevice.ComputeDevice; } }
        private static Object _lock = new Object();
        private static Object _lock2 = new Object();
        public bool NvidiaEszkoz { get; private set; }
        
        protected OpenCLMiner(OpenCLDevice pDevice, String pAlgorithmName, String pFirstAlgorithmName = "", String pSecondAlgorithmName = "")
            : base(pDevice, pAlgorithmName, pFirstAlgorithmName, pSecondAlgorithmName)
        {
            OpenCLDevice = pDevice;
            NvidiaEszkoz = pDevice.ComputeDevice.Vendor.Equals(Consts.VendorNvidia);
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

                    lock (_lock)
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

                    lock (_lock2)
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
                    Directory.CreateDirectory($@"{Consts.ApplicationPath}Miners\Kernel\Bins");
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
                return null;
            }

            return program;
        }
    }

    public class CryptoComputeEventList : ICollection<ComputeEventBase>
    {
        private List<ComputeEventBase> list = new List<ComputeEventBase>();

        public int Count => list.Count;
        public bool IsReadOnly => false;


        public void Add(ComputeEventBase item)
        {
            list.Add(item);
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(ComputeEventBase item)
        {
            return list.Contains(item);
        }

        public void CopyTo(ComputeEventBase[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ComputeEventBase> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public bool Remove(ComputeEventBase item)
        {
            return list.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)list).GetEnumerator();
        }

        public void Wait()
        {
            List<SMEventBase> SMEBList = new List<SMEventBase>();
            List<WaitHandle> waitHandles = new List<WaitHandle>();

            foreach (var item in list)
            {
                var SMEB = new SMEventBase(item);
                waitHandles.Add(SMEB.Mres.WaitHandle);
                SMEBList.Add(SMEB);
            }

            WaitHandle.WaitAll(waitHandles.ToArray());

            SMEBList.ForEach(x => x.Dispose());
        }

        public class SMEventBase
        {
            public ComputeEventBase Event { get; set; }
            public ManualResetEventSlim Mres = new ManualResetEventSlim();

            public SMEventBase(ComputeEventBase @event)
            {
                Event = @event;

                Event.Completed += Event_Completed;
                Event.Aborted += Event_Aborted;
            }

            private void Event_Aborted(object sender, ComputeCommandStatusArgs args)
            {
                Mres.Set();
            }

            private void Event_Completed(object sender, ComputeCommandStatusArgs args)
            {
                Mres.Set();
            }

            internal void Dispose()
            {
                Event.Dispose();
                Mres.Dispose();
            }
        }
    }
}
