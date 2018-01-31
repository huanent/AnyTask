using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                        var task = Task.Run(new Action(action));
                        tasks.Add(task);
                    }
                    else action();
                    if (interval.HasValue)
                    {
                        try
                        {
                            Task.Delay(interval.Value, job._cancellationTokenSource.Token).Wait();
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
                        Task.WhenAll(tasks).Wait(job._cancellationTokenSource.Token);
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
