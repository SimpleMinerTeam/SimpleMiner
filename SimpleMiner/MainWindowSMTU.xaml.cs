using SimpleCPUMiner.ViewModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace SimpleCPUMiner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowSMTU : Window
    {
        public string NotificationUrl { get; set; }
        private static bool willNavigate;
        private DispatcherTimer timer;

        public MainWindowSMTU()
        {
            InitializeComponent();
            var vm = new MainViewModel();
            Closing += vm.ApplicationClosing();
            vm.RefreshPools = RefreshPools;
            //vm.RefreshOpts = RefreshOpts; 
            vm.RefreshDevices = RefreshDevices;
            if (vm.SelectedMinerSettings.IsMinimizeToTray == true || vm.SelectedMinerSettings.ApplicationMode.Equals(Consts.ApplicationMode.Silent, StringComparison.InvariantCultureIgnoreCase)) this.WindowState = WindowState.Minimized;
            this.DataContext = vm;
            tcMaintabs.SelectedIndex = 0;
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(24, 0, 0);
            timer.Start();
            if (vm.CustomSettings == null || string.IsNullOrWhiteSpace(vm.CustomSettings.MainWindowTitle))
            {
#if DEBUG
                vm.MainWindowTitle = "Simple Miner " + Consts.VersionNumber + " - DEBUG MODE!!!";
#elif SMTU
                vm.MainWindowTitle = "Tu Simple Miner " + Consts.VersionNumber;
#else

                vm.MainWindowTitle = "Simple Miner " + Consts.VersionNumber;
#endif
            }
            else
            {
                vm.MainWindowTitle = $"{vm.CustomSettings.MainWindowTitle} (SM v{Consts.VersionNumber})";
            }

#if SMTU
            wbContent.Source = new Uri($"http://www.cryptomanager.net/minernotification.aspx?v={Consts.VersionNumber}&h={vm.Speed}&l=tusm");
#else
            if (vm.CustomSettings == null || string.IsNullOrEmpty(vm.CustomSettings.NotificationURL))
                wbContent.Source = new Uri($"http://cryptomanager.net/minernotification.aspx?v={Consts.VersionNumber}&h={vm.Speed}");
            else
                try
                {
                    wbContent.Source = new Uri(vm.CustomSettings.NotificationURL);
                }
                catch (Exception ex)
                {
                    wbContent.Source = new Uri($"http://cryptomanager.net/minernotification.aspx?v={Consts.VersionNumber}&h={vm.Speed}");
                    Log.InsertError(ex.Message);
                }
#endif

#if X86
            tabGPU.Visibility = Visibility.Collapsed;
#endif
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                try
                {
                    willNavigate = false;
                    var vm = DataContext as MainViewModel;
                    if (vm.CustomSettings == null || string.IsNullOrEmpty(vm.CustomSettings.NotificationURL))
                        wbContent.Source = new Uri($"http://cryptomanager.net/minernotification.aspx?v={Consts.VersionNumber}&h={vm.Speed}");
                    else
                        wbContent.Source = new Uri(vm.CustomSettings.NotificationURL);

                    //wbContent.Refresh();
                    wbContent.Navigate(wbContent.Source);
                }
                catch
                {
                    //ham megesszük
                }
            }));

        }

        public MainWindowSMTU(MinerSettings _minerSettings)
        {
        }

        private void CtrlCCopyCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ListView lv = (ListView)(sender);
            var selected = lv.SelectedItem;

            if (selected != null)
                Clipboard.SetText(selected.ToString());
        }

        private void CtrlCCopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RightClickCopyCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            var selected = mi.DataContext;

            if (selected != null)
                Clipboard.SetText(selected.ToString());
        }

        private void PortPrevTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange > 0.0)
                ((ScrollViewer)e.OriginalSource).ScrollToEnd();
        }

        private void RefreshPools()
        {
            lvPools.Items.Refresh();
        }

        //private void RefreshOpts()
        //{
        //    tvOptlist.Items.Refresh();
        //}

        private void RefreshDevices()
        {
            lvDevices.Items.Refresh();
        }

        private void wbContent_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(e.Uri);
                request.Timeout = 10000;
                var resp = request.GetResponse() as HttpWebResponse;

                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    e.Cancel = true;
                    return;
                }
            }
            catch
            {
                e.Cancel = true;
                return;
            }
            
            //első oldalt a progiba nyissuk meg
            if (!willNavigate)
            {
                willNavigate = true;
                return;
            }

            e.Cancel = true;

            if (e.Uri != null)
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = e.Uri.ToString()
                };

                Process.Start(startInfo);
            }
        }

        private void MyMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            setWindowStyle(false);
        }

        private void setWindowStyle(bool pHide)
        {
            var vm = DataContext as MainViewModel;
            if (pHide || vm.SelectedMinerSettings.ApplicationMode.Equals(Consts.ApplicationMode.Silent, StringComparison.InvariantCultureIgnoreCase))
            {

                WindowInteropHelper wndHelper = new WindowInteropHelper(this);

                int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);

                exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
                SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
            }
        }

#region Window styles
        [Flags]
        public enum ExtendedWindowStyles
        {
            // ...
            WS_EX_TOOLWINDOW = 0x00000080,
            // ...
        }

        public enum GetWindowLongFields
        {
            // ...
            GWL_EXSTYLE = (-20),
            // ...
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            int error = 0;
            IntPtr result = IntPtr.Zero;
            // Win32 SetWindowLong doesn't clear error on success
            SetLastError(0);

            if (IntPtr.Size == 4)
            {
                // use SetWindowLong
                Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(tempResult);
            }
            else
            {
                // use SetWindowLongPtr
                result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                error = Marshal.GetLastWin32Error();
            }

            if ((result == IntPtr.Zero) && (error != 0))
            {
                throw new System.ComponentModel.Win32Exception(error);
            }

            return result;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);
#endregion
    }
}
