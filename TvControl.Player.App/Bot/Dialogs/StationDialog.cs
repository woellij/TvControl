using System;

using Syn.Bot.Oscova;
using Syn.Bot.Oscova.Attributes;
using Syn.Bot.Oscova.Entities;
using Syn.Bot.Oscova.Interfaces;

using TvControl.Player.App.Bot.Extensions;

namespace TvControl.Player.App.Bot.Dialogs
{
    public class StationDialog : Dialog
    {

        [Expression("@stationDirection")]
        [Expression("@station @stationDirection")]
        [Expression("@stationDirection @station")]
        [Expression("@station @direction")]
        [Expression("@direction @station")]
        public void StationChange(Context context, Result result)
        {
            var direction = result.GetDirectionEnum(new[] {
                new Tuple<string, Type>("stationDirection", typeof(StationDirection)),
                new Tuple<string, Type>("direction", typeof(Direction)),
            });
            if (direction == Direction.NONE) {
                return;
            }

            var intDirection = direction == Direction.Down ? -1 : 1;
            TvControlViewModel.Current.ChangeStationCommand.Execute(intDirection);
        }

    }
}