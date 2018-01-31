using System;
using System.Collections.Generic;
using System.Text;

namespace AnyTask
{
    public static class OnceExtensions
    {
        public static Job Once(this Job job)
        {
            job._steps.Enqueue(() =>
            {
                job._action(job._cancellationTokenSource.Token);
            });
            return job;
        }
    }
}
