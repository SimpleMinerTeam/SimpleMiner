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

    }
}
