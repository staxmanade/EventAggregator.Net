using System;
using Xunit;

namespace EventAggregatorNet.Tests
{
	public class MultipleListenersOnAggregatingInterfaceTests
	{
		public class MyClass1 { }
		public class MyClass2 { }
		public interface IListenerAggregate : IListener<MyClass1>, IListener<MyClass2>
		{
		}

		[Fact]
		public void Should_be_able_to_subscribe_to_aggregating_interface()
		{
			var someMessageHandler = new MyClassHandler();
			var eventAggregator = new EventAggregator();
			eventAggregator.AddListener(someMessageHandler);

			eventAggregator.SendMessage<MyClass1>();
			eventAggregator.SendMessage<MyClass2>();

			someMessageHandler.MyClass1.ShouldBeTrue();
			someMessageHandler.MyClass2.ShouldBeTrue();
		}

		public class MyClassHandler : IListenerAggregate
		{
			public void Handle(MyClass1 message)
			{
				MyClass1 = true;
			}

			public bool MyClass1 { get; set; }
			public bool MyClass2 { get; set; }

			public void Handle(MyClass2 message)
			{
				MyClass2 = true;
			}
		}
	}
}