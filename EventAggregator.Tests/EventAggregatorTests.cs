using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EventAggregator.Events;
using Xunit;

namespace EventAggregatorNet.Tests
{
	public class EventAggregatorTests
	{
		[Fact]
		public void Should_send_message()
		{
			var someMessageHandler = new SomeMessageHandler();
            var eventAggregator = new EventAggregator.Events.EventAggregator();

			eventAggregator.AddListener(someMessageHandler);
			eventAggregator.SendMessage<SomeMessage>();
			someMessageHandler.EventsTrapped.Count().ShouldEqual(1);
		}

		[Fact]
		public void When_a_listener_has_been_garbage_collected_and_an_event_is_published_the_zombied_handler_should_be_removed()
		{
            var eventAggregator = new EventAggregator.Events.EventAggregator();

			AddHandlerInScopeThatWillRemoveInstanceWhenGarbageCollected(eventAggregator);
			GC.Collect();

			eventAggregator.SendMessage<SomeMessage>();

			eventAggregator.GetListeners().Count().ShouldEqual(0);
		}
		public void AddHandlerInScopeThatWillRemoveInstanceWhenGarbageCollected(IEventSubscriptionManager eventSubscriptionManager, bool? holdStrongReference = false)
		{
			var someMessageHandler = new SomeMessageHandler();
			eventSubscriptionManager.AddListener(someMessageHandler, holdStrongReference);
		}
		[Fact]
		public void When_instructed_to_hold_a_strong_reference_by_default_and_the_listener_is_attempted__been_garbage_collected_and_an_event_is_published_the_zombied_handler_should_be_removed()
		{
            var config = new EventAggregator.Events.EventAggregator.Config
			{
				HoldReferences = true
			};
            var eventAggregator = new EventAggregator.Events.EventAggregator(config);

			AddHandlerInScopeThatWillRemoveInstanceWhenGarbageCollected(eventAggregator, null);
			GC.Collect();

			eventAggregator.SendMessage<SomeMessage>();

			eventAggregator.GetListeners().Count().ShouldEqual(1);
		}
		[Fact]
		public void When_instructed_to_hold_a_strong_reference_and_the_listener_is_attempted__been_garbage_collected_and_an_event_is_published_the_zombied_handler_should_be_removed()
		{
            var config = new EventAggregator.Events.EventAggregator.Config
			{
				HoldReferences = true
			};
            var eventAggregator = new EventAggregator.Events.EventAggregator(config);

			AddHandlerInScopeThatWillRemoveInstanceWhenGarbageCollected(eventAggregator, true);
			GC.Collect();

			eventAggregator.SendMessage<SomeMessage>();

			eventAggregator.GetListeners().Count().ShouldEqual(1);
		}


		[Fact]
		public void Can_unsubscribe_manually()
		{
			var someMessageHandler = new SomeMessageHandler();
            var eventAggregator = new EventAggregator.Events.EventAggregator();
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
            var config = new EventAggregator.Events.EventAggregator.Config();
			bool warningWasCalled = false;
			config.OnMessageNotPublishedBecauseZeroListeners = msg => { warningWasCalled = true; };
            var eventAggregator = new EventAggregator.Events.EventAggregator(config);

			eventAggregator.SendMessage<SomeMessage>();

			warningWasCalled.ShouldBeTrue();
		}

        [Fact]
        public void When_object_has_multiple_listeners_should_subscribe_to_all()
        {
            var eventAggregator = new EventAggregator.Events.EventAggregator();
            var handler = new SomeMessageHandler2();
            eventAggregator.AddListener(handler);
            eventAggregator.SendMessage<SomeMessage>();
            eventAggregator.SendMessage<SomeMessage2>();

            handler.EventsTrapped.Count().ShouldEqual(2);
        }

        [Fact]
        public void When_object_has_multiple_listeners_defined_in_an_interface_should_subscribe_to_all()
        {
            var eventAggregator = new EventAggregator.Events.EventAggregator();
            var handler = new SomeMessageHandler3();
            eventAggregator.AddListener(handler);
            eventAggregator.SendMessage<SomeMessage>();
            eventAggregator.SendMessage<SomeMessage2>();

            handler.EventsTrapped.Count().ShouldEqual(2);
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
