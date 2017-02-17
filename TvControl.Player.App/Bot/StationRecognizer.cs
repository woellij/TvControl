using System;
using System.Collections.Generic;
using System.Linq;

using Syn.Bot.Oscova;
using Syn.Bot.Oscova.Collection;
using Syn.Bot.Oscova.Interfaces;

using TvControl.Player.App.Model;

namespace TvControl.Player.App.Bot
{
    internal class StationRecognizer : IEntityRecognizer
    {

        private readonly Func<IEnumerable<TvStation>> getStationsFunc;

        public StationRecognizer(Func<IEnumerable<TvStation>> getStationsFunc)
        {
            this.getStationsFunc = getStationsFunc;
        }

        public string Type { get; } = "station";

        public EntityCollection Parse(Request request)
        {
            IEnumerable<TvStation> stations = this.getStationsFunc();
            var entities = new EntityCollection();

            foreach (TvStation station in stations) {
                var match = station.Names.Select(name =>
                {
                    int i = request.NormalizedText.IndexOf(name, StringComparison.OrdinalIgnoreCase);
                    return i >= 0 ? new { Name = name, Index = i } : null;
                }).FirstOrDefault(arg => arg != null);

                if (match == null) {
                    continue;
                }

                entities.Add(new StationEntity(match.Index, match.Name, station));
                break;
            }
            return entities;
        }

    }
}