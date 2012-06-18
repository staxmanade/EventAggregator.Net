using EventAggregator.Net.SampleUsage;
using EventAggregatorSpike.Events;

namespace EventAggregatorNet.SampleUsage.Samples
{
    public class BasicSample
    {
        public static void Run()
        {
            var eventAggregationManager = new EventAggregatorSpike.Events.EventAggregator();

            eventAggregationManager.AddListener(new BasicHandler());

            eventAggregationManager.SendMessage<SampleEventMessage>();
        }
    }


    public class MessagePublisher
    {
        private readonly IEventPublisher _eventPublisher;

        public MessagePublisher(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public void DoSomeWork()
        {
            _eventPublisher.SendMessage<SampleEventMessage>();
        }
    }


    public class BasicHandler : IListener<SampleEventMessage>
    {
        public void Handle(SampleEventMessage message)
        {
            "BasicHandler - Received event".Log();
        }
    }
}