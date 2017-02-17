using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

using PropertyChanged;

namespace TvControl.Player.App.Model
{
    [ImplementPropertyChanged]
    public class TvStation
    {

        public TvStation(string id, string name, params string[] names)
        {
            this.Id = id;
            this.Name = name;
            this.Names = new ReadOnlyCollection<string>(names.Concat(new[] { name }).ToList());
        }

        public string Name { get; }

        public ImageSource Image { get; set; }

        public string Id { get; }
        public Uri FileUrl { get; set; }
        public string FileName { get; set; }
        public IReadOnlyCollection<string> Names { get; }

    }
}