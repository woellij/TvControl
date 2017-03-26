using Syn.Bot.Oscova.Attributes;

namespace TvControl.Player.App.Bot.Entities
{
    public enum Direction
    {
        NONE,
        [Synonyms("hoch", "höher", "rauf", "erhöhen", "erhöhe", "weiter", "erhoehe", "erhohen", "hoeher")] Up,
        [Synonyms("runter", "niedriger", "reduzieren", "verringern", "verringere", "reduziere")] Down

    }
}