using Syn.Bot.Oscova;
using Syn.Bot.Oscova.Entities;

using TvControl.Player.App.Bot.Dialogs;

namespace TvControl.Player.App.Bot
{
    public class TvBot
    {

        private static OscovaBot _instance;

        public static OscovaBot Instance => _instance ?? (_instance = InitBot());

        private static OscovaBot InitBot()
        {
            var bot = new OscovaBot();

            bot.CreateRecognizer<VolumeDirection>("volumeDirection");
            bot.CreateRecognizer<Direction>("direction");
            bot.CreateRecognizer<StationDirection>("stationDirection");
            bot.CreateRecognizer("volumeAddress", new[] { "Lautstärke", "Pegel", "Volume" });
            bot.CreateRecognizer("stationAddress", new[] { "Sender", "Kanal", "Fernsehsender", "Fernsehkanal", "TV-Sender", "Programm" });
            bot.Recognizers.Add(new StationRecognizer(() => TvControlViewModel.Current.TvStations));
            
            bot.Dialogs.Add(new StationDialog());
            bot.Dialogs.Add(new VolumeDialog());

            bot.Trainer.StartTraining();
            return bot;
        }

    }

    internal class StationEntity : Entity
    {

        public StationEntity(int index, string name, TvStation station)
            : base("station")
        {
            this.Index = index;
            this.Value = name;
            this.Station = station;
        }

        public TvStation Station { get; }

    }
}