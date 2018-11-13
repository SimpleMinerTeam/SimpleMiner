using GalaSoft.MvvmLight.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace SimpleCPUMiner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            DispatcherHelper.Initialize();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
#if SMIO
            var startupWindow = new MainWindowSMIO();
            startupWindow.Show();
#elif SMPoolTester
            var startupWindow = new MainWindowPoolTester();
            startupWindow.Show();
#elif SMTU
            var startupWindow = new MainWindowSMTU();
            startupWindow.Show();
#else
            var startupWindow = new MainWindow();
            startupWindow.Show();
#endif

        }
    }
}
