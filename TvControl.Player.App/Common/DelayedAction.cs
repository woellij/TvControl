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

        public IDisposable Run(CancellationToken cancellationToken)
        {
            this.cts = new CancellationTokenSource();
            this.Schedule(CancellationTokenSource.CreateLinkedTokenSource(this.cts.Token, cancellationToken).Token);
            return this;
        }

        private TaskCompletionSource<bool> tcs;

        public Task Task => this.tcs.Task;

        private async void Schedule(CancellationToken cancellation)
        {
            this.tcs = new TaskCompletionSource<bool>();
            try {
                await Task.Delay(this.delay, cancellation);
                if (cancellation.IsCancellationRequested) {
                    this.tcs.TrySetCanceled();
                    return;
                }

                Func<CancellationToken, Task> func = this.taskaction;
                if (func != null) {
                    await func(cancellation).ConfigureAwait(false);
                }
                else {
                    Action<CancellationToken> action = this.action;
                    action?.Invoke(cancellation);
                }
                this.tcs.TrySetResult(true);
            }
            catch (OperationCanceledException) {
                this.tcs.TrySetCanceled();
            }
        }

        public IDisposable Run(CancellationToken cancellation, out Task task)
        {
            IDisposable disposable = this.Run(cancellation);
            task = this.Task;
            return disposable;
        }
    }
}