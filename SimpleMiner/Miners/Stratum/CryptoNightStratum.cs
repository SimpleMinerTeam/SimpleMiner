using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleCPUMiner.Hardware;
using SimpleCPUMiner.Messages;
using SimpleCPUMiner.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleCPUMiner.Miners.Stratum
{

    public class CryptoNightStratum : Stratum
    {
        public new class Work : Stratum.Work
        {
            readonly private Job mJob;

            public new Job GetJob() { return mJob; }

            public Work(Job pJob)
                : base(pJob)
            {
                mJob = pJob;
            }
        }

        public new class Job : Stratum.Job
        {
            private String _strBlob;

            public String ID { get; }
            public byte[] Blob { get; private set; }
            public uint Target { get; private set; }
            public int Variant { get; private set; }
            public ulong Difficulty { get; private set; }

            public Job(Stratum pStratum, string pID, string pBlob, string pTarget, string pVariant)
                : base(pStratum)
            {
#if TEST
                //ETN
                _strBlob = "0606e7e2c1d80568587b00009621506f8b1b4cb0d18ad825d24c51cc44340550a86aab031f313f00000000d137ad05594fc9770631e5d6e08a1c60ba4bd12468f3543323ae456e2916e2dccf01";
                SetBlob(_strBlob);
                SetTarget("8b4f0100");
                SetVariant("1");
                ID = "272014441993087";
#else
                ID = pID;
                _strBlob = pBlob;
                SetBlob(pBlob);
                SetTarget(pTarget);
                SetVariant(pVariant);
#endif
            }

            private bool SetBlob(String blob)
            {
                if (!String.IsNullOrWhiteSpace(blob))
                {
                    Blob = Utils.StringToByteArray(blob);
                    return true;
                }

                return false;
            }

            private bool SetTarget(String target)
            {
                if (!string.IsNullOrWhiteSpace(target))
                {
                    if (target.Length == 8)
                    {
                        Target = BitConverter.ToUInt32(Utils.StringToByteArray(target), 0);
                        Difficulty = 0xFFFFFFFFFFFFFFFF / Target;
                        return true;
                    }
                }

                return false;
            }

            private bool SetVariant(String variant)
            {
                Variant = -1;

                if (Variant == -1)
                {
                    Variant = Blob[0];
                }

                return true;
            }

            public bool Equals(Job right)
            {
                return ID == right.ID && _strBlob == right._strBlob && Target == right.Target;
            }
        }

        String mUserID;
        Job mJob;
        private Mutex mMutex = new Mutex();

        public Job GetJob()
        {
            return mJob;
        }

        protected override void ProcessLine(String line)
        {
            Dictionary<String, Object> response = JsonConvert.DeserializeObject<Dictionary<string, Object>>(line);
            if (response.ContainsKey("method") && response.ContainsKey("params"))
            {
                string method = (string)response["method"];
                JContainer parameters = (JContainer)response["params"];
                if (method.Equals("job"))
                {
                    try { mMutex.WaitOne(5000); } catch (Exception) { }
                    mJob = new Job(this, (string)parameters["job_id"], (string)parameters["blob"], (string)parameters["target"], (string)parameters["variant"]);
                    try { mMutex.ReleaseMutex(); } catch (Exception) { }
                }
                else
                {
                    Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"Unknown stratum method: {line}" });
                }
            }
            else if (response.ContainsKey("id") && response.ContainsKey("error"))
            {
                var ID = response["id"];
                var error = response["error"];

                if (error == null)
                {
                    ReportAcceptedShare();
                }
                else if (error != null)
                {
                    ReportRejectedShare((String)(((JContainer)response["error"])["message"]));
                }
            }
            else
            {
                Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = "Unknown JSON message: " + line });
            }
        }

        override protected void Authorize()
        {
            var line = Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, Object> {
                { "method", "login" },
                { "params", new Dictionary<string, string> {
                    { "login", ActiveClient.Pool.Username },
                    { "pass", String.IsNullOrEmpty(ActiveClient.Pool.Password) ? "x" : ActiveClient.Pool.Password },
                    { "agent", Consts.ApplicationName + "/" + Consts.VersionNumber}}},
                { "id", 1 }
            });

            if (WriteLine(line))
            {
                if ((line = ReadLine()) == null)
                    throw new Exception("Disconnected from stratum server.");
                JContainer result;
                try
                {
                    Dictionary<String, Object> response = JsonConvert.DeserializeObject<Dictionary<string, Object>>(line);
                    result = ((JContainer)response["result"]);
                    var status = (String)(result["status"]);
                    if (status != "OK")
                        throw new Exception("Authorization failed.");
                }
                catch (Exception)
                {
                    throw (new Exception("No answer."));
                }

                try { mMutex.WaitOne(5000); } catch (Exception) { }
                mUserID = (String)(result["id"]);
                mJob = new Job(this, (String)(((JContainer)result["job"])["job_id"]), (String)(((JContainer)result["job"])["blob"]), (String)(((JContainer)result["job"])["target"]), (String)(((JContainer)result["job"])["variant"]));
                try { mMutex.ReleaseMutex(); } catch (Exception) { }
            }
            else
            {
                Reconnect();
            }
        }

        public void Submit(OpenCLDevice device, Job job, UInt32 output, String result)
        {
            try { mMutex.WaitOne(5000); } catch (Exception) { }
            ReportSubmittedShare(device);
            try
            {
                String stringNonce = String.Format("{0:x2}{1:x2}{2:x2}{3:x2}", ((output >> 0) & 0xff), ((output >> 8) & 0xff), ((output >> 16) & 0xff), ((output >> 24) & 0xff));
                String message = JsonConvert.SerializeObject(new Dictionary<string, Object> {
                    { "method", "submit" },
                    { "params", new Dictionary<String, String> {
                        { "id", mUserID },
                        { "job_id", job.ID },
                        { "nonce", stringNonce },
                        { "result", result }}},
                    { "id", 4 }});
                if(WriteLine(message))
                    Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"Device #{device.ADLAdapterIndex} submitted a share." });
                else
                {
                    Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"Device #{device.ADLAdapterIndex} failed to submit share (network issue)" });
                    Reconnect();
                }
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"Failed to submit share: {ex.Message}" });
                Reconnect();
            }
            try { mMutex.ReleaseMutex(); } catch (Exception) { }
            CheckHappening();
        }

        public new Work GetWork()
        {
            return new Work(mJob);
        }

        public CryptoNightStratum(List<PoolSettingsXml> pPools)
            : base(pPools, "cryptonight")
        {
        }
    }
}
