using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace EventAggregator.Tests
{
	public class EventAggregatorTests
	{
		[Fact]
		public void Should_send_message()
		{
			var someMessageHandler = new SomeMessageHandler();
			var eventAggregator = new EventAggregationManager();

			eventAggregator.AddListener(someMessageHandler);
			eventAggregator.SendMessage<SomeMessage>();
			someMessageHandler.EventsTrapped.Count().ShouldEqual(1);
		}

		[Fact]
		public void When_a_listener_has_been_garbage_collected_the_handler_should_be_removed()
		{
			var eventAggregator = new EventAggregationManager();

			AddHandlerInScopeThatWillRemoveInstanceWhenGarbageCollected(eventAggregator);
			GC.Collect();

			eventAggregator.SendMessage<SomeMessage>();

			eventAggregator.GetListeners().Count().ShouldEqual(0);
		}
		public void AddHandlerInScopeThatWillRemoveInstanceWhenGarbageCollected(IEventSubscriptionManager eventSubscriptionManager)
		{
			var someMessageHandler = new SomeMessageHandler();
			eventSubscriptionManager.AddListener(someMessageHandler);
		}


		[Fact]
		public void Can_unsubscribe_manually()
		{
			var someMessageHandler = new SomeMessageHandler();
			var eventAggregator = new EventAggregationManager();
			eventAggregator.AddListener(someMessageHandler);
			eventAggregator.SendMessage<SomeMessage>();
			someMessageHandler.EventsTrapped.Count().ShouldEqual(1);


			eventAggregator.RemoveListener(someMessageHandler);
			eventAggregator.SendMessage<SomeMessage>();

			someMessageHandler.EventsTrapped.Count().ShouldEqual(1);
		}


		[Fact]
		public void When_no_subscribers_can_detect_nothing_was_published()
		{
			var config = new EventAggregationManager.Config();
			bool warningWasCalled = false;
			config.OnMessageNotPublishedBecauseZeroListeners = msg => { warningWasCalled = true; };
			var eventAggregator = new EventAggregationManager(config);

			eventAggregator.SendMessage<SomeMessage>();


			warningWasCalled.ShouldBeTrue();
		}

	}


	public class SomeMessage { }

	public class SomeMessageHandler : IListener<SomeMessage>
	{
		private readonly List<SomeMessage> _eventsTrapped = new List<SomeMessage>();

		public IEnumerable<SomeMessage> EventsTrapped { get { return _eventsTrapped; } }

		public void Handle(SomeMessage message)
		{
			_eventsTrapped.Add(message);
		}
	}

	public static class EventAggregatorTestExtensions
	{
		public static IEnumerable<object> GetListeners(this IEventSubscriptionManager eventSubscriptionManager)
		{
			var field = eventSubscriptionManager.GetType().GetField("_listeners", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			return (IEnumerable<object>)field.GetValue(eventSubscriptionManager);
		}
	}
}
