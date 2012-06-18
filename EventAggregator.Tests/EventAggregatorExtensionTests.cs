using EventAggregatorSpike.Events;
using Xunit;

namespace EventAggregatorNet.Tests
{
	public class EventAggregatorExtensionTests
	{
		[Fact]
		public void Can_use_delegate_to_subscribe_to_message()
		{
            var eventAggregator = new EventAggregator();
			SomeMessage messageTrapped = null;

			eventAggregator.AddListenerAction<SomeMessage>(msg => { messageTrapped = msg; });
			eventAggregator.SendMessage<SomeMessage>();

			messageTrapped.ShouldNotBeNull();
		}


		[Fact]
		public void Can_use_unsubscribe_from_delegate_handler()
		{
			var eventAggregator = new EventAggregator();
			SomeMessage messageTrapped = null;

			var disposable = eventAggregator.AddListenerAction<SomeMessage>(msg => { messageTrapped = msg; });
			disposable.Dispose();
			eventAggregator.SendMessage<SomeMessage>();

			messageTrapped.ShouldBeNull();
		}
	}
}