using System;

using TvControl.Player.App.ViewModels;

namespace TvControl.Player.App.Model
{
    public class CsvEventLog : IEventLog
    {

        public void OnComplete(TvControlTaskViewModel currentTask, bool success)
        {
        }

        public void OnStart(TvControlTaskViewModel currentTask)
        {
        }

    }
}