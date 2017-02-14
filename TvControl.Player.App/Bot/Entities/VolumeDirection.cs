using Syn.Bot.Oscova.Attributes;

namespace TvControl.Player.App.Bot
{
    public enum VolumeDirection
    {
        NONE,
        [Synonyms("lauter")] Up,
        [Synonyms("leiser")] Down

    }
}