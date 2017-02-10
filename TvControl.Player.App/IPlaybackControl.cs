using System.Threading.Tasks;

namespace TvControl.Player.App
{
    public interface IPlaybackControl
    {

        double Volume { get; }

        void SetStation(TvStation tvStation);

        double ChangeVolume(int direction);

    }
}