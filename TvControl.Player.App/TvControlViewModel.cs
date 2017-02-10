using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

using PropertyChanged;

using ReactiveUI;

using PropertyChangingEventArgs = ReactiveUI.PropertyChangingEventArgs;
using PropertyChangingEventHandler = ReactiveUI.PropertyChangingEventHandler;

namespace TvControl.Player.App
{
    [ImplementPropertyChanged]
    public class TvControlViewModel : ViewModelBase
    {

        private readonly IPlaybackControl control;

        private readonly ITvStations tvStations;
        private ObservableAsPropertyHelper<TvStation> selectedStation;

        public TvControlViewModel(ITvStations tvStations, IPlaybackControl control)
        {
            this.tvStations = tvStations;
            this.control = control;

            this.InitAsync();
            this.Volume = control.Volume;

            this.WhenAnyValue(model => model.SelectedIndex).Select(i => this.TvStations[i]).ToProperty(this, model => model.SelectedStation, out selectedStation);
            this.WhenAnyValue(model => model.SelectedStation).Subscribe(station => { this.control.SetStation(station); });

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

        public TvStation SelectedStation => this.selectedStation.Value;

        private async Task InitAsync()
        {
            this.TvStations = new ObservableCollection<TvStation>(await this.tvStations.GetAllAsync());
        }

    }

    public class ViewModelBase : ReactiveObject, INotifyPropertyChanged
    {
        public void OnPropertChanged(string propertyName)
        {
            this.RaisePropertyChanged(propertyName);
        }

    }
}