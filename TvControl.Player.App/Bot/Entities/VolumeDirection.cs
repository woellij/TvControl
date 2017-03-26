using Syn.Bot.Oscova.Attributes;

namespace TvControl.Player.App.Bot.Entities
{
    public enum VolumeDirection
    {
        NONE,
        [Synonyms("lauter", "lauda", "lauder")] Up,
        [Synonyms("leiser", "liza", "leise")] Down

    }
}