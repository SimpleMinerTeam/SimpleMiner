using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.ComponentModel;
using GalaSoft.MvvmLight.Messaging;
using SimpleCPUMiner.Messages;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Threading;
using SimpleCPUMiner.View;
using System.Windows.Threading;
using System.Runtime;
using SimpleCPUMiner.Model;
using static SimpleCPUMiner.Consts;
using System.Linq;
using Microsoft.Win32;
using SimpleCPUMiner.Hardware;
using SimpleCPUMiner.Miners;
using System.Windows.Controls;
using System.Net;
using Newtonsoft.Json;
using Cloo;
using System.Drawing;

namespace SimpleCPUMiner.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public RelayCommand<int> startMiningCommand { get; private set; }
        public RelayCommand<int> ShowAboutCommand { get; private set; }
        public SimpleMinerSettings SelectedMinerSettings { get; set; }
        public Customization CustomSettings { get; set; }
        public Optimization SelectedOptimization { get; set; }
        public RelayCommand<Window> CloseWindowCommand { get; private set; }
        public RelayCommand<string> CpuAffinityCommand { get; private set; }
        public RelayCommand<int> LaunchOnWindowsStartup { get; private set; }
        public RelayCommand<string> PoolAddCommand { get; private set; }
        public RelayCommand<string> PoolRemoveCommand { get; private set; }
        public RelayCommand<string> PoolModifyCommand { get; private set; }
        public RelayCommand<string> ClickOnCheckbox { get; set; }
        public ObservableCollection<string> MinerOutputString { get; set; }
        public List<PoolSettingsXmlUI> Pools { get; set; }
        public List<OpenCLDevice> Devices { get; set; }
        public OpenCLDevice[] OpenCLDevices { get; set; }
        public ObservableCollection<Optimization> OptList { get; set; }
        public int SelectedPoolIndex { get; set; }
        public int SelectedOptIndex { get; set; }
        public int MainTabControlId { get; set; }
        public bool IsIdle { get; set; }
        public bool isEnabledCPUThreadAuto { get; set; }
        public string Hash { get; set; }
        public double Speed { get; set; }
        public bool ShowInTaskbar { get; set; }
        public Visibility WindowVisibility { get; set; }
        public WindowState WindowState { get; set; }
        public string MainWindowTitle { get; set; }
        public string IPAddress { get; set; }

        private PoolSettingsXmlUI _selectedPool;
        private string _threadNumber;
        private bool isAutostartMining;
        private int startingDelayInSec;
        private string startMiningButtonContent { get; set; }
        private int tempDelayInSec;
        private bool isCountDown = false;
        private DispatcherTimer timer;
        private DispatcherTimer updateDeviceStatTimer;
        private TabItem mainTabControl;
        private string uacButtonLabel = "UAC";
        private bool uacButtonIsEnabled;
        private bool isAdministrator;
        private Visibility isVisibleAdminLabel = Visibility.Collapsed;
        private List<MinerProcess> _minerProcesses = new List<MinerProcess>();
        private List<ComputeDevice> _cpus = new List<ComputeDevice>();

        #region propertik kibontva

        public PoolSettingsXmlUI SelectedPool
        {
            get
            {
                return _selectedPool;
            }
            set
            {
                _selectedPool = value;
            }
        }

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
            get
            {

                return startingDelayInSec;
            }
            set
            {
                startingDelayInSec = value;
                SelectedMinerSettings.StartingDelayInSec = value;
                RaisePropertyChanged(nameof(StartingDelayInSec));
            }
        }

        public Visibility IsVisibleAdminLabel
        {
            get
            {
                return isVisibleAdminLabel;
            }
            set
            {
                isVisibleAdminLabel = value;
            }
        }

        public string UacButtonLabel
        {
            get
            {
                return uacButtonLabel;
            }

            set
            {
                uacButtonLabel = value;
            }
        }

        public bool UACButtonIsEnabled
        {
            get
            {
                return uacButtonIsEnabled;
            }

            set
            {
                uacButtonIsEnabled = value;
            }
        }

        public bool IsAdministrator
        {
            get
            {
                return isAdministrator;
            }

            set
            {
                isAdministrator = value;
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
                    Messenger.Default.Send(new ActivePoolMessage() { IsActiveCPUPool = false, IsActiveGPUPool = false, URL = string.Empty, Port = 0 });
                    return "Start";
                }
                else
                {
                    if (isCountDown == true)
                    {
                        return $"Stop{Environment.NewLine}({tempDelayInSec})";
                    }
                    else
                    {
                        return $"Stop";
                    }
                }
            }

            set
            {
                StartMiningButtonContent = (IsIdle == true) ? "Start" : "Stop";
                RaisePropertyChanged(nameof(StartMiningButtonContent));
            }
        }

        public TabItem MainTabControl
        {
            get
            {
                return mainTabControl;
            }
            set
            {
                mainTabControl = value;
                if (value.Name.Equals("tabOptimization"))
                {
                    isAdministrator = Utils.CheckAdminPrincipal();
                    activateOptimizationTab();
                }
            }
        }

        private void activateOptimizationTab()
        {
            if (!isAdministrator && !Debugger.IsAttached)
            {
                UACButtonIsEnabled = false;
                IsVisibleAdminLabel = Visibility.Visible;
                RaisePropertyChanged(nameof(IsVisibleAdminLabel));
                return;
            }
            else
            {
                OptList = Optimize.GetOptList();
                RaisePropertyChanged(nameof(OptList));
            }
        }

        #endregion

        public Action RefreshPools { get; internal set; }
        public Action RefreshOpts { get; internal set; }
        public Action RefreshDevices { get; internal set; }

        public MainViewModel()
        {
            setDefaultValues();

            registerMessageListeners();

            Log.SetLogger(log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType));
            Log.InsertInfo($"Starting Simple Miner v{VersionNumber} OS: Windows{OSType}");

            Utils.TryKillProcess(Consts.ProcessName);

            loadConfigFile();
            SetApplicationMode(SelectedMinerSettings.ApplicationMode);

            if (SelectedMinerSettings.IsAutoUpdateEnabled)
            {
                if (Process.GetProcessesByName("SimpleMinerUpdater2").Length == 0)
                {

                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        File.Copy(AutoUpdatePathOrig, AutoUpdatePath, true);
                        var psi = new ProcessStartInfo()
                        {
                            FileName = AutoUpdatePath,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };

#if X86
                    psi.Arguments = "is32bit";
#endif
                    Process.Start(psi);
                    });
                }
            }

            if (SelectedMinerSettings.IsRemoteManagementEnabled)
            {
                List<SimpleDevice> devices = new List<SimpleDevice>();
                Devices.ForEach(x => devices.Add(new SimpleDevice()
                {
                    Activity = x.Activity,
                    Algo = string.Empty,
                    Fan = x.FanSpeed,
                    ID = x.ADLAdapterIndex,
                    Intensity = x.Intensity,
                    Name = x.Name,
                    Speed = x.Speed,
                    Temp = x.Temperature,
                    Threads = x.Threads,
                    Worksize = x.WorkSize,
                    Shares = x.Shares
                }));

                RemoteManagerHandler.SendMessageAsync(new SimpleMinerManager.Model.RegisterMessage() {
                    IP = IPAddress,
                    Worker = SelectedMinerSettings.RemoteManagerWorkerName,
                    Devices = devices,
                    RemotePoolListEnabled = SelectedMinerSettings.IsRemotePoolListEnabled,
                    State = "Idle",
                    Version = Consts.VersionNumber
                }, SelectedMinerSettings.RemoteManagerHost);

            }

            ThreadNumber = SelectedMinerSettings.NumberOfThreads;
            isAutostartMining = SelectedMinerSettings.IsAutostartMining;
            StartingDelayInSec = SelectedMinerSettings.StartingDelayInSec;

            SetCommands();

            tempDelayInSec = SelectedMinerSettings.StartingDelayInSec;
            if (SelectedMinerSettings.IsLaunchOnWindowsStartup && SelectedMinerSettings.IsLaunchOnWindowsStartup != Utils.IsStartupItem())
                setStartup(0);
            else
            {
                Utils.RemoveProgramFromStartup();
            }

            if (SelectedMinerSettings.IsAutostartMining)
                autoStartMiningWithDelay();

            if(IsGenerateDefaultPoolList) SavePools(null);

            if (CustomSettings != null)
            {
                AboutContact = CustomSettings.ContactEmailAddress ?? AboutContact;
            }

            if (SelectedMinerSettings.VersionNumber != VersionNumber)
            {
                DirectoryInfo di = new DirectoryInfo(ApplicationPath + "\\Miners\\Kernel\\Bins");

                if (di.Exists)
                {
                    foreach (var file in di.GetFiles())
                    {
                        file.Delete();
                    }
                }

                SelectedMinerSettings.VersionNumber = VersionNumber;
            }

            InitializeOpenCLDevices();
        }

        private void SetApplicationMode(string applicationMode)
        {
            switch (applicationMode)
            {
                case Consts.ApplicationMode.Normal:
                    WindowState = WindowState.Normal;
                    WindowVisibility = Visibility.Visible;
                    ShowInTaskbar = true;
                    break;
                case Consts.ApplicationMode.Silent:
                    WindowState = WindowState.Minimized;
                    WindowVisibility = Visibility.Hidden;
                    ShowInTaskbar = false;
                    break;
            }
        }

        private void registerMessageListeners()
        {
            Messenger.Default.Register<MinerOutputMessage>(this, msg => { minerOutputReceived(msg); });
            Messenger.Default.Register<CpuAffinityMessage>(this, msg => { CpuAffinityReceived(msg); });
            Messenger.Default.Register<StopMinerThreadsMessage>(this, msg => { StopMinerThreadsReceived(msg); });
            Messenger.Default.Register<ActivePoolMessage>(this, msg => { SetActivePool(msg); });
        }

        /// <summary>
        /// Alapértékek megadása
        /// </summary>
        private void setDefaultValues()
        {
            Pools = new List<PoolSettingsXmlUI>();
            Devices = new List<OpenCLDevice>();
            IsIdle = true;
            MainTabControlId = 0;
            MinerOutputString = new ObservableCollection<string>();
            updateDeviceStatTimer = new DispatcherTimer();
            updateDeviceStatTimer.Interval = new TimeSpan(0, 0, 5);
            updateDeviceStatTimer.Tick += UpdateDeviceStatTimer_Tick;
            updateDeviceStatTimer.Start();

            var ip = Utils.LocalIPAddress();
            IPAddress = ip != null ? ip.ToString() : string.Empty;
        }

        private void StopMinerThreadsReceived(StopMinerThreadsMessage msg)
        {
            Devices.ForEach(x => { x.SharesAccepted = 0; x.SharesRejected = 0; });
        }

        private void SetActivePool(ActivePoolMessage msg)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                foreach (var pool in Pools)
                {
                    if (msg.IsActiveCPUPool)
                    {
                        if (!string.IsNullOrEmpty(pool.URL) && pool.URL.Equals(msg.URL, StringComparison.InvariantCultureIgnoreCase) && pool.Port == msg.Port)
                        {
                            pool.IsActiveCPUPool = true;
                        }
                        else
                            pool.IsActiveCPUPool = false;
                    }
                    else if (msg.IsActiveGPUPool)
                    {
                        if (!string.IsNullOrEmpty(pool.URL) && pool.URL.Equals(msg.URL, StringComparison.InvariantCultureIgnoreCase) && pool.Port == msg.Port)
                        {
                            pool.IsActiveGPUPool = true;
                        }
                        else
                        {
                            pool.IsActiveGPUPool = false;
                        }
                    }
                    else
                        pool.IsActiveGPUPool = pool.IsActiveCPUPool = false;
                }
            });
        }

        private void UpdateDeviceStatTimer_Tick(object sender, EventArgs e)
        {
            Speed = 0;
            double cpuSpeed = 0;
            double gpuSpeed = 0;

            Devices.ForEach(x => { x.Speed = 0; });

            if (!IsIdle)
            {
                foreach (var mp in _minerProcesses)
                {
                    foreach (var miner in mp.Miners)
                    {
                        Devices.Where(x => x.ADLAdapterIndex == miner.Device.ADLAdapterIndex).FirstOrDefault().Speed += miner.Speed;
                        gpuSpeed += miner.Speed;
                    }
                }

                try
                {
                    if (SelectedMinerSettings.IsCPUMiningEnabled && SelectedMinerSettings.ApplicationMode.Equals(ApplicationMode.Normal))
                        using (var client = new WebClient())
                        {
                            var json = client.DownloadString("http://localhost:54321");
                            var response = JsonConvert.DeserializeObject<XmrigJson>(json);

                            if (response != null)
                            {
                                cpuSpeed += response.hashrate.total.First();

                                var cpuDevice = Devices.Where(x => x.ADLAdapterIndex == -2).FirstOrDefault();
                                if (cpuDevice != null)
                                {
                                    cpuDevice.Speed = cpuSpeed;
                                    cpuDevice.SharesAccepted = response.results.shares_good;
                                    cpuDevice.SharesRejected = response.results.shares_total - response.results.shares_good;
                                }
                            }
                        }
                }
                catch
                {
                    //ham megesszük
                };
            }

            Speed = cpuSpeed + gpuSpeed;

            if (SelectedMinerSettings.IsRemoteManagementEnabled)
            {
                List<SimpleDevice> devices = new List<SimpleDevice>();
                Devices.ForEach(x => devices.Add(new SimpleDevice()
                {
                    Activity = x.Activity,
                    Algo = string.Empty,
                    Fan = x.FanSpeed,
                    ID = x.ADLAdapterIndex,
                    Intensity = x.Intensity,
                    Name = x.Name,
                    Speed = x.Speed,
                    Temp = x.Temperature,
                    Threads = x.Threads,
                    Worksize = x.WorkSize,
                    Shares = x.Shares
                }));
                RemoteManagerHandler.SendMessageAsync(new SimpleMinerManager.Model.StatusMessage() { CPUSpeed = cpuSpeed, GPUSpeed = gpuSpeed, Speed = Speed, IP = IPAddress, Worker = SelectedMinerSettings.RemoteManagerWorkerName, Devices = devices, State = IsIdle ? "Idle" : "Working" }, SelectedMinerSettings.RemoteManagerHost);
            }

            Hash = $"Current speed {Speed:N2} h/s (CPU: {cpuSpeed:N2} h/s, GPU: {gpuSpeed:N2} h/s)";
            RaisePropertyChanged(nameof(Hash));
            RefreshDevices?.Invoke();
            //Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = Hash });
        }

        private void InitializeOpenCLDevices()
        {
            OpenCLDevices = Utils.GetAllOpenCLDevices(out _cpus);

            if (_cpus.Count > 0)
                Devices.Add(new OpenCLDevice(_cpus[0]) { ADLAdapterIndex = -2 });

            if (OpenCLDevices == null)
                return;

            try
            {
                Utils.InitializeADL(OpenCLDevices);
                Utils.InitializeNVML(OpenCLDevices);
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"Failed to initalize ADL, {ex.Message}{Environment.NewLine}{ex.InnerException}{Environment.NewLine}{ex.StackTrace}", IsError = true });
            }

            foreach (var item in OpenCLDevices.Where(x => x.IsUseable))
                Devices.Add(item);

            if (!GpuParameterHandler.ReadParameters(OpenCLDevices))
                GpuParameterHandler.WriteParameters(OpenCLDevices);
        }

        private void SetCommands()
        {
            startMiningCommand = new RelayCommand<int>(startMining);
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow);
            LaunchOnWindowsStartup = new RelayCommand<int>(setStartup);
            ShowAboutCommand = new RelayCommand<int>(ShowAbout);
            CpuAffinityCommand = new RelayCommand<string>(ShowCpuAffinity);
            PoolAddCommand = new RelayCommand<string>(AddPool);
            PoolModifyCommand = new RelayCommand<string>(ModifyPool);
            PoolRemoveCommand = new RelayCommand<string>(RemovePool);
        }

        private void AddPool(string obj)
        {
            var poolSettingWindow = new PoolForm();
            var poolVM = new PoolFormViewModel()
            {
                Pool = new PoolSettingsXmlUI() { IsRemoveable = true, IsGPUPool = true, IsCPUPool = true, CoinType = CoinTypes.OTHER }
            };

            poolVM.AddPool = AddPool;
            poolVM.UpdateCoinType();
            poolSettingWindow.DataContext = poolVM;
            poolSettingWindow.ShowDialog();
        }

        private void ModifyPool(string obj)
        {
            var poolSettingWindow = new PoolForm();
            var poolVM = new PoolFormViewModel()
            {
                Pool = new PoolSettingsXmlUI()
                {
                    CoinType = SelectedPool.CoinType,
                    FailOverPriority = SelectedPool.FailOverPriority,
                    IsCPUPool = SelectedPool.IsCPUPool,
                    IsGPUPool = SelectedPool.IsGPUPool,
                    IsFailOver = SelectedPool.IsFailOver,
                    IsMain = SelectedPool.IsMain,
                    IsRemoveable = SelectedPool.IsRemoveable,
                    Password = SelectedPool.Password,
                    Port = SelectedPool.Port,
                    URL = SelectedPool.URL,
                    Username = SelectedPool.Username,
                    Algorithm = SelectedPool.Algorithm,
                    Name = string.IsNullOrEmpty(SelectedPool.Name) ? SelectedPool.URL : SelectedPool.Name,
                    Website = SelectedPool.Website
                },
            };

            poolVM.UpdatePoolList = SavePools;
            poolVM.UpdateCoinType();
            poolVM.SelectedAlgo = Algorithms.Where(x => x.ID == Utils.MigrateAlgorithm(SelectedPool.Algorithm)).FirstOrDefault();
            poolSettingWindow.DataContext = poolVM;
            poolSettingWindow.ShowDialog();
        }

        private void SavePools(PoolSettingsXml ps)
        {
            if (ps != null)
            {
                if (ps.IsMain)
                {
                    var mainPool = Pools.Where(x => x.IsMain).FirstOrDefault();
                    if (mainPool != null)
                    {
                        mainPool.IsMain = false;
                    }
                }

                SelectedPool.CoinType = ps.CoinType;
                SelectedPool.URL = ps.URL.Trim();
                SelectedPool.FailOverPriority = ps.FailOverPriority;
                SelectedPool.IsCPUPool = ps.IsCPUPool;
                SelectedPool.IsFailOver = ps.IsFailOver;
                SelectedPool.IsGPUPool = ps.IsGPUPool;
                SelectedPool.IsMain = ps.IsMain;
                SelectedPool.Password = string.IsNullOrEmpty(ps.Password) ? string.Empty : ps.Password.Trim();
                SelectedPool.Port = ps.Port;
                SelectedPool.Username = ps.Username.Trim();
                SelectedPool.Algorithm = ps.Algorithm;
                SelectedPool.Name = ps.Name;
                SelectedPool.Website = ps.Website;
            }

            List<PoolSettingsXml> poolzToSave = new List<PoolSettingsXml>();

            Pools.ForEach(x => poolzToSave.Add(new PoolSettingsXml()
            {
                Algorithm = Utils.MigrateAlgorithm(x.Algorithm).ToString(),
                CoinType = x.CoinType,
                FailOverPriority = x.FailOverPriority,
                ID = x.ID,
                IsCPUPool = x.IsCPUPool,
                IsGPUPool = x.IsGPUPool,
                IsFailOver = x.IsFailOver,
                IsMain = x.IsMain,
                IsRemoveable = x.IsRemoveable,
                Name = x.Name,
                Password = x.Password,
                Port = x.Port,
                StatsAvailable = x.StatsAvailable,
                StatUrl = x.StatUrl,
                URL = x.URL,
                Username = x.Username,
                Website = x.Website
            }));
            PoolHandler.SavePools(poolzToSave);
            Pools = Pools.OrderByDescending(x => x.IsMain).ThenByDescending(x => x.IsFailOver).ThenBy(x => x.FailOverPriority).ToList();
            RaisePropertyChanged(nameof(Pools));
            RefreshPools?.Invoke();
        }

        private void RemovePool(string obj)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Pools.Remove(SelectedPool);
                SavePools(null);
            }
        }

        private void AddPool(PoolSettingsXmlUI ps)
        {
            if (ps.IsMain)
            {
                var mainPool = Pools.Where(x => x.IsMain).FirstOrDefault();
                if (mainPool != null)
                {
                    mainPool.IsMain = false;
                }
            }

            Pools.Add(ps);
            SavePools(null);
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
            if (!string.IsNullOrEmpty(msg.OutputText))
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    if (msg.OutputText.Contains("use pool"))
                    {
                        var tmpUrl = string.Empty;
                        var tmpPort = 0;

                        int startIndex = msg.OutputText.IndexOf("pool ") + 5;
                        tmpUrl = msg.OutputText.Substring(startIndex);
                        tmpPort = Convert.ToInt32(tmpUrl.Substring(tmpUrl.IndexOf(":") + 1, tmpUrl.IndexOf(" ") - tmpUrl.IndexOf(":")));
                        tmpUrl = tmpUrl.Substring(0, tmpUrl.IndexOf(":"));

                        Messenger.Default.Send(new ActivePoolMessage() { IsActiveCPUPool = true, IsActiveGPUPool = false, URL = tmpUrl, Port = tmpPort });
                    }

                    if (MinerOutputString.Count > 100)
                        MinerOutputString.RemoveAt(0);

                    MinerOutputString.Add(msg.OutputText.Replace('\r', ' '));
                    if (msg.IsError)
                    {
                        if (msg.Exception != null)
                        {
                            Log.InsertError(msg.OutputText, msg.Exception);
                        }
                        else
                        {
                            Log.InsertError(msg.OutputText);
                        }
                    }
                    else
                    {
                        if (SelectedMinerSettings == null || SelectedMinerSettings.IsLogging)
                            Log.InsertDebug(msg.OutputText);
                    }
                });
        }

        internal CancelEventHandler ApplicationClosing()
        {
            var ce = new CancelEventHandler((object sender, System.ComponentModel.CancelEventArgs e) =>
            {
                if (_minerProcesses != null && _minerProcesses.Count > 0)
                    _minerProcesses.ForEach(x => x.StopMiner());

                if (SelectedMinerSettings.IsAutoUpdateEnabled)
                    Utils.TryKillProcess("SimpleMinerUpdater2");
            });
            return ce;
        }


        /// <summary>
        /// A miner program elindítása a megadott paraméterekkel.
        /// </summary>
        private void startMining(int window)
        {
            if (OpenCLDevices != null && !SelectedMinerSettings.ApplicationMode.Equals(ApplicationMode.Silent) && 
                !StartMiningButtonContent.Equals("Stop",StringComparison.InvariantCultureIgnoreCase))
                GpuParameterHandler.WriteParameters(OpenCLDevices);

            if (Devices.Count > 0)
            {
                Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = "The following devices are initialized:" });

                foreach (var item in Devices)
                    Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"{item.Name} [w: {item.WorkSize}, i: {item.Intensity}, t: {item.Threads}]", IsError = true });
            }

            if (SelectedMinerSettings.IsCPUMiningEnabled && Utils.CheckInstallation() != CheckDetails.Installed)
            {
                if (Utils.InstallMiners() != InstallDetail.Installed)
                {
                    MessageBoxResult mResult = MessageBox.Show(String.Format("Simple Miner is corrupted, may the antivirus application blocked it. \nIf you already add it as an exception in your antivirus application, please try to download the miner again.\nWould you like to navigate to Simple Miner download page?"), "Miner error", MessageBoxButton.YesNo, MessageBoxImage.Stop);
                    if (mResult == MessageBoxResult.No)
                    {
                        return;
                    }
                    else
                    {
                        Process.Start(Consts.MinerDownload);
                        return;
                    }
                }
            }

            //talán csökkenti a memória fragmentációt
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
            if (IsIdle)
            {
                _minerProcesses.Clear();
                MinerOutputString.Clear();
                saveMinerSettings();

                if (OpenCLDevices != null && OpenCLDevices.Any(x => x.IsEnabled)) //videokártya indítása
                {
                    //if (IsAdministrator)
                    //{
                    //    var exeMinerManager = new ExeManager(Consts.ApplicationPath, Consts.ToolExeFileName);
                    //    exeMinerManager.ExecuteResource("disable \"*VEN_1002&DEV_6*\"");
                    //    Thread.Sleep(1000);
                    //    exeMinerManager.ExecuteResource("enable \"*VEN_1002&DEV_6*\"");
                    //}
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        var mp = new MinerProcess() { Algorithm = (int)SupportedAlgos.CryptoNight, Devices = OpenCLDevices, MinerType = MinerType.GPU, Settings = SelectedMinerSettings };
                        if (mp.InitalizeMiner(Pools.Where(x => x.IsGPUPool && (x.IsMain || x.IsFailOver)).ToList()))
                        {
                            mp.StartMiner();
                            _minerProcesses.Add(mp);
                        }
                        else
                        {
                            return;
                        }
                    });
                }

                if (SelectedMinerSettings.IsCPUMiningEnabled) //proci indítása
                {
                    var mp = new MinerProcess() { Algorithm = (int)SupportedAlgos.CryptoNight, Devices = OpenCLDevices, MinerType = MinerType.CPU, Settings = SelectedMinerSettings };

                    if (mp.InitalizeMiner(Pools.Where(x => x.IsCPUPool && (x.IsMain || x.IsFailOver)).ToList()))
                    {
                        mp.StartMiner();
                        _minerProcesses.Add(mp);
                    }
                    else
                    {
                        return;
                    }
                }

                IsIdle = false;
            }
            else
            {
                _minerProcesses.ForEach(x => x.StopMiner());
                IsIdle = true;

                if (!isCountDown)
                {
                    Messenger.Default.Send(new MinerOutputMessage() { OutputText = "Mining finished!" });
                }
            }
            RaisePropertyChanged(nameof(IsIdle));
            RaisePropertyChanged(nameof(StartMiningButtonContent));
            RaisePropertyChanged(nameof(IsEnabledCPUThreadAuto));
        }

        //Betölti a konfigurációt a bin fájlból vagy ha nem találja vagy nem beolvasható, akkor beállítja a default értékeket 
        private void loadConfigFile()
        {
            SelectedMinerSettings = ConfigurationHandler.GetConfig();
            CustomSettings = ConfigurationHandler.GetCustomConfig();
            var PoolList = PoolHandler.GetPools();

            foreach (var pool in PoolHandler.GetPools())
            {
                Pools.Add(new PoolSettingsXmlUI()
                {
                    Algorithm = pool.Algorithm,
                    CoinType = pool.CoinType,
                    FailOverPriority = pool.FailOverPriority,
                    ID = pool.ID,
                    IsCPUPool = pool.IsCPUPool,
                    IsGPUPool = pool.IsGPUPool,
                    IsFailOver = pool.IsFailOver,
                    IsMain = pool.IsMain,
                    IsRemoveable = pool.IsRemoveable,
                    Name = pool.Name,
                    Password = pool.Password,
                    Port = pool.Port,
                    StatsAvailable = pool.StatsAvailable,
                    StatUrl = pool.StatUrl,
                    URL = pool.URL,
                    Username = pool.Username,
                    Website = pool.Website
                });
            }

            SelectedPool = Pools[0];
            SelectedPoolIndex = 0;
        }


        private void CloseWindow(Window window)
        {
            saveMinerSettings();
            Thread.Sleep(500); //hogy kimentsük a fájlt

            if (window != null && !IsIdle)
            {
                MessageBoxResult mResult = MessageBox.Show("Simple Miner is actively working.\nDo you really want to exit?", "Exit confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (mResult == MessageBoxResult.No)
                {
                    return;
                }

                window.Close();
            }
            else if (window != null)
            {
                window.Close();
            }
        }

        /// <summary>
        /// Elmenti a felhasználó által megadott beállításokat
        /// </summary>
        private void saveMinerSettings()
        {
            if (!SelectedMinerSettings.ApplicationMode.Equals(ApplicationMode.Silent))
                ConfigurationHandler.WriteParameters(SelectedMinerSettings);
        }

        /// <summary>
        /// Megvizsgálja, hogy hozzá van-e adva a program mappája a Defender kivételekhez
        /// </summary>
        /// <returns></returns>
        private bool isExcludedThisFolder()
        {
            RegistryKey OurKey = Registry.LocalMachine;
            OurKey = OurKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows Defender\Exclusions\Paths");

            if (OurKey != null)
                foreach (string Keyname in OurKey.GetValueNames())
                {
                    Console.WriteLine(Keyname);

                    //Ha már hozzá van adva
                    if (Keyname == ApplicationPath) return true;
                }

            return false;
        }
    }
}