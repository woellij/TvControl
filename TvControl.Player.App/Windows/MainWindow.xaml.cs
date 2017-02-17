using System;
using System.Windows;

using AutoMapper;

using Nancy.Hosting.Self;
using Nancy.TinyIoc;

using Splat;

using TvControl.ConsoleApp;
using TvControl.Player.App.Api;
using TvControl.Player.App.Messenger;
using TvControl.Player.App.Model;
using TvControl.Player.App.ViewModels;

namespace TvControl.Player.App.Windows
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
            var uriString = "http://localhost:8090/tvcontrolapi/";
            this.Host = new NancyHost(new CustomNancyBoostrapper(), new HostConfiguration {
                UrlReservations = new UrlReservations {
                    CreateAutomatically = true
                }
            }, new Uri(uriString));
            this.Host.Start();

            TinyIoCContainer ioc = TinyIoCContainer.Current;
            ioc.AutoRegister();
            ioc.Register<ITasksService>((container, overloads) => new LocalTasksServiceDecorator(new FirebaseTasksService()));

            var playbackControl = new MediaElementPlaybackControl(playerWindow);
            ioc.Register<IPlaybackControl>(playbackControl);
            TvControlViewModel viewModel;
            this.DataContext = playerWindow.DataContext = viewModel = new TvControlViewModel(new TvStations(), playbackControl);

            ioc.Register<ILogger>(viewModel.Log);

            viewModel.Log.Write($"API Running on {uriString}", LogLevel.Info);

            new TasksWindow { DataContext = ioc.Resolve<TasksViewModel>() }.Show();

            this.udpListener = new UDPListener(11011, s => viewModel.Log.Write(s, LogLevel.Debug));
            await this.udpListener.StartAsync();
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