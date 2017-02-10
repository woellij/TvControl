using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

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

            this.DataContext = playerWindow.DataContext = new TvControlViewModel(new TvStations(), new MediaElementPlaybackControl(playerWindow));
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
}