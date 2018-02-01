using AnyTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var job = new Job(t =>
             {
                 Console.WriteLine("任务执行");
             })
            .Delay(new TimeSpan(0, 0, 2))
            .Once()
            .Start();

            job.OnStart += () =>
            {
                Console.WriteLine("任务开始执行");
            };

            job.OnStop += () =>
            {
                Console.WriteLine("任务执行结束");
            };

            Console.ReadKey();

        }
    }
}
