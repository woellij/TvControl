using System;
using System.Windows.Media;

using PropertyChanged;

namespace TvControl.Player.App
{
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
}