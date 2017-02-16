using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

using AutoMapper;

using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

using PropertyChanged;

using ReactiveUI;

namespace TvControl.Player.App
{
    [ImplementPropertyChanged]
    internal class TasksViewModel : ReactiveObject
    {

        private readonly FirebaseClient client;
        private readonly string tasksKey = "tasks";

        public TasksViewModel()
        {
            IFirebaseConfig config = new FirebaseConfig {
                AuthSecret = "prF1qa1F5FdzT8XXdPyGRct5TUzEUUaFjtCiycOW",
                BasePath = "https://tvcontrolapp.firebaseio.com/"
            };
            this.client = new FirebaseClient(config);

            this.Tasks = new ReactiveList<TvControlTaskViewModel> { ChangeTrackingEnabled = true };

            this.Save = ReactiveCommand.CreateFromTask(this.SaveAsync);
            this.Add = ReactiveCommand.Create(this.AddImpl);

            this.InitAsync();
        }

        public ReactiveCommand<Unit, Unit> Add { get; }

        public ReactiveCommand<Unit, Unit> Save { get; }
        public ReactiveList<TvControlTaskViewModel> Tasks { get; }

        private void AddImpl()
        {
            this.Tasks.Add(new TvControlTaskViewModel());
        }

        private async Task SaveAsync()
        {
            List<TvControlTaskViewModel> newOnes = this.Tasks.Where(model => string.IsNullOrEmpty(model.Id)).ToList();
            if (newOnes.Any()) {
                foreach (TvControlTaskViewModel newOne in newOnes) {
                    newOne.Id = Guid.NewGuid().ToString("N");
                    var tvControlTask = Mapper.Map<TvControlTask>(newOne);
                    await this.client.SetAsync(this.GetDbPath(newOne), tvControlTask);
                    newOne.Changed = false;
                }
            }

            foreach (TvControlTaskViewModel deleted in this.Tasks.Where(model => string.IsNullOrWhiteSpace(model.Description))) {
                await this.client.DeleteAsync(this.GetDbPath(deleted));
                this.Tasks.Remove(deleted);
            }

            List<TvControlTaskViewModel> changedOnes = this.Tasks.Where(model => model.Changed).ToList();
            foreach (TvControlTaskViewModel changedOne in changedOnes) {
                var tvControlTask = Mapper.Map<TvControlTask>(changedOne);
                await this.client.UpdateAsync(this.GetDbPath(changedOne), tvControlTask);
                changedOne.Changed = false;
            }
        }

        private string GetDbPath(TvControlTaskViewModel task)
        {
            return $"{this.tasksKey}/{task.Id}";
        }

        private async Task InitAsync()
        {
            try {
                FirebaseResponse response = await this.client.GetAsync(this.tasksKey);
                var tvControlTasks = response.ResultAs<Dictionary<string, TvControlTask>>();
                if (tvControlTasks == null) {
                    return;
                }

                IEnumerable<TvControlTaskViewModel> tvControlViewModels = tvControlTasks.Select(t => Mapper.Map<TvControlTaskViewModel>(t.Value)).ToList();
                this.Tasks.AddRange(tvControlViewModels);
            }
            catch {
            }
        }

    }

    [ImplementPropertyChanged]
    internal class TvControlTask
    {

        public string Id { get; set; }

        public virtual string Description { get; set; }

    }

    [ImplementPropertyChanged]
    class TvControlTaskViewModel : TvControlTask
    {

        private string description;
        private string original;

        public override string Description {
            get { return this.description; }
            set {
                if (this.description == null) {
                    this.original = value;
                }
                else {
                    this.Changed = string.Equals(this.original, value);
                }
                this.description = value;
            }
        }

        public bool Changed { get; set; }

    }
}