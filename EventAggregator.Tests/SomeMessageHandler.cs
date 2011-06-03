using System.Collections.Generic;

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
}