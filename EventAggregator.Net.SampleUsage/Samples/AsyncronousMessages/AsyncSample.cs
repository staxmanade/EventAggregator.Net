using System.Threading;
using System.Threading.Tasks;

namespace EventAggregator.Net.SampleUsage.Samples.AsyncronousMessages
{
	public class AsyncSample
	{
		public static void Run()
		{
			var config = new EventAggregationManager.Config
			{
				// Make the marshaler run in the background thread
				ThreadMarshaler = action => Task.Factory.StartNew(action),
			};

			var eventAggregationManager = new EventAggregationManager(config);
			eventAggregationManager.AddListener(new LongRunningHandler());

			"EventAggregator setup complete".Log();

			eventAggregationManager.SendMessage<SampleEventMessage>();
		}
	}


	public class LongRunningHandler : IListener<SampleEventMessage>
	{
		public void Handle(SampleEventMessage message)
		{
			"LongRunningHandler - Received event".Log();
			Thread.Sleep(1000);
			"LongRunningHandler - Done with work".Log();
		}
	}
}