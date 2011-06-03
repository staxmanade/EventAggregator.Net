using System;

namespace EventAggregator.Net.SampleUsage
{
	public static class HelperExtensions
	{
		public static void Log(this string msg, params object[] args)
		{
			var newMsg = string.Format("{0} - {1}", DateTime.Now, msg);
			Console.WriteLine(newMsg, args);
		}
	}
}