using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace TvControl.Player.App
{
    public class MediaElementPlaybackControl : IPlaybackControl
    {

        private readonly TimeSpan indicatorDisplayTime = TimeSpan.FromSeconds(3);

        private readonly MediaElement mediaElement;
        private readonly DateTimeOffset startTime;

        private readonly PlayerWindow window;
        private IDisposable hideStationIndicatorAction;
        private IDisposable hideVolumeIndicatorAction;

        public MediaElementPlaybackControl(PlayerWindow window)
        {
            this.window = window;
            this.mediaElement = window.MediaElement;
            this.startTime = DateTimeOffset.UtcNow;
        }

        public double Volume => this.mediaElement.Volume;

        public void SetStation(TvStation tvStation)
        {
            if (tvStation == null) {
                return;
            }

            if (tvStation.FileUrl == null) {
                try {
                    if (this.mediaElement.Source != null) {
                        this.mediaElement.Stop();
                    }
                }
                catch {
                }
                this.mediaElement.Source = null;
            }
            else {
                this.mediaElement.Source = tvStation.FileUrl;
                this.mediaElement.Position = DateTimeOffset.UtcNow - this.startTime;
                this.ToggleStationIndicator();
            }
        }

        public double ChangeVolume(int direction)
        {
            double volume = this.mediaElement.Volume + (direction > 0 ? .05 : -.05);
            volume = volume > 1 ? 1D : volume <= 0 ? 0 : volume;
            this.mediaElement.Volume = volume;
            this.ToggleVolumeIndicator();

            return volume;
        }

        private void ToggleStationIndicator()
        {
            this.window.StationIndicator.Visibility = Visibility.Visible;
            this.hideStationIndicatorAction?.Dispose();
            this.hideStationIndicatorAction =
                new DelayedAction(token => { this.window.StationIndicator.Visibility = Visibility.Collapsed; }, this.indicatorDisplayTime).Run(CancellationToken.None);
        }

        private void ToggleVolumeIndicator()
        {
            this.window.VolumeIndicator.Visibility = Visibility.Visible;
            this.hideVolumeIndicatorAction?.Dispose();
            this.hideVolumeIndicatorAction =
                new DelayedAction(token => { this.window.VolumeIndicator.Visibility = Visibility.Collapsed; }, this.indicatorDisplayTime).Run(CancellationToken.None);
        }

    }
}