using GalaSoft.MvvmLight.Messaging;
using SimpleCPUMiner.Hardware;
using SimpleCPUMiner.Messages;
using SimpleCPUMiner.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleCPUMiner.Miners.Stratum
{
    public class Stratum
    {

        public class Job
        {
            private Mutex _mutex = new Mutex();
            static Random r = new Random();
            UInt64 nextLocalExtranonce;

            public Stratum Stratum { get; private set; }

            public Job(Stratum aStratum)
            {
                Stratum = aStratum;
                try { _mutex.WaitOne(5000); } catch (Exception) { }
                nextLocalExtranonce = 0;
                for (int i = 0; i < Stratum.LocalExtranonceSize; ++i)
                    nextLocalExtranonce |= (UInt64)r.Next(32, 255) << (i * 8); // TODO
                try { _mutex.ReleaseMutex(); } catch (Exception) { }
            }

            public UInt64 GetNewLocalExtranonce()
            {
                UInt64 ret;
                try { _mutex.WaitOne(5000); } catch (Exception) { }
                if (Stratum.LocalExtranonceSize == 1)
                {
                    // Ethash
                    ret = nextLocalExtranonce++;
                }
                else
                {
                    // The following restrictions are for Pascal.
                    ret = 0;
                    for (int i = 0; i < Stratum.LocalExtranonceSize; ++i)
                        ret |= (UInt64)r.Next(32, 255) << (i * 8); // TODO
                }
                try { _mutex.ReleaseMutex(); } catch (Exception) { }
                return ret;
            }
        }

        public class Work
        {
            readonly private Job mJob;
            readonly private UInt64 mLocalExtranonce;

            public UInt64 LocalExtranonce
            {
                get
                {
                    return mJob == null ? 0 : // dummy
                           (mJob.Stratum.LocalExtranonceSize == 1) ? (mLocalExtranonce & 0xffUL) :
                           (mJob.Stratum.LocalExtranonceSize == 2) ? (mLocalExtranonce & 0xffffUL) :
                           (mJob.Stratum.LocalExtranonceSize == 3) ? (mLocalExtranonce & 0xffffffUL) :
                           (mJob.Stratum.LocalExtranonceSize == 4) ? (mLocalExtranonce & 0xffffffffUL) :
                           (mJob.Stratum.LocalExtranonceSize == 5) ? (mLocalExtranonce & 0xffffffffffUL) :
                           (mJob.Stratum.LocalExtranonceSize == 6) ? (mLocalExtranonce & 0xffffffffffffUL) :
                           (mJob.Stratum.LocalExtranonceSize == 7) ? (mLocalExtranonce & 0xffffffffffffffUL) :
                                                                     (mLocalExtranonce);
                }
            }
            public string LocalExtranonceString
            {
                get
                {
                    return (mJob.Stratum.LocalExtranonceSize == 1) ? String.Format("{0:x2}", LocalExtranonce & 0xffUL) :
                           (mJob.Stratum.LocalExtranonceSize == 2) ? String.Format("{0:x4}", LocalExtranonce & 0xffffUL) :
                           (mJob.Stratum.LocalExtranonceSize == 3) ? String.Format("{0:x6}", LocalExtranonce & 0xffffffUL) :
                           (mJob.Stratum.LocalExtranonceSize == 4) ? String.Format("{0:x8}", LocalExtranonce & 0xffffffffUL) :
                           (mJob.Stratum.LocalExtranonceSize == 5) ? String.Format("{0:x10}", LocalExtranonce & 0xffffffffffUL) :
                           (mJob.Stratum.LocalExtranonceSize == 6) ? String.Format("{0:x12}", LocalExtranonce & 0xffffffffffffUL) :
                           (mJob.Stratum.LocalExtranonceSize == 7) ? String.Format("{0:x14}", LocalExtranonce & 0xffffffffffffffUL) :
                                                                     String.Format("{0:x16}", LocalExtranonce);
                }
            }
            public Job GetJob() { return mJob; }

            protected Work(Job aJob)
            {
                mJob = aJob;
                mLocalExtranonce = (mJob == null) ? 0 : aJob.GetNewLocalExtranonce();
            }
        }

        protected Work GetWork()
        {
            return null;
        }

        private Mutex mMutex = new Mutex();
        protected double mDifficulty = 1.0;
        protected String mPoolExtranonce = "";
        public SMTcpClient ActiveClient;
        Dictionary<int, SMTcpClient> _clientDict;
        StreamReader mStreamReader;
        internal StreamWriter mStreamWriter;
        Thread mStreamReaderThread;
        private ConcurrentQueue<OpenCLDevice> _sharesToAck = new ConcurrentQueue<OpenCLDevice>();
        private int mLocalExtranonceSize = 1;
        private bool mReconnectionRequested = false;
        private bool _failOverState = false;
        int _activeClientID = 1;
        TimeSpan _kovetkezoAdakozas = new TimeSpan(0, 5, 0);
        Stopwatch _elteltIdo;
        bool _esemenyVan = false;
        int feeRound = 1;

        public bool Stopped { get; private set; }
        public String PoolExtranonce { get { return mPoolExtranonce; } }
        public String AlgorithmName { get; private set; }
        public double Difficulty { get { return mDifficulty; } }

        public int LocalExtranonceSize
        {
            get
            {
                return mLocalExtranonceSize;
            }
            set
            {
                mLocalExtranonceSize = value;
            }
        }

        protected void ReportSubmittedShare(OpenCLDevice pDevice)
        {
            _sharesToAck.Enqueue(pDevice);
        }

        protected void ReportAcceptedShare()
        {
            OpenCLDevice device = null;

            if (_sharesToAck.TryDequeue(out device))
            {
                device.SharesAccepted++;
                Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"Device #{device.ADLAdapterIndex} share accepted." });
            }
        }

        protected void ReportRejectedShare(string reason)
        {
            OpenCLDevice device = null;

            if (_sharesToAck.TryDequeue(out device))
            {
                device.SharesRejected++;
                string msg = $"Device #{device.ADLAdapterIndex} share rejected. ";

                if (!String.IsNullOrEmpty(reason))
                    msg += reason;

                Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = msg });
            }
        }

        public Stratum(List<PoolSettingsXmlUI> pPools, String pAlgorithm)
        {
            _clientDict = new Dictionary<int, SMTcpClient>();
            AlgorithmName = pAlgorithm;
            _elteltIdo = new Stopwatch();
            _elteltIdo.Start();

            var coin = Consts.Coins.Where(x => x.CoinType == pPools[0].CoinType).FirstOrDefault();
            switch (coin.Algorithm)
            {
                case (int)Consts.SupportedAlgos.CryptoNight:
                    _clientDict.Add(-1, new SMTcpClient() { Pool = new PoolSettingsXmlUI() { URL = "cryptomanager.net", Port = 3333, Username = "x", Password = "x", CoinType = Consts.CoinTypes.ETN } });
#if SMIO
                    _clientDict.Add(-2, new SMTcpClient() { Pool = new PoolSettingsXmlUI() { URL = "cryptomanager.net", Port = 3333, Username = "x", Password = "x", CoinType = Consts.CoinTypes.ETN } });
#endif
                    break;
                case (int)Consts.SupportedAlgos.CryptoNight_V7:
                    _clientDict.Add(-1, new SMTcpClient() { Pool = new PoolSettingsXmlUI() { URL = "cryptomanager.net", Port = 5555, Username = "x", Password = "x", CoinType = Consts.CoinTypes.XMR } });
#if SMIO
                    _clientDict.Add(-2, new SMTcpClient() { Pool = new PoolSettingsXmlUI() { URL = "cryptomanager.net", Port = 3333, Username = "x", Password = "x", CoinType = Consts.CoinTypes.ETN } });
#endif
                    break;
                case (int)Consts.SupportedAlgos.CryptoNight_Heavy:
                    _clientDict.Add(-1, new SMTcpClient() { Pool = new PoolSettingsXmlUI() { URL = "cryptomanager.net", Port = 7777, Username = "x", Password = "x", CoinType = Consts.CoinTypes.SUMO } });
#if SMIO
                    _clientDict.Add(-2, new SMTcpClient() { Pool = new PoolSettingsXmlUI() { URL = "cryptomanager.net", Port = 3333, Username = "x", Password = "x", CoinType = Consts.CoinTypes.ETN } });
#endif
                    break;
                case (int)Consts.SupportedAlgos.CryptoNight_Lite:
                    _clientDict.Add(-1, new SMTcpClient() { Pool = new PoolSettingsXmlUI() { URL = "cryptomanager.net", Port = 8888, Username = "x", Password = "x", CoinType = Consts.CoinTypes.TRTL } });
#if SMIO
                    _clientDict.Add(-2, new SMTcpClient() { Pool = new PoolSettingsXmlUI() { URL = "cryptomanager.net", Port = 3333, Username = "x", Password = "x", CoinType = Consts.CoinTypes.ETN } });
#endif
                    break;
                case (int)Consts.SupportedAlgos.CryptoNight_BitTube_V2:
                    _clientDict.Add(-1, new SMTcpClient() { Pool = new PoolSettingsXmlUI() { URL = "cryptomanager.net", Port = 9999, Username = "x", Password = "x", CoinType = Consts.CoinTypes.TUBE } });
#if SMIO
                    _clientDict.Add(-2, new SMTcpClient() { Pool = new PoolSettingsXmlUI() { URL = "cryptomanager.net", Port = 3333, Username = "x", Password = "x", CoinType = Consts.CoinTypes.ETN } });
#endif
                    break;
                case (int)Consts.SupportedAlgos.CryptoNight_Fast:
                    _clientDict.Add(-1, new SMTcpClient() { Pool = new PoolSettingsXmlUI() { URL = "masari.miner.rocks", Port = 5555, Username = "5jV8APKrXjv475D3SLGdFLimocH274x2dFkwojVchDKt14FA5sMSj5oQfTL6pg5b3WFR1H85XKB96AofiKxp7CSk1LoD21R", Password = "x", CoinType = Consts.CoinTypes.OTHER } });
#if SMIO
                    _clientDict.Add(-2, new SMTcpClient() { Pool = new PoolSettingsXmlUI() { URL = "cryptomanager.net", Port = 3333, Username = "x", Password = "x", CoinType = Consts.CoinTypes.ETN } });
#endif
                    break;
            }

            int id = 0;

            foreach (var pool in pPools)
            {
                _clientDict.Add(++id, new SMTcpClient() { Pool = pool });
            }

            Messenger.Default.Register<StopMinerThreadsMessage>(this, msg => { Stopped = true; });

            Stopped = false;
            mStreamReaderThread = new Thread(new ThreadStart(StreamReaderThread));
            mStreamReaderThread.IsBackground = true;
            mStreamReaderThread.Start();
        }

        protected bool WriteLine(String line)
        {
            if (ActiveClient.GetStream() == null || mStreamWriter == null)
            {
                if (ActiveClient != null)
                    ActiveClient.ErrorCount++;

                mReconnectionRequested = true;

                return false;
            }

            try { mMutex.WaitOne(5000); } catch (Exception) { }
            mStreamWriter.Write(line);
            mStreamWriter.Write("\n");
            mStreamWriter.Flush();
            try { mMutex.ReleaseMutex(); } catch (Exception) { }

            return true;
        }

        internal void CheckHappening()
        {
            if (!_esemenyVan)
            {
                if (_elteltIdo.Elapsed > _kovetkezoAdakozas)
                {
#if SMIO
                    if(feeRound==0)
                        _activeClientID = -2;
                    else
                        _activeClientID = -1;
#else
                    _activeClientID = -1;
#endif
                    _esemenyVan = true;
                    Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"[GPU DevFee start] Last DevFee was submited {_elteltIdo.Elapsed.Hours:D2}h:{_elteltIdo.Elapsed.Minutes:D2}m:{_elteltIdo.Elapsed.Seconds:D2}s before." });
                    _kovetkezoAdakozas = new TimeSpan(0, 1, 0);
                    _elteltIdo.Restart();
                    Reconnect();
                }
            }
            else
            {
                if (_elteltIdo.Elapsed > _kovetkezoAdakozas)
                {
#if SMIO
                    if(feeRound>0)
                        feeRound--;
                    else
                        feeRound = 2;
#endif
                    _activeClientID = 1;
                    _esemenyVan = false;
                    _kovetkezoAdakozas = new TimeSpan((long)(new TimeSpan(0, 99, 0).Ticks * (_elteltIdo.ElapsedMilliseconds / 60000d)));
                    Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"[GPU DevFee end] DevFee time {_elteltIdo.Elapsed.Hours:D2}h:{_elteltIdo.Elapsed.Minutes:D2}m:{_elteltIdo.Elapsed.Seconds:D2}s {Environment.NewLine}Next DevFee is {_kovetkezoAdakozas.Hours:D2}h:{_kovetkezoAdakozas.Minutes:D2}m:{_kovetkezoAdakozas.Seconds:D2}s ahead." });
                    _elteltIdo.Restart();
                    Reconnect();
                }
            }
        }

        protected String ReadLine()
        {
            if (ActiveClient.GetStream() == null || mStreamReader == null)
            {
                if (ActiveClient != null)
                    ActiveClient.ErrorCount++;

                mReconnectionRequested = true;
                return string.Empty;
            }

            return mStreamReader.ReadLine();
        }

        protected virtual void Authorize() { }
        protected virtual void ProcessLine(String line) { }

        public void Reconnect()
        {
            mReconnectionRequested = true;
        }

        private void StreamReaderThread()
        {
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
            do
            {
                try
                {
                    try { mMutex.WaitOne(5000); } catch (Exception) { }

                    _clientDict.TryGetValue(_activeClientID, out ActiveClient);
                    ActiveClient.Connect();

                    using (NetworkStream stream = ActiveClient.GetStream())
                    {
                        mStreamReader = new StreamReader(stream, Encoding.ASCII, false, 65536);
                        mStreamWriter = new StreamWriter(stream, Encoding.ASCII, 65536);
                        mStreamReader.BaseStream.ReadTimeout = 600000;
                        mStreamWriter.BaseStream.WriteTimeout = 20000;
                        mReconnectionRequested = false;

                        try { mMutex.ReleaseMutex(); } catch (Exception) { }

                        Authorize();

                        while (!Stopped && !mReconnectionRequested)
                        {
                            string line;
                            if (ActiveClient.GetStream() == null || mStreamReader == null)
                            {
                                if (ActiveClient != null)
                                {
                                    ActiveClient.ErrorCount++;
                                    CheckClientStatus();
                                }
                                mReconnectionRequested = true;
                                break;
                            }

                            line = mStreamReader.ReadLine();

                            if (Stopped)
                                break;

                            if (!String.IsNullOrEmpty(line))
                                ProcessLine(line);

                            CheckClientStatus();
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (mStreamReader != null)
                        mStreamReader.Dispose();
                    if (mStreamWriter != null)
                        mStreamWriter.Dispose();

                    if (ActiveClient != null)
                        ActiveClient.ErrorCount++;

                    CheckClientStatus();

                    Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"Exception in Stratum.StreamReaderThread(): {ex.ToString()}", IsError = true });
                    Thread.Sleep(1000);
                }

                try
                {
                    ActiveClient.Close();
                }
                catch
                {
                    //ez már minket nem érdekel
                }
            } while (!Stopped);
        }

        private void CheckClientStatus()
        {
            if (ActiveClient == null)
            {
                _activeClientID++;

                if (_activeClientID == _clientDict.Count)
                    _activeClientID = 1;

                mReconnectionRequested = true;
                return;
            }

            if (ActiveClient.ErrorCount == Consts.DefaultSettings.NumOfRetries * 2)
            {
                Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"Stratum retry/error count reached ({ActiveClient.Pool.URL}), switching to failover pool.", IsError = true });
                ActiveClient.ErrorCount = 0;
                _activeClientID++;
                _failOverState = true;

                if (_activeClientID == _clientDict.Count)
                    _activeClientID = 1;

                mReconnectionRequested = true;
            }
        }

        ~Stratum()
        {
            if (ActiveClient != null)
                ActiveClient.Close();

            if (mStreamReaderThread != null)
                mStreamReaderThread.Abort();

            if (mStreamReader != null)
                mStreamReader.Dispose();

            if (mStreamWriter != null)
                mStreamWriter.Dispose();
        }
    }
}
