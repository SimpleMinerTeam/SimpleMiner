using Cloo;
using GalaSoft.MvvmLight.Messaging;
using SimpleCPUMiner.Hardware;
using SimpleCPUMiner.Messages;
using SimpleCPUMiner.Miners.Stratum;
using SimpleCPUMiner.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace SimpleCPUMiner.Miners
{
    class OpenCLCryptoNightMiner : OpenCLMiner, IDisposable
    {
        private static readonly int outputSize = 256 + 255 * 8;
        static Mutex mProgramArrayMutex = new Mutex();

        private CryptoNightStratum _stratum;

        long[] globalWorkSizeA = new long[] { 0, 8 };
        long[] globalWorkSizeB = new long[] { 0 };
        long[] localWorkSizeA = new long[] { 0, 8 };
        long[] localWorkSizeB = new long[] { 0 };
        long[] globalWorkOffsetA = new long[] { 0, 1 };
        long[] globalWorkOffsetB = new long[] { 0 };

        Int32[] terminate = new Int32[1];
        UInt32[] output = new UInt32[outputSize];
        byte[] input = new byte[76];

        ComputeKernel searchKernel0 = null;
        ComputeKernel searchKernel1 = null;
        ComputeKernel searchKernel2 = null;
        ComputeKernel searchKernel3 = null;

        private ComputeBuffer<byte> statesBuffer = null;
        private ComputeBuffer<byte> inputBuffer = null;
        private ComputeBuffer<UInt32> outputBuffer = null;
        private ComputeBuffer<Int32> terminateBuffer = null;
        private ComputeBuffer<byte> scratchpadsBuffer = null;

        Coin _coin = null;

        public bool NiceHashMode { get; private set; }

        public OpenCLCryptoNightMiner(OpenCLDevice pDevice)
            : base(pDevice, "CryptoNight")
        {
            inputBuffer = new ComputeBuffer<byte>(Context, ComputeMemoryFlags.ReadOnly, input.Length);
            outputBuffer = new ComputeBuffer<UInt32>(Context, ComputeMemoryFlags.ReadWrite, outputSize);
            terminateBuffer = new ComputeBuffer<Int32>(Context, ComputeMemoryFlags.ReadWrite, 1);
        }

        public void Start(CryptoNightStratum pStratum, int pRawIntensity, int pLocalWorkSize, bool pNicehashMode = true)
        {
            //foreach (ProcessThread pt in Process.GetCurrentProcess().Threads)
            //{
            //    int utid = Utils.GetCurrentThreadId();
            //    if (utid == pt.Id)
            //    {
            //        pt.ProcessorAffinity = (IntPtr)Utils.PriorityThread;
            //    }
            //}

            var prevGlobalWorkSize = globalWorkSizeA[0];

            _stratum = pStratum;
            _coin = Consts.Coins.Where(x => x.CoinType == _stratum.ActiveClient.Pool.CoinType).FirstOrDefault();

            if(_coin.Algorithm == Consts.Algorithm.CryptoNightHeavy)
                globalWorkSizeA[0] = globalWorkSizeB[0] = Utils.NearestEven(pRawIntensity * pLocalWorkSize/2); 
            else
                globalWorkSizeA[0] = globalWorkSizeB[0] = pRawIntensity * pLocalWorkSize;

            localWorkSizeA[0] = localWorkSizeB[0] = pLocalWorkSize;
            NiceHashMode = pNicehashMode;

            if (statesBuffer == null)
                statesBuffer = new ComputeBuffer<byte>(Context, ComputeMemoryFlags.ReadWrite, 200 * globalWorkSizeA[0]);

            base.Start();
        }

        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        [System.Security.SecurityCritical]
        override unsafe protected void MinerThread()
        {
            Messenger.Default.Register<StopMinerThreadsMessage>(this, msg => {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    Messenger.Default.Unregister(this);
                    this.Dispose();
                });
            });
            Random r = new Random();
            ComputeDevice computeDevice = OpenCLDevice.ComputeDevice;
            ComputeProgram program;
            var GCN1 = computeDevice.Name.Equals("Capeverde") || computeDevice.Name.Equals("Hainan") || computeDevice.Name.Equals("Oland") || computeDevice.Name.Equals("Pitcairn") || computeDevice.Name.Equals("Tahiti");

            string programName = String.Empty;

            if (_coin == null)
            {
                Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"Invalid coin! Device #{DeviceIndex} {_stratum.ActiveClient.Pool.CoinType}" });
                Dispose();
                return;
            }

            switch(_coin.Algorithm)
            {
                case Consts.Algorithm.CryptoNight:
                case Consts.Algorithm.CryptoNightV7:
                    programName = "cryptonight";
                    scratchpadsBuffer = new ComputeBuffer<byte>(Context, ComputeMemoryFlags.ReadWrite, ((long)1 << 21) * globalWorkSizeA[0]);
                    break;
                case Consts.Algorithm.CryptoNightHeavy:
                    programName = "cryptonightHeavy";
                    scratchpadsBuffer = new ComputeBuffer<byte>(Context, ComputeMemoryFlags.ReadWrite, ((long)1 << 22) * globalWorkSizeA[0]);
                    break;
                case Consts.Algorithm.CryptoNightLite:
                case Consts.Algorithm.CryptoNightLiteV1:
                    programName = "cryptonightLite";
                    scratchpadsBuffer = new ComputeBuffer<byte>(Context, ComputeMemoryFlags.ReadWrite, ((long)1 << 20) * globalWorkSizeA[0]);
                    break;
            }

            string algorithmBuildParameters = $@" -I Miners\Kernel -DWORKSIZE={localWorkSizeA[0]}  -DSTRIDED_INDEX=1 -DMEMORY_CHUNK_SIZE=2";
            program = BuildProgram(programName, localWorkSizeA[0], $"{algorithmBuildParameters} -O5" + (GCN1 ? " -legacy" : ""), algorithmBuildParameters, algorithmBuildParameters);

            if (program == null)
            {
                Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"Faild to build/load opencl kernel! Device #{DeviceIndex} {AlgorithmName}" });
                Dispose();
                return;
            }

            searchKernel0 = program.CreateKernel("search");
            searchKernel1 = program.CreateKernel("search1");
            searchKernel2 = program.CreateKernel("search2");
            searchKernel3 = program.CreateKernel("search3");


            fixed (long* globalWorkOffsetAPtr = globalWorkOffsetA)
            fixed (long* globalWorkOffsetBPtr = globalWorkOffsetB)
            fixed (long* globalWorkSizeAPtr = globalWorkSizeA)
            fixed (long* globalWorkSizeBPtr = globalWorkSizeB)
            fixed (long* localWorkSizeAPtr = localWorkSizeA)
            fixed (long* localWorkSizeBPtr = localWorkSizeB)
            fixed (Int32* terminatePtr = terminate)
            fixed (byte* inputPtr = input)
            fixed (UInt32* outputPtr = output)
            {
                while (!Stopped)
                {
                    try
                    {
                        searchKernel0.SetMemoryArgument(0, inputBuffer);
                        searchKernel0.SetMemoryArgument(1, scratchpadsBuffer);
                        searchKernel0.SetMemoryArgument(2, statesBuffer);

                        searchKernel1.SetMemoryArgument(0, scratchpadsBuffer);
                        searchKernel1.SetMemoryArgument(1, statesBuffer);
                        searchKernel1.SetMemoryArgument(2, terminateBuffer);
                        searchKernel1.SetMemoryArgument(4, inputBuffer);

                        searchKernel2.SetMemoryArgument(0, scratchpadsBuffer);
                        searchKernel2.SetMemoryArgument(1, statesBuffer);

                        searchKernel3.SetMemoryArgument(0, statesBuffer);
                        searchKernel3.SetMemoryArgument(1, outputBuffer);

                        // Wait for the first job to arrive.
                        int elapsedTime = 0;
                        while ((_stratum == null || _stratum.GetJob() == null) && elapsedTime < 60000 && !Stopped)
                        {
                            Thread.Sleep(100);
                            elapsedTime += 100;
                        }
                        if (_stratum == null || _stratum.GetJob() == null)
                            throw new TimeoutException("Stratum server failed to send a new job.");

                        System.Diagnostics.Stopwatch consoleUpdateStopwatch = new System.Diagnostics.Stopwatch();
                        CryptoNightStratum.Work work;

                        while (!Stopped && (work = _stratum.GetWork()) != null)
                        {
                            var job = work.GetJob();                            
                            Array.Copy(job.Blob, input, 76);
                            
                            byte localExtranonce = (byte)work.LocalExtranonce;
                            UInt32 startNonce;
                            if (NiceHashMode)
                            {
                                startNonce = ((UInt32)input[42] << (8 * 3)) | ((UInt32)localExtranonce << (8 * 2)) | (UInt32)(r.Next(0, int.MaxValue) & (0x0000ffffu));
                            }
                            else
                            {
                                startNonce = ((UInt32)localExtranonce << (8 * 3)) | (UInt32)(r.Next(0, int.MaxValue) & (0x00ffffffu));
                            }

                            if ((_stratum.ActiveClient.Pool.Algorithm!=null && (_stratum.ActiveClient.Pool.Algorithm == Consts.Algorithm.CryptoNight || _stratum.ActiveClient.Pool.Algorithm == Consts.Algorithm.CryptoNightLite))
                                ||(_stratum.ActiveClient.Pool.Algorithm == null && (_coin.Algorithm == Consts.Algorithm.CryptoNight
                                || _coin.Algorithm == Consts.Algorithm.CryptoNightLite)))
                            {
                                searchKernel0.SetValueArgument<uint>(3, 0);
                                searchKernel1.SetValueArgument<uint>(3, 0);
                                searchKernel2.SetValueArgument<uint>(2, 0);
                            }
                            else if((_stratum.ActiveClient.Pool.Algorithm != null && _stratum.ActiveClient.Pool.Algorithm == Consts.Algorithm.CryptoNightLiteV1) || (_stratum.ActiveClient.Pool.Algorithm == null && _coin.Algorithm == Consts.Algorithm.CryptoNightLiteV1))
                            {
                                searchKernel0.SetValueArgument<uint>(3, 7);
                                searchKernel1.SetValueArgument<uint>(3, 7);
                                searchKernel2.SetValueArgument<uint>(2, 7);
                            }
                            else
                            {
                                searchKernel0.SetValueArgument<uint>(3, (uint)job.Variant);
                                searchKernel1.SetValueArgument<uint>(3, (uint)job.Variant);
                                searchKernel2.SetValueArgument<uint>(2, (uint)job.Variant);
                            }

                            searchKernel3.SetValueArgument<UInt32>(2, job.Target);

                            Queue.Write<byte>(inputBuffer, true, 0, 76, (IntPtr)inputPtr, null);

                            consoleUpdateStopwatch.Start();

                            while (!Stopped && _stratum.GetJob().Equals(job))
                            {
                                globalWorkOffsetA[0] = globalWorkOffsetB[0] = startNonce;

                                // Get a new local extranonce if necessary.
                                if (NiceHashMode)
                                {
                                    if ((startNonce & 0xffff) + (UInt32)globalWorkSizeA[0] >= 0x10000)
                                        break;
                                }
                                else
                                {
                                    if ((startNonce & 0xffffff) + (UInt32)globalWorkSizeA[0] >= 0x1000000)
                                        break;
                                }

                                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                                sw.Start();
                                output[255] = 0; // output[255] is used as an atomic counter.
                                Queue.Write<UInt32>(outputBuffer, true, 0, outputSize, (IntPtr)outputPtr, null);
                                terminate[0] = 0;
                                Queue.Write<Int32>(terminateBuffer, true, 0, 1, (IntPtr)terminatePtr, null);
                                Queue.Execute(searchKernel0, globalWorkOffsetA, globalWorkSizeA, localWorkSizeA, null);
                                Queue.Finish();
                                if (Stopped)
                                    break;
                                Queue.Execute(searchKernel1, globalWorkOffsetB, globalWorkSizeB, localWorkSizeB, null);
                                Queue.Finish();
                                if (Stopped)
                                    break;
                                Queue.Execute(searchKernel2, globalWorkOffsetA, globalWorkSizeA, localWorkSizeA, null);
                                Queue.Finish();
                                if (Stopped)
                                    break;
                                Queue.Execute(searchKernel3, globalWorkOffsetB, globalWorkSizeB, localWorkSizeB, null);
                                Queue.Finish();
                                if (Stopped)
                                    break;

                                Queue.Read<UInt32>(outputBuffer, true, 0, outputSize, (IntPtr)outputPtr, null);
                                if (_stratum.GetJob().Equals(job))
                                {
                                    for (int i = 0; i < output[255]; ++i)
                                    {
                                        String result = "";
                                        for (int j = 0; j < 8; ++j)
                                        {
                                            UInt32 word = output[256 + i * 8 + j];
                                            result += String.Format("{0:x2}{1:x2}{2:x2}{3:x2}", ((word >> 0) & 0xff), ((word >> 8) & 0xff), ((word >> 16) & 0xff), ((word >> 24) & 0xff));
                                        }
                                        _stratum.Submit(Device, job, output[i], result);
                                    }
                                }
                                startNonce += (UInt32)globalWorkSizeA[0];

                                sw.Stop();
                                Speed = globalWorkSizeA[0] / sw.Elapsed.TotalSeconds;
                                if (consoleUpdateStopwatch.ElapsedMilliseconds >= 10 * 1000)
                                {
                                    //Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"Device #{DeviceIndex} (CryptoNight): {Speed:N2} h/s" });
                                    consoleUpdateStopwatch.Restart();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"Compute error! Device #{DeviceIndex} (CryptoNight)" });
                    }

                    Speed = 0;

                    if (!Stopped)
                    {
                        Thread.Sleep(5000);
                    }
                }
            }
        }

        public void Dispose()
        {
            try
            {
                Stop();

                try { mProgramArrayMutex.WaitOne(5000); } catch (Exception) { }

                Queue.Dispose();

                statesBuffer.Dispose();
                inputBuffer.Dispose();
                outputBuffer.Dispose();
                terminateBuffer.Dispose();
                scratchpadsBuffer.Dispose();

                searchKernel0.Dispose();
                searchKernel1.Dispose();
                searchKernel2.Dispose();
                searchKernel3.Dispose();

                try { mProgramArrayMutex.ReleaseMutex(); } catch (Exception) { }

                Abort();
            }
            catch
            {

            }
        }
    }
}
