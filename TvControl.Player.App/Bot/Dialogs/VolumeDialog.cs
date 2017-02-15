using Syn.Bot.Oscova;
using Syn.Bot.Oscova.Attributes;
using Syn.Bot.Oscova.Entities;
using Syn.Bot.Oscova.Interfaces;

namespace TvControl.Player.App.Bot.Dialogs
{
    public class VolumeDialog : Dialog
    {

        [Expression("@volumeDirection")]
        [Expression("@volumeAddress @direction")]
        [Expression("@direction @volumeAdress")]
        public void VolumeChange(Context context, Result result)
        {
            IEntity appEntity = result.Entities.OfType("app");
            var dir = 0;

            var entity = result.Entities.OfType<Entity>("volumeDirection");
            if (entity != null) {
                dir = entity.ValueAs<VolumeDirection>() == VolumeDirection.Down ? -1 : 1;
            }
            else {
                entity = result.Entities.OfType<Entity>("direction");
                if (entity != null) {
                    dir = entity.ValueAs<Direction>() == Direction.Down ? -1 : 1;
                }
            }

            if (dir != 0) {
                TvControlViewModel.Current.ChangeVolumeCommand.Execute(dir);
            }
        }

    }
}