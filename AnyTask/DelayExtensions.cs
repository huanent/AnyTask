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
                    Task.Delay(timeSpan, job._cancellationTokenSource.Token).Wait();
                }
                catch (Exception)
                {
                }
                
            });
            return job;
        }
    }
}
