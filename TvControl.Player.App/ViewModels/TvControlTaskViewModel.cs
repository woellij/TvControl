using System;

using PropertyChanged;

namespace TvControl.Player.App.ViewModels
{
    [ImplementPropertyChanged]
    public class TvControlTaskViewModel
    {

        private static readonly string Format = "hh:mm:ss";

        public string Id { get; set; }

        public string Description { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset FinishedTime { get; set; }

        public string StartTimeString => this.StartTime.ToString(Format);

        public string FinishedTimeString => this.FinishedTime == default(DateTimeOffset) ? "" : this.FinishedTime.ToString(Format);

    }
}