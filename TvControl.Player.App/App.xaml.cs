using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Windows;

using ReactiveUI;

namespace TvControl.Player.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            this.InitializeComponent();
            base.ShutdownMode = ShutdownMode.OnMainWindowClose;

            RxApp.MainThreadScheduler = new DispatcherScheduler(this.Dispatcher);
            RxApp.SupportsRangeNotifications = false;
        }

        protected override void OnActivated(EventArgs e)
        {

            base.OnActivated(e);
        }

    }
}
