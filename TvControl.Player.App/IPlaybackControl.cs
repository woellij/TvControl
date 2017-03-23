using System.Reactive;
using System.Threading.Tasks;

using TvControl.Player.App.Model;

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

        Task ToggleInfoAsync(TvStation station, bool show);

    }
}