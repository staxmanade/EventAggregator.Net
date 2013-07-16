using Xunit;

namespace EventAggregatorNet.Tests
{
	public class MessageInheritanceTests
	{
		public class NewMessage : BaseMessage { }

        public class BaseMessage : IMessage { }

	    public interface IMessage {}

	    [Fact]
		public void Should_be_able_to_send_using_base_type()
		{
            var someMessageHandler = new NewMessageHandler();
            var eventAggregator = new EventAggregator();
			eventAggregator.AddListener(someMessageHandler);

            eventAggregator.SendMessage<BaseMessage>(new NewMessage());

			someMessageHandler.NewMessageWasHandled.ShouldBeTrue();
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

            baseMessageHandler.BaseMessageWasHandled.ShouldBeTrue();
            newMessageHandler.NewMessageWasHandled.ShouldBeTrue();
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

            baseMessageHandler.BaseMessageWasHandled.ShouldBeTrue();
            newMessageHandler.NewMessageWasHandled.ShouldBeFalse();
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

            baseMessageHandler.BaseMessageWasHandled.ShouldBeTrue();
            newMessageHandler.NewMessageWasHandled.ShouldBeFalse();
        }

        [Fact]
        public void Message_inheritance_enabled_should_be_able_to_send_class_using_inheritance()
        {
            var newMessageHandler = new NewMessageHandler();
            var baseMessageHandler = new BaseMessageHandler();
            var eventAggregator = new EventAggregator(new EventAggregator.Config { SupportMessageInheritance = true });
            eventAggregator.AddListener(newMessageHandler);
            eventAggregator.AddListener(baseMessageHandler);

            eventAggregator.SendMessage<object>(new NewMessage());

            baseMessageHandler.BaseMessageWasHandled.ShouldBeTrue();
            newMessageHandler.NewMessageWasHandled.ShouldBeTrue();
        }

        [Fact]
        public void Message_inheritance_default_should_not_be_able_to_send_class_using_inheritance()
        {
            var newMessageHandler = new NewMessageHandler();
            var baseMessageHandler = new BaseMessageHandler();
            var eventAggregator = new EventAggregator();
            eventAggregator.AddListener(newMessageHandler);
            eventAggregator.AddListener(baseMessageHandler);

            eventAggregator.SendMessage<object>(new NewMessage());

            baseMessageHandler.BaseMessageWasHandled.ShouldBeFalse();
            newMessageHandler.NewMessageWasHandled.ShouldBeTrue();
        }

        [Fact]
        public void Should_be_able_to_send_subclass_using_interface()
        {
            var newMessageHandler = new NewMessageHandler();
            var baseMessageHandler = new BaseMessageHandler();
            var messageHandler = new InterfaceHandler();
            var eventAggregator = new EventAggregator();
            eventAggregator.AddListener(newMessageHandler);
            eventAggregator.AddListener(baseMessageHandler);
            eventAggregator.AddListener(messageHandler);

            eventAggregator.SendMessage<IMessage>(new BaseMessage());

            baseMessageHandler.BaseMessageWasHandled.ShouldBeTrue();
            newMessageHandler.NewMessageWasHandled.ShouldBeFalse();
            messageHandler.MessageWasHandled.ShouldBeTrue();
        }

        [Fact]
        public void Message_inheritance_enabled_should_be_able_to_send_subclass_message()
        {
            var newMessageHandler = new NewMessageHandler();
            var baseMessageHandler = new BaseMessageHandler();
            var messageHandler = new InterfaceHandler();
            var eventAggregator = new EventAggregator(new EventAggregator.Config { SupportMessageInheritance = true });
            eventAggregator.AddListener(newMessageHandler);
            eventAggregator.AddListener(baseMessageHandler);
            eventAggregator.AddListener(messageHandler);

            eventAggregator.SendMessage(new NewMessage());

            baseMessageHandler.BaseMessageWasHandled.ShouldBeTrue();
            newMessageHandler.NewMessageWasHandled.ShouldBeTrue();
            messageHandler.MessageWasHandled.ShouldBeTrue();
        }

        [Fact]
        public void Message_inheritance_default_should_be_able_to_listen_to_subclass_message()
        {
            var newMessageHandler = new NewMessageHandler();
            var baseMessageHandler = new BaseMessageHandler();
            var messageHandler = new InterfaceHandler();
            var eventAggregator = new EventAggregator();
            eventAggregator.AddListener(newMessageHandler);
            eventAggregator.AddListener(baseMessageHandler);
            eventAggregator.AddListener(messageHandler);

            eventAggregator.SendMessage(new NewMessage());

            baseMessageHandler.BaseMessageWasHandled.ShouldBeFalse();
            newMessageHandler.NewMessageWasHandled.ShouldBeTrue();
            messageHandler.MessageWasHandled.ShouldBeFalse();
        }

