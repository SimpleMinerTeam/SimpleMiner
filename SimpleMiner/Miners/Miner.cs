using Cloo;
using SimpleCPUMiner.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleCPUMiner.Miners
{
    public class Miner : IDisposable
    {
        private OpenCLDevice mDevice;
        private bool mStopped = false;
        private String mAlgorithmName = "";
        private String mFirstAlgorithmName = "";
        private String mSecondAlgorithmName = "";
        private Thread mMinerThread = null;
        private DateTime mLastAlive = DateTime.Now;

        public OpenCLDevice Device { get { return mDevice; } }
        public int DeviceIndex { get { return mDevice.ADLAdapterIndex; } }
        public bool Stopped { get { return mStopped; } }
        public double Speed { get; set; }
        public double SecondSpeed { get; set; }
        public String AlgorithmName { get { return mAlgorithmName; } }
        public String FirstAlgorithmName { get { return mFirstAlgorithmName; } }
        public String SecondAlgorithmName { get { return mSecondAlgorithmName; } }
        public ComputeContext Context { get { return mDevice.Context; } }

        protected Miner(OpenCLDevice aDevice, String aAlgorithmName, String aFirstAlgorithmName = "", String aSecondAlgorithmName = "")
        {
            mDevice = aDevice;
            mAlgorithmName = aAlgorithmName;
            mFirstAlgorithmName = (aFirstAlgorithmName == "") ? aAlgorithmName : aFirstAlgorithmName;
            mSecondAlgorithmName = aSecondAlgorithmName;
            Speed = 0;
            SecondSpeed = 0;
        }

        ~Miner()
        {
            Stop();
            Abort();
        }

        public void Start()
        {
            mStopped = false;

            MarkAsAlive();
            mMinerThread = new Thread(MinerThread);
            mMinerThread.IsBackground = true;
            mMinerThread.Start();
        }

        unsafe protected virtual void MinerThread() { }

        public void Stop()
        {
            mStopped = true;
            Speed = 0;
            SecondSpeed = 0;
        }

        public void Abort()
        {
            if (mMinerThread != null)
            {
                try
                {
                    mMinerThread.Abort();
                }
                catch (Exception) { }
                mMinerThread = null;
            }
        }

        protected void MarkAsAlive()
        {
            mLastAlive = DateTime.Now;
        }

        public void Dispose()
        {

            if (!Stopped)
            {
                Stop();
            }

            Abort();
        }

        public bool Alive
        {
            get
            {
                if (mMinerThread != null && (DateTime.Now - mLastAlive).TotalSeconds >= 5)
                    Speed = 0;
                return !(mMinerThread != null && (DateTime.Now - mLastAlive).TotalSeconds >= 60);
            }
        }
    }
}
