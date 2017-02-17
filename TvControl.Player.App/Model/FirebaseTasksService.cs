using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace TvControl.Player.App.Model
{
    class FirebaseTasksService : ITasksService
    {

        private readonly FirebaseClient client;
        private readonly string tasksKey = "tasks";

        public FirebaseTasksService()
        {
            IFirebaseConfig config = new FirebaseConfig {
                AuthSecret = "prF1qa1F5FdzT8XXdPyGRct5TUzEUUaFjtCiycOW",
                BasePath = "https://tvcontrolapp.firebaseio.com/"
            };
            this.client = new FirebaseClient(config);
        }

        public async Task<TvControlTask[]> GetTasksAsync()
        {
            try {
                FirebaseResponse response = await this.client.GetAsync(this.tasksKey);
                var tvControlTasks = response.ResultAs<Dictionary<string, TvControlTask>>();
                return
                    tvControlTasks.Select(
                        p => Mapper.Map<TvControlTask>(p.Value, options => options.AfterMap((source, destination) => ((TvControlTask) destination).Id = p.Key))).ToArray();
            }
            catch {
                return null;
            }
        }

        public async Task CreateTaskAsync(TvControlTask task)
        {
            await this.client.SetAsync(this.GetDbPath(task), task);
        }

        public Task UpdateTaskAsync(TvControlTask task)
        {
            return this.client.UpdateAsync(this.GetDbPath(task), Mapper.Map<TvControlTask>(task));
        }

        public Task DeleteTaskAsync(string id)
        {
            return this.client.DeleteAsync(this.GetDbPath(id));
        }

        private string GetDbPath(string id)
        {
            return $"{this.tasksKey}/{id}";
        }

        private string GetDbPath(TvControlTask task)
        {
            return this.GetDbPath(task.Id);

            ;
        }

    }
}