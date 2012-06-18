using System.Collections.Generic;
using EventAggregatorSpike.Events;

namespace EventAggregatorNet.Tests
{
    public class SomeMessageHandler : IListener<SomeMessage>
    {
        private readonly List<SomeMessage> _eventsTrapped = new List<SomeMessage>();

        public IEnumerable<SomeMessage> EventsTrapped { get { return _eventsTrapped; } }

        public void Handle(SomeMessage message)
        {
            _eventsTrapped.Add(message);
        }
    }

    public class SomeMessageHandler2 :
        IListener<SomeMessage>,
        IListener<SomeMessage2>
    {
        private readonly List<object> _eventsTrapped = new List<object>();

        public IEnumerable<object> EventsTrapped { get { return _eventsTrapped; } }

        public void Handle(SomeMessage message)
        {
            _eventsTrapped.Add(message);
        }

        public void Handle(SomeMessage2 message)
        {
            _eventsTrapped.Add(message);
        }
    }

    public interface IHandlerOfMultipleMessages : IListener<SomeMessage>,
        IListener<SomeMessage2>
    { }

    public class SomeMessageHandler3 : IHandlerOfMultipleMessages
    {
        private readonly List<object> _eventsTrapped = new List<object>();

        public IEnumerable<object> EventsTrapped { get { return _eventsTrapped; } }

        public void Handle(SomeMessage message)
        {
            _eventsTrapped.Add(message);
        }

        public void Handle(SomeMessage2 message)
        {
            _eventsTrapped.Add(message);
        }
    }
}