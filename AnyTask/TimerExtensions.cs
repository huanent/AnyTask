using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace AnyTask
{
    public static class TimerExtensions
    {
        public static Job Timer(this Job job,
            int? year = null,
            int? month = null,
            int? day = null,
            int? hour = null,
            int? minute = null,
            int? second = null,
            int? count = null)
        {
            var tokenSource = new CancellationTokenSource();

            job._steps.Enqueue(() =>
            {
                void handler(object s, ElapsedEventArgs e)
                {
                    if (job._cancellationTokenSource.IsCancellationRequested) tokenSource.Cancel();
                    if (count.HasValue && count <= 0) return;
                    if (year.HasValue && year.Value != e.SignalTime.Year) return;
                    if (month.HasValue && month.Value != e.SignalTime.Month) return;
                    if (day.HasValue && day.Value != e.SignalTime.Day) return;
                    if (hour.HasValue && hour.Value != e.SignalTime.Hour) return;
                    if (minute.HasValue && minute.Value != e.SignalTime.Minute) return;
                    if (second.HasValue && second.Value != e.SignalTime.Second) return;
                    if (count.HasValue) count--;
                    job._action(job._cancellationTokenSource.Token);
                    if (count.HasValue && count.Value == 0) tokenSource.Cancel();
                }
                Job._timer.Elapsed += handler;

                try
                {
#if NET40
                    Task.WaitAll(Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(Timeout.Infinite);
                    }, tokenSource.Token));
#else
                    Task.Delay(Timeout.Infinite, tokenSource.Token).Wait();
#endif
                }
                catch (Exception)
                {
                    Job._timer.Elapsed -= handler;
                }
            });



            return job;
        }
    }
}
