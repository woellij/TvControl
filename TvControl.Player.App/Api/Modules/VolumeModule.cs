using Nancy;

namespace TvControl.Player.App.Api.Modules
{
    public class VolumeModule : NancyModule
    {

        public VolumeModule() : base("volume")
        {
            this.Post["volumeUp", "/up"] = o => TvControlViewModel.Current.ChangeVolumeCommand.Execute(+1);
            this.Post["volumeUp", "/down"] = o => TvControlViewModel.Current.ChangeVolumeCommand.Execute(-1);
        }

    }
}