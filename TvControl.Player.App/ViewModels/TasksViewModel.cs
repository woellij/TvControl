using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using AutoMapper;

using PropertyChanged;

using ReactiveUI;

using Splat;

using TvControl.Player.App.Model;

namespace TvControl.Player.App.ViewModels
{
    internal class Selection<TItem>
    {

        private Selection(ICollection<TItem> list)
        {
            this.Items = list;
        }

        public ICollection<TItem> Items { get; set; }
        public TItem SelectedItem { get; set; }

        public static Selection<TItem> FromEnum()
        {
            Array values = Enum.GetValues(typeof(TItem));
            var list = new List<TItem>();
            foreach (object value in values)
            {
                if (value is TItem)
                {
                    list.Add((TItem)value);
                }
            }

            return new Selection<TItem>(list);
        }

    }

    [ImplementPropertyChanged]
    internal class TasksViewModel : ViewModelBase
    {

        private readonly IEventLog eventLog;

        private readonly ILogger logger;

        private readonly IPlaybackControl playbackControl;

        private readonly ITasksService tasksService;

        private int currentIndex;

        public TasksViewModel(ITasksService tasksService, IPlaybackControl playbackControl, IEventLog eventLog, ILogger logger)
        {
            this.tasksService = tasksService;
            this.playbackControl = playbackControl;
            this.eventLog = eventLog;
            this.logger = logger;

            this.ModalitiesSelection = Selection<Modality>.FromEnum();
            this.Genders = Selection<Gender>.FromEnum();
            this.Proband = new Proband();

            this.Tasks = new ReactiveList<TvControlTaskViewModel> { ChangeTrackingEnabled = true };

            this.Reset = ReactiveCommand.Create(ResetImplementation);
            this.Save = ReactiveCommand.CreateFromTask(this.SaveAsync);
            this.Add = ReactiveCommand.Create(this.AddImpl);
            this.StartStop = ReactiveCommand.Create(this.StartStopImpl);
            this.SetFinished = ReactiveCommand.Create<bool, Unit>(this.SetFinishedImpl, this.WhenAnyValue(model => model.CurrentTask).Select(model => model != null));

            this.InitAsync();
        }

        public Selection<Gender> Genders { get; set; }

        public ReactiveCommand Reset { get; set; }

        public Selection<Modality> ModalitiesSelection { get; }

        public ReactiveCommand<bool, Unit> SetFinished { get; }

        public TvControlTaskViewModel CurrentTask { get; private set; }

        public ReactiveCommand<Unit, Unit> Add { get; }

        public ICommand StartStop { get; }

        public ReactiveCommand<Unit, Unit> Save { get; }

        public ReactiveList<TvControlTaskViewModel> Tasks { get; }

        public Proband Proband { get; set; }

        private void ResetImplementation()
        {
            this.currentIndex = -1;
            foreach (var task in this.Tasks)
            {
                task.FinishedTime = default(DateTimeOffset);
            }
        }

        private Unit SetFinishedImpl(bool success)
        {
            this.SetFinishedTime();

            this.eventLog.OnComplete(this.CurrentTask, success);

            this.CurrentTask = null;
            return Unit.Default;
        }

        private void StartStopImpl()
        {
            if (this.currentIndex < 0)
            {
                // reset over command
                this.currentIndex = 0;
                this.playbackControl.HideMessage();
                return;
            }
            if (this.playbackControl.IsDisplayingMessage)
            {
                // hide message
                this.playbackControl.HideMessage();
                if (this.currentIndex < this.Tasks.Count)
                {
                    // when still in index range -> new task started
                    this.CurrentTask = this.Tasks[this.currentIndex];
                    this.currentIndex++;
                    this.CurrentTask.Proband = this.Proband;
                    this.CurrentTask.Modality = this.ModalitiesSelection.SelectedItem;
                    this.CurrentTask.StartTime = DateTimeOffset.UtcNow;
                }
            }
            else
            {
                if (this.SetFinishedTime())
                {
                    return;
                }
                if (this.currentIndex >= this.Tasks.Count)
                {
                    // finished
                    this.playbackControl.DisplayMessage("Vielen Dank. Das waren soweit alle Aufgaben");
                    // reset to be able to restart
                    this.ResetImplementation();
                }
                else
                {
                    if (this.CurrentTask != null)
                    {
                        this.logger.Write("Markiere zuerst den aktuellen Task als erfolgreich oder nicht erfolgreich abgeschlossen", LogLevel.Error);
                        return;
                    }

                    ShowTaskInstructions();
                }
            }
        }

        private void ShowTaskInstructions()
        {
            TvControlTaskViewModel task = this.Tasks[this.currentIndex];
            this.playbackControl.DisplayMessage(task.Description);
        }

        private bool SetFinishedTime()
        {
            if (this.CurrentTask != null && this.CurrentTask.FinishedTime == default(DateTimeOffset))
            {
                this.CurrentTask.FinishedTime = DateTimeOffset.UtcNow;
                return true;
            }
            return false;
        }

        private void AddImpl()
        {
            this.Tasks.Add(new TvControlTaskViewModel());
        }

        private async Task SaveAsync()
        {
            List<IndexedItem<TvControlTaskViewModel>> indexed = this.Tasks.Select(IndexedItem<TvControlTaskViewModel>.Create).ToList();

            foreach (IndexedItem<TvControlTaskViewModel> newOne in indexed.Where(i => string.IsNullOrWhiteSpace(i.Object.Id)).ToList())
            {
                newOne.Object.Id = Guid.NewGuid().ToString("N");
                await this.tasksService.CreateTaskAsync(newOne.To<TvControlTask>());
                indexed.Remove(newOne);
            }

            foreach (IndexedItem<TvControlTaskViewModel> deleted in indexed.Where(ii => string.IsNullOrWhiteSpace(ii.Object.Description)).ToList())
            {
                await this.tasksService.DeleteTaskAsync(deleted.Object.Id);
                this.Tasks.Remove(deleted.Object);
                indexed.Remove(deleted);
            }

            foreach (IndexedItem<TvControlTaskViewModel> changedOne in indexed)
            {
                await this.tasksService.UpdateTaskAsync(changedOne.To<TvControlTask>());
            }
        }

        private async Task InitAsync()
        {
            IEnumerable<TvControlTask> tasks = await this.tasksService.GetTasksAsync();
            if (tasks == null)
            {
                return;
            }

            tasks = tasks.OrderBy(task => task.Position);
            IEnumerable<TvControlTaskViewModel> tvControlViewModels = tasks?.Select(Mapper.Map<TvControlTaskViewModel>).ToList();
            this.Tasks.AddRange(tvControlViewModels);
        }

    }

    public class Proband
    {

        public string Id { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }

    }

    public enum Gender
    {

        Male,
        Female

    }
}