using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace TvControl.Player.App.Model
{
    class LocalTasksServiceDecorator : ITasksService
    {

        private readonly ITasksService tasksServiceImplementation;

        public LocalTasksServiceDecorator(ITasksService tasksServiceImplementation)
        {
            this.tasksServiceImplementation = tasksServiceImplementation;
        }

        public async Task CreateTaskAsync(TvControlTask task)
        {
            using (var context = new TvControlContext())
            {
                if (string.IsNullOrWhiteSpace(task.Id)) {
                    task.Id = Guid.NewGuid().ToString("N");
                }
                context.Tasks.Add(Mapper.Map<TvControlTaskDbo>(task));
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            await this.tasksServiceImplementation.CreateTaskAsync(task);
        }

        public async Task UpdateTaskAsync(TvControlTask task)
        {
            using (var context = new TvControlContext())
            {
                context.Entry(Mapper.Map<TvControlTaskDbo>(task)).State = EntityState.Modified;
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            await this.tasksServiceImplementation.UpdateTaskAsync(task).ConfigureAwait(false);
            
        }

        public async Task DeleteTaskAsync(string id)
        {
            using (var context = new TvControlContext())
            {
                context.Tasks.Remove(new TvControlTaskDbo { Id = id });
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            await this.tasksServiceImplementation.DeleteTaskAsync(id).ConfigureAwait(false);
            
        }

        async Task<TvControlTask[]> ITasksService.GetTasksAsync()
        {
            TvControlTask[] tasks = await this.tasksServiceImplementation.GetTasksAsync().ConfigureAwait(false);

            using (var context = new TvControlContext()) {
                if (tasks == null || !tasks.Any()) {
                    List<TvControlTaskDbo> localTasks = await context.Tasks.ToListAsync().ConfigureAwait(false);
                    tasks = Mapper.Map<TvControlTask[]>(localTasks);
                }
                else {
                    // override local tasks
                    string tablename = nameof(context.Tasks);
                    await context.Database.ExecuteSqlCommandAsync($"DELETE FROM [{tablename}]");
                    await context.Tasks.AddRangeAsync(Mapper.Map<TvControlTaskDbo[]>(tasks)).ConfigureAwait(false);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }

            return tasks;
        }

        public class TvControlContext : DbContext
        {

            public TvControlContext()
            {
                this.Database.Migrate();
            }

            public DbSet<TvControlTaskDbo> Tasks { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                string connectionStringBuilder = new
                        SqliteConnectionStringBuilder {
                            DataSource = "tvcontrolapp.db"
                        }
                    .ToString();
                optionsBuilder.UseSqlite(connectionStringBuilder);
            }

        }

        internal class TvControlTaskDbo
        {

            [MaxLength(500)]
            public string Description { get; set; }

            [Key]
            public string Id { get; set; }

            public int Position { get; set; }

        }

    }
}