using Syn.Bot.Oscova.Attributes;

namespace TvControl.Player.App.Bot
{
    public enum StationDirection
    {
        NONE,
        [Synonyms("nächster", "nächsten", "nächste")]
        Up,
        [Synonyms("vorheriger", "vorherigen", "vorherige", "zurück")]
        Down

    }
}