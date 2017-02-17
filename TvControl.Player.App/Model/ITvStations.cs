using System.Threading.Tasks;

namespace TvControl.Player.App.Model
{
    public interface ITvStations
    {

        Task<TvStation[]> GetAllAsync();

    }
}