using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnyTask
{
    public static class LoopExtensions
    {
        public static Job Loop(this Job job, TimeSpan? interval = null, int? count = null, bool async = false)
        {
            var tasks = new List<Task>();
            job._steps.Enqueue(() =>
            {
                void action() => job._action(job._cancellationTokenSource.Token);
                while ((count > 0 || count == null) && !job._cancellationTokenSource.IsCancellationRequested)
                {
                    if (count.HasValue) count--;

                    if (async)
                    {
                        var task = Task.Factory.StartNew(new Action(action));
                        tasks.Add(task);
                    }
                    else action();
                    if (interval.HasValue)
                    {
                        try
                        {
#if NET40
                            Task.WaitAll(new Task(() =>
                            {
                                Thread.Sleep(interval.Value);
                            }, job._cancellationTokenSource.Token));
#else
                            Task.Delay(interval.Value, job._cancellationTokenSource.Token).Wait();
#endif
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                if (tasks.Any())
                {
                    try
                    {
                        Task.WaitAll(tasks.ToArray(), job._cancellationTokenSource.Token);
                    }
                    catch (Exception)
                    {
                    }
                }
            });

            return job;
        }
    }
}