        [Fact]
        public void Message_inheritance_enabled_should_be_able_to_listen_using_interface()
        {
            var newMessageHandler = new NewMessageHandler();
            var baseMessageHandler = new BaseMessageHandler();
            var messageHandler = new InterfaceHandler();
            var eventAggregator = new EventAggregator(new EventAggregator.Config { SupportMessageInheritance = true });
            eventAggregator.AddListener(newMessageHandler);
            eventAggregator.AddListener(baseMessageHandler);
            eventAggregator.AddListener(messageHandler);

            eventAggregator.SendMessage(new BaseMessage());

            baseMessageHandler.BaseMessageWasHandled.ShouldBeTrue();
            newMessageHandler.NewMessageWasHandled.ShouldBeFalse();
            messageHandler.MessageWasHandled.ShouldBeTrue();
        }

        [Fact]
        public void Message_inheritance_default_should_not_be_able_to_listen_using_interface()
        {
            var newMessageHandler = new NewMessageHandler();
            var baseMessageHandler = new BaseMessageHandler();
            var messageHandler = new InterfaceHandler();
            var eventAggregator = new EventAggregator();
            eventAggregator.AddListener(newMessageHandler);
            eventAggregator.AddListener(baseMessageHandler);
            eventAggregator.AddListener(messageHandler);

            eventAggregator.SendMessage(new BaseMessage());

            baseMessageHandler.BaseMessageWasHandled.ShouldBeTrue();
            newMessageHandler.NewMessageWasHandled.ShouldBeFalse();
            messageHandler.MessageWasHandled.ShouldBeFalse();
        }

        [Fact]
        public void Multiple_listeners_should_support_inheritance_with_base_class()
        {
            var bothMessageHandler = new BothNewAndMessageHandler();

            var eventAggregator = new EventAggregator(new EventAggregator.Config { SupportMessageInheritance = true });
            eventAggregator.AddListener(bothMessageHandler);

            eventAggregator.SendMessage(new BaseMessage());

            bothMessageHandler.MessageWasHandled.ShouldBeTrue();
            bothMessageHandler.NewMessageWasHandled.ShouldBeFalse();
        }

        [Fact]
        public void Multiple_listeners_should_support_inheritance_with_base_class_and_object_caller()
        {
            var bothMessageHandler = new BothNewAndMessageHandler();

            var eventAggregator = new EventAggregator(new EventAggregator.Config { SupportMessageInheritance = true });
            eventAggregator.AddListener(bothMessageHandler);

            eventAggregator.SendMessage<object>(new BaseMessage());

            bothMessageHandler.MessageWasHandled.ShouldBeTrue();
            bothMessageHandler.NewMessageWasHandled.ShouldBeFalse();
        }

        [Fact]
        public void Multiple_listeners_should_support_inheritance_with_normal_class()
        {
            var bothMessageHandler = new BothNewAndMessageHandler();

            var eventAggregator = new EventAggregator(new EventAggregator.Config { SupportMessageInheritance = true });
            eventAggregator.AddListener(bothMessageHandler);

            eventAggregator.SendMessage(new NewMessage());

            bothMessageHandler.MessageWasHandled.ShouldBeTrue();
            bothMessageHandler.NewMessageWasHandled.ShouldBeTrue();
        }

        [Fact]
        public void Multiple_listeners_should_support_inheritance_with_normal_class_and_object_caller()
        {
            var bothMessageHandler = new BothNewAndMessageHandler();

            var eventAggregator = new EventAggregator(new EventAggregator.Config { SupportMessageInheritance = true });
            eventAggregator.AddListener(bothMessageHandler);

            eventAggregator.SendMessage<object>(new NewMessage());

            bothMessageHandler.MessageWasHandled.ShouldBeTrue();
            bothMessageHandler.NewMessageWasHandled.ShouldBeTrue();
        }

        public class InterfaceHandler : IListener<IMessage>
        {
            public void Handle(IMessage message)
            {
                MessageWasHandled = true;
            }

            public bool MessageWasHandled { get; set; }
        }

	    public class NewMessageHandler : IListener<NewMessage>
		{
            public void Handle(NewMessage message)
			{
				NewMessageWasHandled = true;
			}

			public bool NewMessageWasHandled { get; set; }
		}

        public class BaseMessageHandler : IListener<BaseMessage>
        {
            public void Handle(BaseMessage message)
            {
                BaseMessageWasHandled = true;
            }

            public bool BaseMessageWasHandled { get; set; }
        }

        public class BothNewAndMessageHandler : IListener<NewMessage>, IListener<IMessage>
        {
            public void Handle(NewMessage message)
            {
                NewMessageWasHandled = true;
            }

            public bool NewMessageWasHandled { get; set; }

            public void Handle(IMessage message)
            {
                MessageWasHandled = true;
            }

            public bool MessageWasHandled { get; set; }
        }
	}
}