using System.Threading.Tasks;

namespace TvControl.Player.App
{
    public interface ITvStations
    {

        Task<TvStation[]> GetAllAsync();

    }
}