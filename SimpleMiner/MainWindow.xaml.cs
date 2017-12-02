using SimpleCPUMiner.ViewModel;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleCPUMiner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string testURL { get; set; }
        private static bool willNavigate;

        public MainWindow()
        {
            InitializeComponent();
            var vm = new MainViewModel();
            Closing += vm.ApplicationClosing();
            if (vm.SelectedMinerSettings.IsMinimizeToTray == true) this.WindowState = WindowState.Minimized;
            this.DataContext = vm;
        }

        public MainWindow(MinerSettings _minerSettings)
        {
            tbWalletAddress.Text = _minerSettings.Username;
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

        private void wbContent_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
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
