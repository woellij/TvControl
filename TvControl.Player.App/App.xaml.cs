using System.Reactive.Concurrency;
using System.Windows;

using ReactiveUI;

namespace TvControl.Player.App
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            this.InitializeComponent();
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            RxApp.MainThreadScheduler = new DispatcherScheduler(this.Dispatcher);
            RxApp.SupportsRangeNotifications = false;
        }

    }
}