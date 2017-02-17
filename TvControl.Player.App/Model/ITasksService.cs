using System.Collections.Generic;
using System.Threading.Tasks;


namespace TvControl.Player.App.Model
{
    public interface ITasksService
    {

        Task<TvControlTask[]> GetTasksAsync();

        Task CreateTaskAsync(TvControlTask task);

        Task UpdateTaskAsync(TvControlTask task);

        Task DeleteTaskAsync(string id);

    }
}