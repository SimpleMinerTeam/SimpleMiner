using SimpleCPUMiner.ViewModel;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace SimpleCPUMiner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string testURL { get; set; }
        private static bool willNavigate;
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            var vm = new MainViewModel();
            Closing += vm.ApplicationClosing();
            vm.RefreshPools = RefreshPools;
            if (vm.SelectedMinerSettings.IsMinimizeToTray == true) this.WindowState = WindowState.Minimized;
            this.DataContext = vm;
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(24, 0, 0);
            timer.Start();
            wbContent.Source = new Uri($"http://cryptomanager.net/minernotification.aspx?v={Consts.VersionNumber}");
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            willNavigate = false;
            wbContent.Refresh();
        }

        public MainWindow(MinerSettings _minerSettings)
        {
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

    }
}
