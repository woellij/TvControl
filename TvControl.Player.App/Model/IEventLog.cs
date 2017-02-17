using TvControl.Player.App.ViewModels;

namespace TvControl.Player.App.Model
{
    public interface IEventLog
    {

        void OnComplete(TvControlTaskViewModel currentTask, bool success);

    }
}