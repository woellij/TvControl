using Syn.Bot.Oscova.Attributes;

namespace TvControl.Player.App.Bot.Entities
{
    public enum StationDirection
    {
        NONE,
        [Synonyms("nächster", "nächsten", "nächste", "naechste", "naechsten", "naechste")]
        Up,
        [Synonyms("vorheriger", "vorherigen", "vorherige", "zurück", "zurueck")]
        Down

    }
}