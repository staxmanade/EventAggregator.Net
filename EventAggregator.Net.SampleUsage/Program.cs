using System;
using EventAggregator.Net.SampleUsage.Samples.AsyncronousMessages;

namespace EventAggregator.Net.SampleUsage
{
	class Program
	{
		static void Main(string[] args)
		{

			AsyncSample.Run();




			Console.WriteLine("");
			Console.WriteLine("Press any key...");
			Console.Read();
		}
	}
}
