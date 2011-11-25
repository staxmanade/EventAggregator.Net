using System;
using EventAggregatorNet.SampleUsage.Samples;

namespace EventAggregatorNet.SampleUsage
{
    class Program
    {
        static void Main(string[] args)
        {

            BasicSample.Run();

            AsyncSample.Run();



            Console.WriteLine("");
            Console.WriteLine("Press any key...");
            Console.Read();
        }
    }
}
