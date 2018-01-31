using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;

namespace AnyTask
{
    public class Job
    {
        #region fields
        internal readonly ConcurrentQueue<Action> _steps = new ConcurrentQueue<Action>();

        internal readonly Action<CancellationToken> _action;

        internal readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        internal static System.Timers.Timer _timer = new System.Timers.Timer { AutoReset = true, Interval = 1000 };
        #endregion

        #region events
        public event Action OnStart;

        public event Action OnStop;
        #endregion

        #region ctor
        public Job(Action<CancellationToken> action) => _action = action;
        #endregion

        #region methods
        public Job Start()
        {
            _timer.Start();
            var task = RunSteps();
            task.Start();
            return this;
        }

        private Task RunSteps()
        {
            return new Task(() =>
            {
                OnStart?.Invoke();
                while (_steps.Count > 0)
                {
                    _steps.TryDequeue(out var action);
                    action();
                }
                OnStop?.Invoke();
            }, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            _timer.Stop();
            _cancellationTokenSource.Cancel();
        }
        #endregion
    }
}
