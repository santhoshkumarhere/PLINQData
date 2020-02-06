using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PLINQData
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var customer = new Customer();

            await customer.TestParallel();
            await customer.TestParallelAsync();
            await customer.Process();
            await customer.ProcessNormalAsync();

            Console.WriteLine("Press any key to exist");
            Console.ReadLine();
        } 

    }
}
