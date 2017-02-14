using Syn.Bot.Oscova.Attributes;

namespace TvControl.Player.App.Bot
{
    public enum Direction
    {
        NONE,
        [Synonyms("hoch", "höher", "rauf", "erhöhen", "erhöhe", "weiter")] Up,
        [Synonyms("runter", "niedriger", "reduzieren", "verringern", "verringere", "reduziere")] Down

    }
}