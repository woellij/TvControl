using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using TvControl.Player.App.Common;
using TvControl.Player.App.Model;
using TvControl.Player.App.Windows;

namespace TvControl.Player.App
{
    public class NextList<T>
    {

        private readonly Func<IEnumerator<T>> factory;
        private IEnumerator<T> enumerator;

        public NextList(Func<IEnumerator<T>> enumeratorFactory)
        {
            this.factory = enumeratorFactory;
        }

        private IEnumerator<T> Collection
        {
            get
            {
                if (this.enumerator == null || !this.enumerator.MoveNext())
                {
                    this.enumerator = this.factory();
                    this.enumerator.MoveNext();
                }
                return this.enumerator;
            }
        }

        public T Next()
        {
            T next = this.Collection.Current;
            return next;
        }

    }

    public class ShowTexts : NextList<string>
    {

        public ShowTexts()
            : base(TextGenerator)
        {
        }

        private static IEnumerator<string> TextGenerator()
        {
            yield return
                "Jemand musste Josef K. verleumdet haben, denn ohne dass er etwas B�ses getan h�tte, wurde er eines Morgens verhaftet. �Wie ein Hund!� sagte er, es war, als sollte die Scham ihn �berleben. Als Gregor Samsa eines Morgens aus unruhigen Tr�umen erwachte, fand er sich in seinem Bett zu einem ungeheueren Ungeziefer verwandelt. "
                ;

            yield return
                "�Es ist ein eigent�mlicher Apparat�, sagte der Offizier zu dem Forschungsreisenden und �berblickte mit einem gewisserma�en bewundernden Blick den ihm doch wohlbekannten Apparat. Sie h�tten noch ins Boot springen k�nnen, aber der Reisende hob ein schweres, geknotetes Tau vom Boden, drohte ihnen damit und hielt sie dadurch von dem Sprunge ab."
                ;

            yield return
                "In den letzten Jahrzehnten ist das Interesse an Hungerk�nstlern sehr zur�ckgegangen. Aber sie �berwanden sich, umdr�ngten den K�fig und wollten sich gar nicht fortr�hren.Jemand musste Josef K. verleumdet haben, denn ohne dass er etwas B�ses getan h�tte, wurde er eines Morgens verhaftet."
                ;

            yield return
                "Er h�rte leise Schritte hinter sich. Das bedeutete nichts Gutes. Wer w�rde ihm schon folgen, sp�t in der Nacht und dazu noch in dieser engen Gasse mitten im �bel beleumundeten Hafenviertel? Gerade jetzt, wo er das Ding seines Lebens gedreht hatte und mit der Beute verschwinden wollte!"
                ;
        }

    }

    public class MediaElementPlaybackControl : IPlaybackControl
    {

        private readonly TimeSpan indicatorDisplayTime = TimeSpan.FromSeconds(3);

        private readonly MediaElement mediaElement;

        readonly Random random = new Random();
        private readonly DateTimeOffset startTime;

        private readonly PlayerWindow window;
        private DelayedAction hideStationIndicatorAction;
        private IDisposable hideVolumeIndicatorAction;
        private readonly ShowTexts texts = new ShowTexts();

        public MediaElementPlaybackControl(PlayerWindow window)
        {
            this.window = window;
            this.mediaElement = window.MediaElement;
            this.startTime = DateTimeOffset.UtcNow;


            this.window.ShowProgressBar.Minimum = 0;
            this.window.ShowProgressBar.Maximum = 100D;

            this.mediaElement.MediaOpened += MediaElement_MediaOpened;
        }

        TimeSpan duration;
        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            this.UpdateProgress();
        }

        public double Volume => this.mediaElement.Volume;

        public bool IsDisplayingMessage => this.window.MessageBox.IsVisible;

        public void SetStation(TvStation tvStation)
        {
            if (tvStation?.FileUrl == null)
            {
                try
                {
                    if (this.mediaElement.Source != null)
                    {
                        this.mediaElement.Stop();
                    }
                }
                catch
                {
                }
                this.mediaElement.Source = null;
            }
            else
            {
                this.mediaElement.Source = tvStation.FileUrl;
                this.UpdateProgress();
            }
            if (tvStation != null)
            {
                this.ToggleStationIndicator();
            }
            else
            {
                this.hideStationIndicatorAction?.ExecuteAsync()?.ContinueWith(t => t?.Result?.Dispose());
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

        public void DisplayMessage(string message)
        {
            this.window.MessageTextBlock.Text = message;
            this.window.MessageBox.Visibility = Visibility.Visible;
        }

        public void HideMessage()
        {
            this.window.MessageBox.Visibility = Visibility.Collapsed;
        }

        public async Task ToggleInfoAsync(TvStation station, bool show)
        {
            this.hideStationIndicatorAction?.Dispose();
            if (!show)
            {
                this.window.StationIndicator.Visibility = Visibility.Collapsed;
            }
            else
            {
                TimeSpan showTime = TimeSpan.FromSeconds(20);
                await this.ToggleStationInfo(showTime);
            }
        }

        private void ToggleStationIndicator()
        {
            TimeSpan displayTime = this.indicatorDisplayTime;
            this.ToggleStationInfo(displayTime);
        }

        private Task ToggleStationInfo(TimeSpan displayTime)
        {
            this.UpdateProgress();

            this.window.ShowText.Text = this.texts.Next();

            this.window.StationIndicator.Visibility = Visibility.Visible;
            this.hideStationIndicatorAction?.Dispose();

            Task task;
            this.hideStationIndicatorAction =
                new DelayedAction(token => { this.window.StationIndicator.Visibility = Visibility.Collapsed; }, displayTime).Run(CancellationToken.None, out task);
            return task;
        }

        private void UpdateProgress()
        {

            try
            {
                var duration = this.mediaElement.NaturalDuration.HasTimeSpan ? this.mediaElement.NaturalDuration.TimeSpan : TimeSpan.FromMinutes(60);

                var position = DateTimeOffset.UtcNow - this.startTime;
                if (position > duration)
                {
                    position = TimeSpan.FromSeconds(position.TotalSeconds % duration.TotalSeconds);
                }
                this.mediaElement.Position = position;
                var value = position.TotalSeconds / duration.TotalSeconds;
                this.window.ShowProgressBar.Value = value * 100;

            }
            catch { }
        }

        private void ToggleVolumeIndicator()
        {
            this.hideStationIndicatorAction?.ExecuteAsync()?.ContinueWith(t => t?.Result?.Dispose());
            this.window.VolumeIndicator.Visibility = Visibility.Visible;
            this.hideVolumeIndicatorAction?.Dispose();
            this.hideVolumeIndicatorAction =
                new DelayedAction(token => { this.window.VolumeIndicator.Visibility = Visibility.Collapsed; }, this.indicatorDisplayTime).Run(CancellationToken.None);
        }

    }
}