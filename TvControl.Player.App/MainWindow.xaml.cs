using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using PropertyChanged;

using ReactiveUI;

using TinyMessenger;

namespace TvControl.Player.App
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            this.InitializeComponent();
            var playerWindow = new PlayerWindow();
            playerWindow.Show();

            var messenger = new TinyMessengerHub();

            this.DataContext = playerWindow.DataContext = new TvControlViewModel(new TvStations(), new MediaElementPlayback(playerWindow));
        }

        private void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                // Note that you can have more than one file.
                var files = (string[]) e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                var station = ((FrameworkElement) sender).DataContext as TvStation;
                ObservableCollection<TvStation> stations = ((TvControlViewModel) this.DataContext).TvStations;
                int index = stations.IndexOf(station);

                foreach (string file in files) {
                    TvStation tvStation = stations[index];
                    tvStation.FileUrl = new Uri(file);
                    tvStation.FileName = tvStation.FileUrl.Segments.LastOrDefault();
                    index++;
                }
            }
        }

    }

    public class DelayedAction : IDisposable
    {

        private readonly Action<CancellationToken> action;
        private readonly TimeSpan delay;
        private readonly Func<CancellationToken, Task> taskaction;
        private CancellationTokenSource cts;

        public DelayedAction(Action<CancellationToken> action, TimeSpan delay)
        {
            this.action = action;
            this.delay = delay;
        }

        public DelayedAction(Func<CancellationToken, Task> action, TimeSpan delay)
        {
            this.taskaction = action;
            this.delay = delay;
        }

        public void Dispose()
        {
            this.cts.Cancel();
        }

        public IDisposable Run(CancellationToken cancellationToken)
        {
            this.cts = new CancellationTokenSource();
            this.Schedule(CancellationTokenSource.CreateLinkedTokenSource(this.cts.Token, cancellationToken).Token);
            return this;
        }

        private async void Schedule(CancellationToken cancellation)
        {
            try {
                await Task.Delay(this.delay, cancellation);
                if (cancellation.IsCancellationRequested) {
                    return;
                }

                Func<CancellationToken, Task> func = this.taskaction;
                if (func != null) {
                    await func(cancellation).ConfigureAwait(false);
                }
                else {
                    Action<CancellationToken> action = this.action;
                    action?.Invoke(cancellation);
                }
            }
            catch (OperationCanceledException) {
            }
        }

    }

    public class MediaElementPlayback : IPlaybackControl
    {

        private readonly MediaElement mediaElement;
        private readonly DateTimeOffset startTime;

        private readonly PlayerWindow window;
        private IDisposable hideVolumeIndicatorAction;

        public MediaElementPlayback(PlayerWindow window)
        {
            this.window = window;
            this.mediaElement = window.MediaElement;
            this.startTime = DateTimeOffset.UtcNow;
        }

        public double Volume => this.mediaElement.Volume;

        public Task SetStationAsync(TvStation tvStation)
        {
            this.mediaElement.Source = tvStation.FileUrl;
            this.mediaElement.Position = DateTimeOffset.UtcNow - this.startTime;
            return Task.FromResult(tvStation);
        }

        public double ChangeVolume(int direction)
        {
            double volume = this.mediaElement.Volume + (direction > 0 ? .05 : -.05);
            volume = volume > 1 ? 1D : volume <= 0 ? 0 : volume;
            this.mediaElement.Volume = volume;
            this.ToggleVolumeIndicator();

            return volume;
        }

        private void ToggleVolumeIndicator()
        {
            this.window.VolumeIndicator.Visibility = Visibility.Visible;
            this.hideVolumeIndicatorAction?.Dispose();
            this.hideVolumeIndicatorAction =
                new DelayedAction(token => { this.window.VolumeIndicator.Visibility = Visibility.Collapsed; }, TimeSpan.FromSeconds(3)).Run(CancellationToken.None);
        }

    }

    public interface ITvStations
    {

        Task<TvStation[]> GetAllAsync();

    }

    class TvStations : ITvStations
    {

        private string[] resources;

        public Task<TvStation[]> GetAllAsync()
        {
            this.resources = this.GetType().Assembly.GetManifestResourceNames();

            List<TvStation> stations = this.GetStations().ToList();
            stations.ForEach(this.AppendImage);
            return Task.FromResult(stations.ToArray());
        }

        private void AppendImage(TvStation station)
        {
            string resource = this.resources.FirstOrDefault(filename => filename.Contains($".{station.Id}."));
            Stream stream = this.GetType().Assembly.GetManifestResourceStream(resource);
            var bitmapImage = new BitmapImage();

            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            station.Image = bitmapImage;
        }

        private IEnumerable<TvStation> GetStations()
        {
            yield return new TvStation("1", "ARD");
            yield return new TvStation("26", "ZDF");
            yield return new TvStation("5", "BR");
            yield return new TvStation("4", "3sat");
            yield return new TvStation("2", "arte");
            yield return new TvStation("16", "Pro 7");
            yield return new TvStation("19", "Sat1");
            yield return new TvStation("24", "VOX");
            yield return new TvStation("27", "Kabel 1");
            yield return new TvStation("35", "ARD alpha");
        }

    }

    public interface IPlaybackControl
    {

        double Volume { get; }

        Task SetStationAsync(TvStation tvStation);

        double ChangeVolume(int direction);

    }

    [ImplementPropertyChanged]
    public class TvStation
    {

        public TvStation(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public string Name { get; }

        public ImageSource Image { get; set; }

        public string Id { get; }
        public Uri FileUrl { get; set; }
        public string FileName { get; set; }

    }

    [ImplementPropertyChanged]
    public class TvControlViewModel
    {

        private readonly IPlaybackControl control;

        private readonly ITvStations tvStations;

        public TvControlViewModel(ITvStations tvStations, IPlaybackControl control)
        {
            this.tvStations = tvStations;
            this.control = control;

            this.InitAsync();
            this.Volume = control.Volume;

            this.WhenAnyValue(model => model.SelectedIndex).Subscribe(index => { this.control.SetStationAsync(this.SelectedStation); });
            this.WhenAnyValue(model => model.Volume).Subscribe(vol => this.DisplayVolume = Math.Round(100 * vol, 0, MidpointRounding.AwayFromZero).ToString());

            this.ChangeStationCommand = ReactiveCommand.Create<int>(dir =>
            {
                int newIndexAbsolute = this.SelectedIndex + (dir > 0 ? 1 : -1);
                this.SelectedIndex = newIndexAbsolute < 0 ? this.TvStations.Count - 1 : newIndexAbsolute >= this.TvStations.Count ? 0 : newIndexAbsolute;
            });

            this.ChangeVolumeCommand = ReactiveCommand.Create<int>(dir => { this.Volume = this.control.ChangeVolume(dir); });
        }

        public double Volume { get; set; }

        public string DisplayVolume { get; private set; }

        public ReactiveCommand<int, Unit> ChangeStationCommand { get; set; }

        public ObservableCollection<TvStation> TvStations { get; set; }

        public int SelectedIndex { get; set; }

        public ReactiveCommand<int, Unit> ChangeVolumeCommand { get; }

        private TvStation SelectedStation => this.TvStations[this.SelectedIndex];

        private async Task InitAsync()
        {
            this.TvStations = new ObservableCollection<TvStation>(await this.tvStations.GetAllAsync());
        }

    }
}