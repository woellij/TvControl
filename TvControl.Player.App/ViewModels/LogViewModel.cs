using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;

using Splat;

using TvControl.Player.App.Model;

namespace TvControl.Player.App.ViewModels
{
    public class LogViewModel : ILogger
    {

        private readonly Dispatcher dispatcher;

        public LogViewModel()
        {
            this.Items = new ObservableCollection<LogItem>();
            this.dispatcher = Dispatcher.CurrentDispatcher;
        }

        public ObservableCollection<LogItem> Items { get; set; }

        public LogLevel Level { get; set; }

        public void Write(string message, LogLevel logLevel)
        {
            this.dispatcher.Invoke(() => this.Items.Add(new LogItem(message, logLevel, DateTimeOffset.UtcNow)));
        }

    }
}