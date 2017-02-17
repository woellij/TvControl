using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

using PropertyChanged;

using ReactiveUI;

using TvControl.Player.App.Extensions;
using TvControl.Player.App.Model;

namespace TvControl.Player.App.ViewModels
{
    [ImplementPropertyChanged]
    public class TvControlViewModel : ViewModelBase
    {

        private readonly IPlaybackControl control;
        private readonly ObservableAsPropertyHelper<TvStation> selectedStation;

        private readonly ITvStations tvStations;

        public TvControlViewModel(ITvStations tvStations, IPlaybackControl control)
        {
            this.tvStations = tvStations;
            this.control = control;

            Current = this;

            this.InitAsync();
            this.Volume = control.Volume;

            this.WhenAnyValue(model => model.SelectedIndex)
                .Where(i => i > 0)
                .ObserveOnDispatcher()
                .Select(i => this.TvStations[i])
                .ToProperty(this, model => model.SelectedStation, out this.selectedStation);

            this.WhenAnyValue(model => model.SelectedStation).ObserveOnDispatcher().Subscribe(station => { this.control.SetStation(station); });

            this.WhenAnyValue(model => model.Volume).Subscribe(vol => this.DisplayVolume = Math.Round(100 * vol, 0, MidpointRounding.AwayFromZero).ToString());

            this.ChangeStationCommand = ReactiveCommand.Create<int>(dir =>
            {
                int newIndexAbsolute = this.SelectedIndex + (dir > 0 ? 1 : -1);
                this.SelectedIndex = newIndexAbsolute < 0 ? this.TvStations.Count - 1 : newIndexAbsolute >= this.TvStations.Count ? 0 : newIndexAbsolute;
            });

            this.ChangeVolumeCommand = ReactiveCommand.Create<int, Unit>(dir =>
            {
                RxApp.MainThreadScheduler.Schedule(() => { this.Volume = this.control.ChangeVolume(dir); });
                return Unit.Default;
            });
        }

        public static TvControlViewModel Current { get; private set; }

        public double Volume { get; set; }

        public string DisplayVolume { get; private set; }

        public ReactiveCommand<int, Unit> ChangeStationCommand { get; set; }

        public ObservableCollection<TvStation> TvStations { get; set; }

        public int SelectedIndex { get; set; } = -1;

        public ReactiveCommand<int, Unit> ChangeVolumeCommand { get; }

        public TvStation SelectedStation => this.selectedStation.Value;

        public LogViewModel Log { get; } = new LogViewModel();

        private async Task InitAsync()
        {
            this.TvStations = new ObservableCollection<TvStation>(await this.tvStations.GetAllAsync());
        }

        public void AppendVideoFile(TvStation station, string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            string stationId;
            if (!this.TryGetStationId(filePath, out stationId)) {
                string extension = Path.GetExtension(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                fileInfo.Rename(fileName + "_station_" + station.Id + "_" + extension);
            }
            SetFileProperties(station, filePath);
        }

        private bool TryGetStationId(string fileNameOrPath, out string stationId)
        {
            try {
                string[] split = Path.GetFileNameWithoutExtension(fileNameOrPath).Split(new[] { "_station_" }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length == 2) {
                    split = split[1].Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                    stationId = split[0];
                    return true;
                }
            }
            catch {
            }

            stationId = null;
            return false;
        }

        public void AppendVideoFiles(TvStation station, string[] files)
        {
            int index = this.TvStations.IndexOf(station);

            foreach (string file in files) {
                TvStation tvStation;
                string stationId;
                if (this.TryGetStationId(file, out stationId)) {
                    tvStation = this.TvStations.FirstOrDefault(s => s.Id == stationId);
                    if (tvStation != null) {
                        SetFileProperties(tvStation, file);
                        continue;
                    }
                }

                tvStation = this.TvStations[index];
                SetFileProperties(tvStation, file);
                index++;
            }
        }

        private static void SetFileProperties(TvStation tvStation, string file)
        {
            tvStation.FileUrl = new Uri(file);
            tvStation.FileName = tvStation.FileUrl.Segments.LastOrDefault();
        }

        public bool SetCurrentStation(string id)
        {
            TvStation selected = this.TvStations.FirstOrDefault(station => station.Id == id);
            return this.SetCurrentStation(selected);
        }

        public bool SetCurrentStation(TvStation selected)
        {
            if (selected != null) {
                this.SelectedIndex = this.TvStations.IndexOf(selected);
                return true;
            }

            return false;
        }

    }
}