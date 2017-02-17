using System;

using Splat;

namespace TvControl.Player.App.Model
{
    public class LogItem
    {

        public LogItem(string message, LogLevel logLevel, DateTimeOffset time)
        {
            this.Message = message;
            this.Level = logLevel;
            this.Time = time;
        }

        public LogLevel Level { get; }
        public string Message { get; }
        public DateTimeOffset Time { get; }

        public override string ToString()
        {
            return $"{this.Level}\t{this.Time:G}\t {this.Message}";
        }

    }
}