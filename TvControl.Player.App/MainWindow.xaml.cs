using System;
using System.Windows;

using AutoMapper;

using Nancy.Hosting.Self;
using Nancy.TinyIoc;

using Splat;

using TinyMessenger;

using TvControl.ConsoleApp;
using TvControl.Player.App.Model;

namespace TvControl.Player.App
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private UDPListener udpListener;

        public MainWindow()
        {
            this.InitializeComponent();

            Mapper.Initialize(expression =>
            {
                AutoMapperConfig.Init(expression);
                expression.CreateMissingTypeMaps = true;
            });

            var playerWindow = new PlayerWindow();
            playerWindow.Show();

            this.Init(playerWindow);
        }

        public NancyHost Host { get; private set; }

        protected override void OnClosed(EventArgs e)
        {
            this.Host.Dispose();
            this.udpListener.Dispose();
            base.OnClosed(e);
        }

        private async void Init(PlayerWindow playerWindow)
        {
            var messenger = new TinyMessengerHub();

            var uriString = "http://localhost:8090/tvcontrolapi/";
            this.Host = new NancyHost(new CustomNancyBoostrapper(), new HostConfiguration {
                UrlReservations = new UrlReservations {
                    CreateAutomatically = true
                }
            }, new Uri(uriString));
            this.Host.Start();
            TinyIoCContainer tinyIoCContainer = TinyIoCContainer.Current;
            tinyIoCContainer.Register<ITasksService>((container, overloads) => new LocalTasksServiceDecorator(new FirebaseTasksService()));
            tinyIoCContainer.AutoRegister();

            var playbackControl = new MediaElementPlaybackControl(playerWindow);
            tinyIoCContainer.Register<IPlaybackControl>(playbackControl);
            TvControlViewModel viewModel;
            this.DataContext = playerWindow.DataContext = viewModel = new TvControlViewModel(new TvStations(), playbackControl);

            new TasksWindow { DataContext = tinyIoCContainer.Resolve<TasksViewModel>() }.Show();

            this.udpListener = new UDPListener(11011, s => viewModel.Log.Write(s, LogLevel.Debug));
            await this.udpListener.StartAsync();

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