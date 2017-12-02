using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.ComponentModel;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using SimpleCPUMiner.Messages;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Threading;
using SimpleCPUMiner.View;
using System.Windows.Threading;
using System.Runtime;

namespace SimpleCPUMiner.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public RelayCommand<int> startMiningCommand { get; private set; }
        public RelayCommand<int> ShowAboutCommand { get; private set; }
        public ObservableCollection<MinerSettings> MinerSettingsList { get; set; }
        private UserConfiguration userConfiguration;
        public MinerSettings SelectedMinerSettings { get; set;}
        public RelayCommand<Window> CloseWindowCommand { get; private set; }
        public RelayCommand<string> CpuAffinityCommand { get; private set; }
        public RelayCommand<int> LaunchOnWindowsStartup { get; private set; }
        public Thread MinerThread { get; set; }
        public ObservableCollection<string> MinerOutputString { get; set;}
        public bool IsIdle { get; set; }
        public bool isEnabledCPUThreadAuto { get; set; }
        private string _threadNumber;
        private bool isAutostartMining;
        private int startingDelayInSec;
        private string startMiningButtonContent { get; set; }
        private int tempDelayInSec;
        private bool isCountDown = false;
        private DispatcherTimer timer;

        public string ThreadNumber
        {
            get { return _threadNumber; }
            set
            {
                _threadNumber = value;
                SelectedMinerSettings.NumberOfThreads = value;

                if (value.Equals("0") && IsIdle)
                    isEnabledCPUThreadAuto = true;
                else
                    isEnabledCPUThreadAuto = false;

                RaisePropertyChanged(nameof(IsEnabledCPUThreadAuto));
            }
        }

        public bool IsEnabledCPUThreadAuto
        {
            get
            {
                if (!IsIdle) return false;
                else return isEnabledCPUThreadAuto;
            }
        }

        public bool IsAutoStartMining
        {
            get
            {
                return isAutostartMining;
            }
            set
            {
                isAutostartMining = value;
                SelectedMinerSettings.IsAutostartMining = value;
                saveMinerSettings();
            }
        }

        public int StartingDelayInSec
        {
            get {
                
                return startingDelayInSec;
            }
            set
            {
                startingDelayInSec = value;
                SelectedMinerSettings.StartingDelayInSec = value;
                RaisePropertyChanged(nameof(StartingDelayInSec));
            }
        }

        public string MainWindowTitle
        {
            get
            {
#if DEBUG
                return "Simple Miner " + Consts.VersionNumber + " - DEBUG MODE!!!";
#else
                return "Simple Miner " + Consts.VersionNumber;
#endif
            }
        }

        public string StartMiningButtonContent
        {
            get
            {
                if (IsIdle == true)
                {
                    isCountDown = false;
                    RaisePropertyChanged(nameof(StartingDelayInSec));
                    return "Start mining";
                }
                else
                {
                    if (isCountDown == true)
                    {
                        return $"Stop mining ({tempDelayInSec})";
                    }
                    else
                    {
                        return $"Stop mining";
                    }
                }
            }
        
            set
            {
                StartMiningButtonContent = (IsIdle == true) ? "Start mining" : "Stop mining";
                RaisePropertyChanged(nameof(StartMiningButtonContent));
            }
        }

        ExeManager MyManager;

        public MainViewModel()
        {
            terminateProcess();
            loadConfigFile();
            IsIdle = true;
            MinerOutputString = new ObservableCollection<string>();
            SelectedMinerSettings = userConfiguration.SettingsList[userConfiguration.SelectedConfigIndex];
            ThreadNumber = SelectedMinerSettings.NumberOfThreads;
            isAutostartMining = SelectedMinerSettings.IsAutostartMining;
            StartingDelayInSec = SelectedMinerSettings.StartingDelayInSec;
            MinerSettingsList = new ObservableCollection<MinerSettings>();
            this.startMiningCommand = new RelayCommand<int>(startMining);
            this.CloseWindowCommand = new RelayCommand<Window>(CloseWindow);
            this.LaunchOnWindowsStartup = new RelayCommand<int>(setStartup);
            this.ShowAboutCommand = new RelayCommand<int>(ShowAbout);
            this.CpuAffinityCommand = new RelayCommand<string>(ShowCpuAffinity);
            Messenger.Default.Register<MinerOutputMessage>(this, msg => { minerOutputReceived(msg); });
            Messenger.Default.Register<CpuAffinityMessage>(this, msg => { CpuAffinityReceived(msg); });
            tempDelayInSec = SelectedMinerSettings.StartingDelayInSec;
            SelectedMinerSettings.IsLaunchOnWindowsStartup = Utils.IsStartupItem();
            if (SelectedMinerSettings.IsAutostartMining) autoStartMiningWithDelay();
        }

        private void CpuAffinityReceived(object msg)
        {
            var mm = msg as CpuAffinityMessage;
            if (mm != null)
            {
                SelectedMinerSettings.CPUAffinity = mm.CpuAffinity;
                RaisePropertyChanged(nameof(SelectedMinerSettings));
            }
        }

        private void ShowCpuAffinity(string pThreadNumber)
        {
            CpuAffinity ca = new CpuAffinity();
            var vm = new CpuAffinityViewModel();

            if (String.IsNullOrEmpty(_threadNumber))
                vm.ThreadNumber = "0";
            else
                vm.ThreadNumber = _threadNumber;

            vm.CalcCpuAffinity();
            ca.DataContext = vm;
            ca.ShowDialog();
        }

        private void ShowAbout(int obj)
        {
            var window = new About();
            window.ShowDialog();
        }

        //Automatikusan elindítja a bányászást
        private void autoStartMiningWithDelay()
        {
            if (SelectedMinerSettings.StartingDelayInSec > 0)
            {
                isCountDown = true;
                IsIdle = false;
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += Timer_Elapsed;
                timer.Start();
            }
        }

        private void Timer_Elapsed(object sender, EventArgs e)
        {
            if (IsIdle == false && isCountDown == true)
            {
                tempDelayInSec--;
                RaisePropertyChanged(nameof(StartingDelayInSec));
                RaisePropertyChanged(nameof(StartMiningButtonContent));

                if (tempDelayInSec == 0)
                {
                    timer.Stop();
                    isCountDown = false;
                    IsIdle = true;
                    startMining(0);
                }
            }
        }

        /// <summary>
        /// Beállítja, hogy induljon-e a Windowssal a program
        /// </summary>
        private void setStartup(int obj)
        {
            if (SelectedMinerSettings.IsLaunchOnWindowsStartup == true) Utils.AddProgramToStartup();
            else Utils.RemoveProgramFromStartup();
        }

        private void minerOutputReceived(MinerOutputMessage msg)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => {
                if (MinerOutputString.Count > 100)
                    MinerOutputString.RemoveAt(0);

                MinerOutputString.Add(msg.OutputText);
                if (msg.IsError)
                {
                    IsIdle = true;
                    RaisePropertyChanged(nameof(IsIdle));
                    RaisePropertyChanged(nameof(StartMiningButtonContent));
                }
            });
        }

        internal CancelEventHandler ApplicationClosing()
        {
            var ce = new CancelEventHandler((object sender, System.ComponentModel.CancelEventArgs e) => {
                terminateProcess();
            });
            return ce;
        }

        private void terminateProcess()
        {
            try
            {
                if (MinerThread != null && MinerThread.IsAlive)
                {
                    MinerThread.Abort();

                    foreach (Process proc in Process.GetProcessesByName(Consts.ProcessName))
                    {
                        proc.Kill();
                    }
                    MinerThread.Join();
                }

                Thread.Sleep(500);
                string rPath = Consts.ApplicationPath + Consts.ExeFileName;
                if (File.Exists(rPath)) File.Delete(rPath);
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show(ex.Message);
#endif
            }
        }

        /// <summary>
        /// A miner program elindítása a megadott paraméterekkel.
        /// </summary>
        private void startMining(int window)
        {
            //talán csökkenti a memória fragmentációt
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
            if (IsIdle)
            {
#if DEBUG
                MessageBox.Show(generateMinerCall());
#endif
                MinerOutputString.Clear();
                saveMinerSettings();
                MyManager = new ExeManager(Properties.Resources.cpuminer, Consts.ApplicationPath);

                MinerThread = new Thread(() => MyManager.ExecuteResource(generateMinerCall()));
                //MinerThread.Priority = ThreadPriority.Highest;
                MinerThread.Start();
                IsIdle = false;
            }
            else
            {
                terminateProcess();
                IsIdle = true;
                if (!isCountDown){ Messenger.Default.Send(new MinerOutputMessage() { OutputText = "Mining finished!" }); }
            }
            RaisePropertyChanged(nameof(IsIdle));
            RaisePropertyChanged(nameof(StartMiningButtonContent));
            RaisePropertyChanged(nameof(IsEnabledCPUThreadAuto));
        }

        //Betölti a konfigurációt a bin fájlból vagy ha nem találja vagy nem beolvasható, akkor beállítja a default értékeket 
        private void loadConfigFile()
        {
            if (!File.Exists(Consts.ConfigFilePath))
            {
                setDefaultConfigValues();
            }
            else
            {
                userConfiguration = Utils.DeSerializeObject<UserConfiguration>(Consts.ConfigFilePath);

                if (userConfiguration == null)
                {
                    setDefaultConfigValues();
                }
            }
        }

        /// <summary>
        /// Alapértékek beállítása
        /// </summary>
        private void setDefaultConfigValues()
        {
            userConfiguration = new UserConfiguration();
            userConfiguration.SelectedConfigIndex = 0;
            userConfiguration.SettingsList.Add(new MinerSettings()
            {
                URL = Consts.DefaultSettings.URL,
                Port = Consts.DefaultSettings.Port,
                Username = Consts.DefaultSettings.UserName,
                Password = Consts.DefaultSettings.Password,
                DonateLevel = Consts.DefaultSettings.DonateLevel,
                IsKeepalive = Consts.DefaultSettings.IsKeepalive,
                NoColor = Consts.DefaultSettings.IsNoColor,
                IsLogging = Consts.DefaultSettings.IsLogging,
                IsNicehashSupport = Consts.DefaultSettings.IsNicehashSupport,
                IsBackgroundMining = Consts.DefaultSettings.IsBackgroundMining,
                IsLaunchOnWindowsStartup = Consts.DefaultSettings.IsLaunchOnWindowsStartup,
                IsAutostartMining = Consts.DefaultSettings.IsAutostartMining,
                StartingDelayInSec = Consts.DefaultSettings.StartingDelayInSec,
                IsMinimizeToTray = Consts.DefaultSettings.IsMinimizeToTray,
                NumberOfThreads = Consts.DefaultSettings.NumberOfThreads,
                MaxCPUUsage = Consts.DefaultSettings.MaxCpuUsage,
                RetryPause = Consts.DefaultSettings.RetryPause
                
            });
        }

        /// <summary>
        /// Legenerálja a felparaméterezett miner hívást.
        /// </summary>
        /// <returns>Az exe paraméterezett hívása</returns>
        private string generateMinerCall()
        {
            StringBuilder sb = new StringBuilder($" -o {SelectedMinerSettings.URL}:{SelectedMinerSettings.Port} -u {SelectedMinerSettings.Username} -p {SelectedMinerSettings.Password}");
            sb.Append((SelectedMinerSettings.Algo == null) ? "" : " -a " + SelectedMinerSettings.Algo);
            sb.Append((SelectedMinerSettings.NumberOfThreads.Equals("0") || SelectedMinerSettings.NumberOfThreads.Equals("")) ? "" : $" -t {SelectedMinerSettings.NumberOfThreads}");
            sb.Append((SelectedMinerSettings.AlgorithmVariation == null) ? "" : $" -v {SelectedMinerSettings.AlgorithmVariation}");
            sb.Append((SelectedMinerSettings.IsKeepalive == true) ? " -k" : "");
            sb.Append((SelectedMinerSettings.NumOfRetries == 0) ? "" : $" -r {SelectedMinerSettings.NumOfRetries}");
            sb.Append((SelectedMinerSettings.RetryPause == 0) ? "" : $" -R {SelectedMinerSettings.RetryPause}");
            sb.Append(!String.IsNullOrEmpty(SelectedMinerSettings.CPUAffinity) ? $" --cpu-affinity {SelectedMinerSettings.CPUAffinity}" : "");
            sb.Append((SelectedMinerSettings.NoHugePages == true) ? " --no-huge-pages" : "");
            sb.Append((SelectedMinerSettings.NoColor == true) ? " --no-color" : "");
            sb.Append($" --donate-level={SelectedMinerSettings.DonateLevel}");
            sb.Append((SelectedMinerSettings.UserAgent == true) ? " --user-agent" : "");
            sb.Append((SelectedMinerSettings.IsBackgroundMining == true) ? " -B" : "");
            sb.Append((SelectedMinerSettings.JSONConfigfile == null) ? "" : $" -c {SelectedMinerSettings.JSONConfigfile}");
            sb.Append((SelectedMinerSettings.IsLogging == true) ? $" -l {DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}.log" : "");
            sb.Append((SelectedMinerSettings.MaxCPUUsage <= 1 || !SelectedMinerSettings.NumberOfThreads.Equals("0")) ? "" : $" --max-cpu-usage={SelectedMinerSettings.MaxCPUUsage}");
            sb.Append((SelectedMinerSettings.Safe == true) ? " --safe" : "");
            sb.Append((SelectedMinerSettings.IsNicehashSupport == true) ? " --nicehash" : "");
            sb.Append((SelectedMinerSettings.PrintHashRate == 0) ? "" : $" --print-time={SelectedMinerSettings.PrintHashRate}");

            return sb.ToString();
        }

        private void CloseWindow(Window window)
        {
            saveMinerSettings();

            if (window != null)
            {
                window.Close();
            }
        }

        /// <summary>
        /// Elmenti a felhasználó által megadott beállításokat
        /// </summary>
        private void saveMinerSettings()
        {
            Utils.SerializeObject(Consts.ConfigFilePath,userConfiguration);
        }
    }

    class ExeManager
    {
        byte[] exeMemory;
        string rPath;

        public ExeManager(byte[] FileBytes, string DestinationPath)
        {
            exeMemory = FileBytes;
            rPath = DestinationPath + @"\" + Consts.ExeFileName;
        }

        /// <summary>
        /// Kiírja a resourcesben lévő exe fájlt, majd futtatja.
        /// </summary>
        public void ExecuteResource(string _parameters)
        {
            Process x = null;
            try
            {
                //1) Fetch EXe file content from Resources
                byte[] exeFile = exeMemory;

                //2) Create file to be deleted complete execution
                FileStream aFile = new FileStream(rPath, FileMode.Create, FileAccess.Write, FileShare.None, 20000, FileOptions.None);

                //3) Write Exe file content
                aFile.Write(exeFile, 0, exeFile.Length);
                aFile.Flush();
                aFile.Close();
                while (!File.Exists(rPath));
                var startInfo = new ProcessStartInfo() {
                    Arguments = _parameters,
                    FileName = rPath,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                x = Process.Start(startInfo);
               // x.PriorityClass = System.Diagnostics.ProcessPriorityClass.High;
                x.EnableRaisingEvents = true;
                x.OutputDataReceived += X_OutputDataReceived;
                x.BeginOutputReadLine();
                x.WaitForExit();
                if (File.Exists(rPath)) File.Delete(rPath);
                Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = "Something went wrong! Check miner parameters!", IsError = true });
            }
            catch (ThreadAbortException)
            {
                if (x != null && !x.HasExited)
                {
                    x.Kill();
                    if (File.Exists(rPath)) File.Delete(rPath);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error");
                if (File.Exists(rPath)) File.Delete(rPath);
            }
        }

        private void X_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = e.Data });
        }

        private void sendOutputData(object sender, DataReceivedEventArgs e)
        {
            MessageBox.Show(e.Data);
            //Messenger.Default.Send(new MinerOutputMessage() { OutputText = "Itt az üzenet!" });
        }
    }
}