namespace TvControl.Player.App
{
    public interface ITaskLog
    {

        void OnComplete(TvControlTaskViewModel currentTask);

        void OnStart(TvControlTaskViewModel currentTask);

    }
}