using Xunit;

namespace EventAggregatorNet.Tests
{
	public class SendMessageWithBaseTypeTests
	{
		public class NewMessage : BaseMessage { }

	    public class BaseMessage {}

	    [Fact]
		public void Should_be_able_to_send_using_base_type()
		{
			var someMessageHandler = new MyClassHandler();
            var eventAggregator = new EventAggregator();
			eventAggregator.AddListener(someMessageHandler);

            eventAggregator.SendMessage<BaseMessage>(new NewMessage());

			someMessageHandler.WasHandled.ShouldBeTrue();
		}

        public class MyClassHandler : IListener<NewMessage>
		{
            public void Handle(NewMessage message)
			{
				WasHandled = true;
			}

			public bool WasHandled { get; set; }
		}
	}
}