using System.Threading;
using System.Threading.Tasks;
using EventAggregator.Net.SampleUsage;

namespace EventAggregatorNet.SampleUsage.Samples
{
    public class AsyncSample
    {
        public static void Run()
        {
            var config = new EventAggregator.Config
            {
                // Make the marshaler run in the background thread
                DefaultThreadMarshaler = action => Task.Factory.StartNew(action),
            };

            var eventAggregationManager = new EventAggregator(config);
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