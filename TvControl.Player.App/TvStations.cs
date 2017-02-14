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
            Stream fileStream = this.GetType().Assembly.GetManifestResourceStream(resource);

            MemoryStream stream = new MemoryStream();
            fileStream.CopyTo(stream);
            
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            station.Image = bitmapImage;
        }

        private IEnumerable<TvStation> GetStations()
        {
            yield return new TvStation("1", "ARD", "Das Erste", "Ersten", "Erste", "Erstes");
            yield return new TvStation("26", "ZDF", "Das Zweite", "Zweiten", "Zweite", "Zweites");
            yield return new TvStation("5", "BR", "Bayerischer Rundfunk", "Dritte", "Drittes", "Das Dritte");
            yield return new TvStation("4", "3sat", "drei satt", "3 satt", "dreisatt", "drei sat");
            yield return new TvStation("2", "arte");
            yield return new TvStation("16", "Pro 7", "Pro sieben");
            yield return new TvStation("19", "Sat1", "sat eins", "satt eins", "sateins", "satteins"); 
            yield return new TvStation("24", "VOX", "Fox", "Foxx");
            yield return new TvStation("27", "Kabel 1", "Kabel eins", "kabeleins", "Kabel ein", "kabelein"); 
            yield return new TvStation("35", "ARD alpha", "alpha", "br alpha");
        }

    }
}