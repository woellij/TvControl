using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TvControl.Player.App
{
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
}