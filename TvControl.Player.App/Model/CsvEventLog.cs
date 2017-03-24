using System;
using System.IO;

using AutoMapper;

using CsvHelper;
using CsvHelper.Configuration;

using TvControl.Player.App.ViewModels;

namespace TvControl.Player.App.Model
{
    public class TaskResult
    {

        public string Id { get; set; }

        public Modality Modality { get; set; }
        public bool Success { get; set; }

        public TimeSpan CompletionTime { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset FinishedTime { get; set; }

        public string Description { get; set; }

        public string ProbandId { get; set; }

        public Gender ProbandGender { get; set; }

        public string ProbandAge { get; set; }

    }

    public class CsvEventLog : IEventLog
    {

        private readonly CsvConfiguration csvConfiguration = new CsvConfiguration { Delimiter = ";", HasHeaderRecord = true };

        private IMapper Mapper => AutoMapper.Mapper.Instance;

        public void OnComplete(TvControlTaskViewModel currentTask, bool success)
        {
            var task = this.Mapper.Map<TaskResult>(currentTask);
            task.Success = success;
            task.CompletionTime = task.FinishedTime - task.StartTime;

            string filePath = $"{task.ProbandId}.csv";
            bool isNew = !File.Exists(filePath);
            using (var csv = new CsvWriter(new StreamWriter(filePath, true), this.csvConfiguration)) {
                if (isNew) {
                    csv.WriteHeader<TaskResult>();
                }
                csv.WriteRecord(task);
            }
        }

    }
}