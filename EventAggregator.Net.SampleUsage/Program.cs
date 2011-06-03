using System;
using EventAggregator.Net.SampleUsage.Samples;

namespace EventAggregator.Net.SampleUsage
{
	class Program
	{
		static void Main(string[] args)
		{

			BasicSample.Run();

			//AsyncSample.Run();



			Console.WriteLine("");
			Console.WriteLine("Press any key...");
			Console.Read();
		}
	}
}
