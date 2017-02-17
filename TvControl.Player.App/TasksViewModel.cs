using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;

using AutoMapper;

using PropertyChanged;

using ReactiveUI;

using TvControl.Player.App.Model;

namespace TvControl.Player.App
{
    [ImplementPropertyChanged]
    internal class TasksViewModel : ViewModelBase
    {

        private readonly IPlaybackControl playbackControl;
        private readonly ITaskLog taskLog;

        private readonly ITasksService tasksService;

        private int currentIndex;

        public TasksViewModel(ITasksService tasksService, IPlaybackControl playbackControl, ITaskLog taskLog)
        {
            this.tasksService = tasksService;
            this.playbackControl = playbackControl;
            this.taskLog = taskLog;

            this.Tasks = new ReactiveList<TvControlTaskViewModel> { ChangeTrackingEnabled = true };

            this.Save = ReactiveCommand.CreateFromTask(this.SaveAsync);
            this.Add = ReactiveCommand.Create(this.AddImpl);
            this.StartStop = ReactiveCommand.Create(this.StartStopImpl);

            this.InitAsync();
        }

        public TvControlTaskViewModel CurrentTask { get; private set; }

        public ReactiveCommand<Unit, Unit> Add { get; }

        public ICommand StartStop { get; }

        public ReactiveCommand<Unit, Unit> Save { get; }

        public ReactiveList<TvControlTaskViewModel> Tasks { get; }

        private void StartStopImpl()
        {
            if (this.playbackControl.IsDisplayingMessage) {
                this.playbackControl.HideMessage();
                this.taskLog.OnStart(this.CurrentTask);
            }
            else {
                if (this.currentIndex >= this.Tasks.Count) {
                    // finished
                    this.playbackControl.DisplayMessage("Vielen Dank. Das waren soweit alle Aufgaben");
                    // reset to be able to restart
                    this.currentIndex = 0;
                    this.CurrentTask = null;
                }
                else {
                    this.CurrentTask = this.Tasks[this.currentIndex];
                    this.playbackControl.DisplayMessage(this.CurrentTask.Description);
                    this.currentIndex++;
                }

                this.taskLog.OnComplete(this.CurrentTask);
            }
        }

        private void AddImpl()
        {
            this.Tasks.Add(new TvControlTaskViewModel());
        }

        private async Task SaveAsync()
        {
            List<IndexedItem<TvControlTaskViewModel>> indexed = this.Tasks.Select(IndexedItem<TvControlTaskViewModel>.Create).ToList();

            foreach (IndexedItem<TvControlTaskViewModel> newOne in indexed.Where(i => string.IsNullOrWhiteSpace(i.Object.Id)).ToList()) {
                newOne.Object.Id = Guid.NewGuid().ToString("N");
                await this.tasksService.CreateTaskAsync(newOne.To<TvControlTask>());
                indexed.Remove(newOne);
            }

            foreach (IndexedItem<TvControlTaskViewModel> deleted in indexed.Where(ii => string.IsNullOrWhiteSpace(ii.Object.Description)).ToList()) {
                await this.tasksService.DeleteTaskAsync(deleted.Object.Id);
                this.Tasks.Remove(deleted.Object);
                indexed.Remove(deleted);
            }

            foreach (IndexedItem<TvControlTaskViewModel> changedOne in indexed) {
                await this.tasksService.UpdateTaskAsync(changedOne.To<TvControlTask>());
            }
        }

        private async Task InitAsync()
        {
            IEnumerable<TvControlTask> tasks = await this.tasksService.GetTasksAsync();
            if (tasks == null) {
                return;
            }

            tasks = tasks.OrderBy(task => task.Position);
            IEnumerable<TvControlTaskViewModel> tvControlViewModels = tasks?.Select(Mapper.Map<TvControlTaskViewModel>).ToList();
            this.Tasks.AddRange(tvControlViewModels);
        }

    }
}