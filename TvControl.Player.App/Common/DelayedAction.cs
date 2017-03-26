using System;
using System.Threading;
using System.Threading.Tasks;

namespace TvControl.Player.App.Common
{
    public class DelayedAction : IDisposable
    {

        private readonly Action<CancellationToken> action;
        private readonly TimeSpan delay;
        private readonly Func<CancellationToken, Task> taskaction;
        private CancellationTokenSource cts;

        public DelayedAction(Action<CancellationToken> action, TimeSpan delay)
        {
            this.action = action;
            this.delay = delay;
        }

        public DelayedAction(Func<CancellationToken, Task> action, TimeSpan delay)
        {
            this.taskaction = action;
            this.delay = delay;
        }

        public void Dispose()
        {
            this.cts.Cancel();
        }

        public DelayedAction Run(CancellationToken cancellationToken)
        {
            this.cts = new CancellationTokenSource();
            this.Schedule(CancellationTokenSource.CreateLinkedTokenSource(this.cts.Token, cancellationToken).Token);
            return this;
        }

        private TaskCompletionSource<bool> tcs;
        private CancellationToken cancellation;

        public Task Task => this.tcs.Task;
        public bool IsScheduled => this.tcs != null && !this.tcs.Task.IsCompleted && !this.tcs.Task.IsCanceled && !this.Task.IsFaulted;

        private async void Schedule(CancellationToken cancellation)
        {
            this.tcs = new TaskCompletionSource<bool>();
            this.cancellation = cancellation;
            try
            {
                await Task.Delay(this.delay, cancellation);
                await this.ExecuteAsync();
            }
            catch (OperationCanceledException)
            {
                this.tcs.TrySetCanceled();
            }
        }

        public async Task<IDisposable> ExecuteAsync()
        {
            if (cancellation.IsCancellationRequested)
            {
                this.tcs.TrySetCanceled();
                return this;
            }

            Func<CancellationToken, Task> func = this.taskaction;

            try
            {
                if (func != null)
                {
                    await func(cancellation).ConfigureAwait(false);
                }
                else
                {
                    Action<CancellationToken> action = this.action;
                    action?.Invoke(cancellation);
                    this.tcs.TrySetResult(true);
                }
            }
            catch (Exception e)
            {
                this.tcs.TrySetException(e);
            }
            return this;
        }

        public DelayedAction Run(CancellationToken cancellation, out Task task)
        {
            var disposable = this.Run(cancellation);
            task = this.Task;
            return disposable;
        }
    }
}