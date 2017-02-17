using System.Threading.Tasks;

namespace TvControl.Player.App
{
    public interface IPlaybackControl
    {

        double Volume { get; }

        bool IsDisplayingMessage { get; }

        void SetStation(TvStation tvStation);

        double ChangeVolume(int direction);

        void DisplayMessage(string message);

        void HideMessage();

    }
}