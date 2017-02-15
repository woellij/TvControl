using Nancy;

using Syn.Bot.Oscova;

using TvControl.Player.App.Bot;

namespace TvControl.Player.App.Api.Modules
{
    public class TextControlModule : NancyModule
    {

        public TextControlModule()
            : base("textinput")
        {
            this.Post["/{text}"] = this.OnPostText;
        }

        private dynamic OnPostText(dynamic o)
        {
            OscovaBot bot = TvBot.Instance;
            dynamic result = bot.Evaluate(o.text);
            result.Invoke();
            return result;
        }

    }
}