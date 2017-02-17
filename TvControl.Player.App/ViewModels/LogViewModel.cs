using System;
using System.Collections.ObjectModel;

using Splat;

using TvControl.Player.App.Model;

namespace TvControl.Player.App.ViewModels
{
    public class LogViewModel : ILogger
    {

        public LogViewModel()
        {
            this.Items = new ObservableCollection<LogItem>();
        }

        public ObservableCollection<LogItem> Items { get; set; }

        public LogLevel Level { get; set; }

        public void Write(string message, LogLevel logLevel)
        {
            this.Items.Add(new LogItem(message, logLevel, DateTimeOffset.UtcNow));
        }

    }
}