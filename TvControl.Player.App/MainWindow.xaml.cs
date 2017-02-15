using System;
using System.Reactive.Concurrency;
using System.Windows;

using AutoMapper;

using Nancy.Hosting.Self;

using ReactiveUI;

using Splat;

using TinyMessenger;

using TvControl.ConsoleApp;

namespace TvControl.Player.App
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public NancyHost Host { get; private set; }

        private UDPListener udpListener;

        public MainWindow()
        {
            this.InitializeComponent();

            RxApp.MainThreadScheduler = new DispatcherScheduler(this.Dispatcher);

            Mapper.Initialize(expression =>
            {
                AutoMapperConfig.Init(expression);
                expression.CreateMissingTypeMaps = true;
            });

            var playerWindow = new PlayerWindow();
            playerWindow.Show();

            this.Init(playerWindow);
        }

        protected override void OnClosed(EventArgs e)
        {
            this.Host.Dispose();
            this.udpListener.Dispose();
            base.OnClosed(e);
        }

        private async void Init(PlayerWindow playerWindow)
        {
            var messenger = new TinyMessengerHub();

            TvControlViewModel viewModel;
            this.DataContext = playerWindow.DataContext = viewModel = new TvControlViewModel(new TvStations(), new MediaElementPlaybackControl(playerWindow));
            this.udpListener = new UDPListener(11011, s => viewModel.Log.Write(s, LogLevel.Debug));
            await this.udpListener.StartAsync();

            var uriString = "http://localhost:8090/tvcontrolapi/";
            this.Host = new NancyHost(new CustomNancyBoostrapper(), new HostConfiguration {
                UrlReservations = new UrlReservations {
                    CreateAutomatically = true
                }
            }, new Uri(uriString));
            this.Host.Start();
            Console.WriteLine($"Running on {uriString}");
            Console.ReadLine();
        }

        private void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                // Note that you can have more than one file.
                var files = (string[]) e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                var tvControlViewModel = (TvControlViewModel) this.DataContext;
                var station = ((FrameworkElement) sender).DataContext as TvStation;
                if (files.Length == 1) {
                    tvControlViewModel.AppendVideoFile(station, files[0]);
                }
                else if (files.Length > 1) {
                    tvControlViewModel.AppendVideoFiles(station, files);
                }
            }
        }

    }
}