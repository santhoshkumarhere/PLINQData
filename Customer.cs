using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PLINQData
{
    public class Customer
    {
        ParallelOptions options = new ParallelOptions()
        {
            MaxDegreeOfParallelism = 2
        };
        public async Task TestParallel()
        {
            Console.WriteLine(Environment.ProcessorCount);
            DateTime StartDateTime = DateTime.Now;
            Console.WriteLine(@"Parallel foreach method start at : {0}", StartDateTime);

            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 2
            };

            List<int> integerList = Enumerable.Range(1, 50).ToList();
            Parallel.ForEach(integerList, options, i =>
            {
                long total = Compute(i);
                Console.WriteLine(@"value of i = {0} - {1}, thread = {2}", i, total, Thread.CurrentThread.ManagedThreadId);

            });

            DateTime EndDateTime = DateTime.Now;
            Console.WriteLine(@"Parallel foreach method end at : {0}", EndDateTime);
            TimeSpan span = EndDateTime - StartDateTime;
            int ms = (int)span.TotalMilliseconds;

            Console.WriteLine(@"Time Taken by Parallel foreach method in miliseconds {0}", ms);
        }

        public async Task TestParallelAsync()
        {
            DateTime StartDateTime = DateTime.Now;
            Console.WriteLine(@"Parallel foreach method start at : {0}", StartDateTime);

           
            
            List<int> integerList = Enumerable.Range(1, 50).ToList();
            Parallel.ForEach(integerList, options, async i =>
            {
                var total =await Calculate(i);
                Console.WriteLine(@"value of i = {0} - {1}, thread = {2}", i,  total, Thread.CurrentThread.ManagedThreadId);

            });

            DateTime EndDateTime = DateTime.Now;
            Console.WriteLine(@"Parallel foreach method end at : {0}", EndDateTime);
            TimeSpan span = EndDateTime - StartDateTime;
            int ms = (int)span.TotalMilliseconds;

            Console.WriteLine(@"Time Taken by Parallel foreach method in miliseconds {0}", ms);
        }


        public async Task Process()
        {
            List<int> integerList = Enumerable.Range(1, 50).ToList();

            DateTime StartDateTime = DateTime.Now;
            Console.WriteLine(@"Method start at : {0}", StartDateTime);
            IEnumerable<Task<long>> CalculateTasksQuery = from item in integerList select Calculate(item);
            List<Task<long>> calcualteTasks = CalculateTasksQuery.ToList();
            Console.WriteLine("While Starts...");

            while (calcualteTasks.Count > 0)
            {
                Task<long> firstFinishedTask = await Task.WhenAny(calcualteTasks);

                calcualteTasks.Remove(firstFinishedTask);

                long total = await firstFinishedTask;
                Console.WriteLine(@"value of total = {0}, thread = {1}", total, Thread.CurrentThread.ManagedThreadId);
            }
            DateTime EndDateTime = DateTime.Now;

            TimeSpan span = EndDateTime - StartDateTime;
            int ms = (int)span.TotalMilliseconds;

            Console.WriteLine(@"Time Taken by Parallel foreach method in miliseconds {0}", ms);

        }

        public async Task ProcessNormalAsync()
        {
            Console.WriteLine();
            DateTime StartDateTime = DateTime.Now;
            Console.WriteLine(@"Method start at : {0}", StartDateTime);

            var semaphore = new SemaphoreSlim(5);
            var tasks = new List<Task<long>>();
            List<int> integerList = Enumerable.Range(1, 50).ToList();
            foreach (var item in integerList)
            {
                await semaphore.WaitAsync();

                var task = this.Calculate(item)
                    .ContinueWith(t =>
                    {
                        semaphore.Release();
                        if (t.Exception != null)
                        {
                            throw t.Exception;
                        }

                        return t.Result;
                    });
                tasks.Add(task);
            }

            var all = await Task.WhenAll(tasks); 
            
            foreach(var item in all)
            {
                 
                Console.WriteLine(item);
            }
        
        
            DateTime EndDateTime = DateTime.Now;

            TimeSpan span = EndDateTime - StartDateTime;
            int ms = (int)span.TotalMilliseconds;

            Console.WriteLine(@"Time Taken by Semaphore foreach method in miliseconds {0}", ms);
        }

        public async Task<long> Calculate(int customerId)
        {
            long total = 0;
            for (int i = 1; i < 100000000; i++)
            {
                total += i;
            }
            return await Task.FromResult(total);
        }

        public long Compute(int customerId)
        {
            long total = 0;
            for (int i = 1; i < 100000000; i++)
            {
                total += i;
            }
            return total;
        }
    }
}
