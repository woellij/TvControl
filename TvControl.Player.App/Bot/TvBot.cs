using Syn.Bot.Oscova;

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

            bot.CreateRecognizer("station", new[] { "Sender", "Kanal", "Fernsehsender", "Fernsehkanal", "TV-Sender", "Programm" });

            bot.Dialogs.Add(new StationDialog());
            bot.Dialogs.Add(new VolumeDialog());

            bot.Trainer.StartTraining();
            return bot;
        }

    }
}