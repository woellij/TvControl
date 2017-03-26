using System;

using Syn.Bot.Oscova;
using Syn.Bot.Oscova.Attributes;
using Syn.Bot.Oscova.Entities;
using Syn.Bot.Oscova.Interfaces;

using TvControl.Player.App.Bot.Entities;
using TvControl.Player.App.Bot.Extensions;
using TvControl.Player.App.ViewModels;

namespace TvControl.Player.App.Bot.Dialogs
{
    public class StationDialog : Dialog
    {

        [Expression("@stationDirection")]
        [Expression("@stationAddress @stationDirection")]
        [Expression("@stationDirection @stationAddress")]
        [Expression("@stationAddress @direction")]
        [Expression("@direction @stationAddress")]
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

        [Expression("Aus")]
        [Expression("Ausschalten")]
        public void TurnOff(Context context, Result result)
        {
            TvControlViewModel.Current.SetCurrentStation("");
        }

        [Expression("Ein")]
        [Expression("An")]
        [Expression("Anschalten")]
        [Expression("Einschalten")]
        public void TurnOn(Context context, Result result)
        {
            TvControlViewModel.Current.SetCurrentStation("1");
        }


        [Expression("Info")]
        [Expression("Infos")]
        [Expression("Informationen")]
        [Expression("Details")]
        public void ToggleStation(Context context, Result result)
        {
            TvControlViewModel.Current.ToggleInfoCommand.Execute();
        }


        [Expression("@station")]
        [Expression("@stationAdress @station")]
        [Expression("@station @stationAdress")]
        [Expression("Schalte zu @stationAdress")]
        [Expression("Wechsle zu @stationAdress")]
        public void SetStation(Context context, Result result)
        {
            var stationEntity = result.Entities.OfType<StationEntity>("station");
            if (stationEntity?.Station == null) {
                return;
            }

            TvControlViewModel.Current.SetCurrentStation(stationEntity.Station);
        }
    }
}