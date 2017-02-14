using System;

using Nancy;

using TvControl.Player.App.Bot;

namespace TvControl.Player.App.Api
{
    public class TextControlModule : NancyModule
    {

        public TextControlModule() : base("textinput")
        {
            this.Post["/{text}"] = OnPostText;
        }

        private dynamic OnPostText(dynamic o)
        {
            var bot = TvBot.Instance;
            var result = bot.Evaluate(o.text);
            result.Invoke();
            return result;
        }

    }
}