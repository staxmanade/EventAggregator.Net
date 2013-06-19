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
            var someMessageHandler = new NewMessageHandler();
            var eventAggregator = new EventAggregator();
			eventAggregator.AddListener(someMessageHandler);

            eventAggregator.SendMessage<BaseMessage>(new NewMessage());

			someMessageHandler.WasHandled.ShouldBeTrue();
		}

        [Fact]
        public void Should_be_able_to_send_to_both_using_base_type()
        {
            var newMessageHandler = new NewMessageHandler();
            var baseMessageHandler = new BaseMessageHandler();
            var eventAggregator = new EventAggregator();
            eventAggregator.AddListener(newMessageHandler);
            eventAggregator.AddListener(baseMessageHandler);

            eventAggregator.SendMessage<BaseMessage>(new NewMessage());

            baseMessageHandler.WasHandled.ShouldBeTrue();
            newMessageHandler.WasHandled.ShouldBeTrue();
        }

        [Fact]
        public void Should_match_same_type_only()
        {
            var newMessageHandler = new NewMessageHandler();
            var baseMessageHandler = new BaseMessageHandler();
            var eventAggregator = new EventAggregator();
            eventAggregator.AddListener(newMessageHandler);
            eventAggregator.AddListener(baseMessageHandler);

            eventAggregator.SendMessage(new BaseMessage());

            baseMessageHandler.WasHandled.ShouldBeTrue();
            newMessageHandler.WasHandled.ShouldBeFalse();
        }

        [Fact]
        public void Should_be_able_to_send_using_object_type()
        {
            var newMessageHandler = new NewMessageHandler();
            var baseMessageHandler = new BaseMessageHandler();
            var eventAggregator = new EventAggregator();
            eventAggregator.AddListener(newMessageHandler);
            eventAggregator.AddListener(baseMessageHandler);

            eventAggregator.SendMessage<object>(new BaseMessage());

            baseMessageHandler.WasHandled.ShouldBeTrue();
            newMessageHandler.WasHandled.ShouldBeFalse();
        }

        public class NewMessageHandler : IListener<NewMessage>
		{
            public void Handle(NewMessage message)
			{
				WasHandled = true;
			}

			public bool WasHandled { get; set; }
		}

        public class BaseMessageHandler : IListener<BaseMessage>
        {
            public void Handle(BaseMessage message)
            {
                WasHandled = true;
            }

            public bool WasHandled { get; set; }
        }
	}
}