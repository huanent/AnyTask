using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnyTask
{
    public static class DelayExtensions
    {
        public static Job Delay(this Job job, TimeSpan timeSpan)
        {
            job._steps.Enqueue(() =>
            {
                try
                {
#if NET40
                    Task.WaitAll(Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(timeSpan);
                    }, job._cancellationTokenSource.Token));
#else
                    Task.Delay(timeSpan, job._cancellationTokenSource.Token).Wait();
#endif


                }
                catch (Exception)
                {
                }

            });
            return job;
        }
    }
}
