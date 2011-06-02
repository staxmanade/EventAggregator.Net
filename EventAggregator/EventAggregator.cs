using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventAggregator
{
	/*
	 * EventAggregator origins based on work from StatLight's EventAggregator. Which 
	 * is based on original work by Jermey Miller's EventAggregator in StoryTeller 
	 * with some concepts pulled from Rob Eisenberg in caliburnmicro.
	 * 
	 * TODO:
	 *		Possibly provide well defined initial thread marshalling actions (depending on platform (WinForm, WPF, Silverlight, WP7???)
	 *		Document the public API better.
	 */

	/// <summary>
	/// Marker interface - TODO: find way to remove
	/// </summary>
	public interface IListener { }

	/// <summary>
	/// Specifies a class that would like to receive particular messages.
	/// </summary>
	/// <typeparam name="TMessage">The type of message object to subscribe to.</typeparam>
	public interface IListener<in TMessage> : IListener
	{
		/// <summary>
		/// This will be called every time a TMessage is published through the event aggregator
		/// </summary>
		void Handle(TMessage message);
	}

	/// <summary>
	/// Provides a way to add and remove a listener object from the EventAggregator
	/// </summary>
	public interface IEventSubscriptionManager
	{
		/// <summary>
		/// Adds
		/// </summary>
		/// <param name="listener"></param>
		/// <returns>Returns the current instance of IEventSubscriptionManager to allow for easy additional</returns>
		IEventSubscriptionManager AddListener(object listener);
		IEventSubscriptionManager RemoveListener(object listener);
	}

	public interface IEventPublisher
	{
		void SendMessage<TMessage>(TMessage message, Action<Action> marshal = null);
		void SendMessage<TMessage>(Action<Action> marshal = null) where TMessage : new();
	}

	public interface IEventAggregator : IEventPublisher, IEventSubscriptionManager { }
	public class EventAggregator : IEventPublisher, IEventSubscriptionManager
	{
		private readonly ListenerWrapperCollection _listeners = new ListenerWrapperCollection();
		private readonly object _sync = new object();
		private readonly Config _config;

		public EventAggregator()
			: this(new Config())
		{
		}

		public EventAggregator(Config config)
		{
			_config = config;
		}

		public void SendMessage<TMessage>(TMessage message, Action<Action> marshal = null)
		{
			if (marshal == null)
				marshal = _config.ThreadMarshaller;

			var wasAnyMessageHandled = Call<IListener<TMessage>>(message, marshal);

			if (wasAnyMessageHandled)
				return;
		}

		public void SendMessage<TMessage>(Action<Action> marshal = null) where TMessage : new()
		{
			SendMessage(new TMessage(), marshal);
		}

		private bool Call<TListener>(object message, Action<Action> marshaller)
			where TListener : class
		{
			int listenerCalledCount = 0;
			marshaller(() =>
			{
				foreach (ListenerWrapper o in _listeners)
				{
					if (o.Handles<TListener>())
					{
						bool wasThisOneCalled = false;
						o.TryHandle<TListener>(message, ref wasThisOneCalled);
						if (wasThisOneCalled)
							listenerCalledCount++;
					}
				}
			});

			var wasAnyListenerCalled = listenerCalledCount > 0;

			if (!wasAnyListenerCalled)
			{
				_config.OnMessagePublishedWithZeroListeners(message);
			}
			return wasAnyListenerCalled;
		}

		public IEventSubscriptionManager AddListener(object listener)
		{
			_listeners.AddListener(listener);

			return this;
		}

		public IEventSubscriptionManager RemoveListener(object listener)
		{
			lock (_sync)
			{
				_listeners.RemoveListener(listener);
			}
			return this;
		}


		class ListenerWrapperCollection : IEnumerable<ListenerWrapper>
		{
			private readonly List<ListenerWrapper> _listeners = new List<ListenerWrapper>();
			private readonly object _sync = new object();

			public void RemoveListener(object listener)
			{
				lock (_sync)
				{
					ListenerWrapper listenerWrapper;
					if (TryGetListenerWrapperByListener(listener, out listenerWrapper))
					{
						_listeners.Remove(listenerWrapper);
					}
				}
			}

			private void RemoveListenerWrapper(ListenerWrapper listenerWrapper)
			{
				lock (_sync)
				{
					_listeners.Remove(listenerWrapper);
				}
			}


			public IEnumerator<ListenerWrapper> GetEnumerator()
			{
				lock (_sync)
				{
					return _listeners.ToList().GetEnumerator();
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}


			private bool ContainsListener(object listener)
			{
				ListenerWrapper listenerWrapper;
				if (TryGetListenerWrapperByListener(listener, out listenerWrapper))
				{
					return true;
				}
				return false;
			}

			private bool TryGetListenerWrapperByListener(object listener, out ListenerWrapper listenerWrapper)
			{
				lock (_sync)
				{
					listenerWrapper = _listeners.SingleOrDefault(x => x.ListenerInstance == listener);
				}

				return listenerWrapper != null;
			}

			public void AddListener(object listener)
			{
				if (!ContainsListener(listener))
				{
					var listenerWrapper = new ListenerWrapper(listener, RemoveListenerWrapper);
					_listeners.Add(listenerWrapper);
				}
			}
		}

		class ListenerWrapper
		{
			private const string HandleMethodName = "Handle";
			private readonly Dictionary<Type, MethodInfo> _supportedListeners = new Dictionary<Type, MethodInfo>();
			private readonly Action<ListenerWrapper> _onRemoveCallback;
			private readonly WeakReference _reference;

			public ListenerWrapper(object listener, Action<ListenerWrapper> onRemoveCallback)
			{
				_onRemoveCallback = onRemoveCallback;

				_reference = new WeakReference(listener);

				var listenerInterfaces = listener
					.GetType()
					.GetInterfaces()
					.Where(x => typeof(IListener).IsAssignableFrom(x) && x.IsGenericType);

				foreach (var listenerInterface in listenerInterfaces)
				{
					var handleMethod = listenerInterface.GetMethod(HandleMethodName);
					var messageType = listenerInterface.GetGenericArguments().First();
					_supportedListeners.Add(messageType, handleMethod);
				}
			}

			public object ListenerInstance { get { return _reference.Target; } }

			public bool Handles<TListener>()
				where TListener : class
			{
				var messageType = typeof(TListener).GetGenericArguments().First();
				if (_supportedListeners.ContainsKey(messageType))
					return true;
				return false;
			}

			public void TryHandle<TListener>(object message, ref bool wasHandled)
				where TListener : class
			{
				var target = _reference.Target;
				wasHandled = false;
				if (target == null)
				{
					_onRemoveCallback(this);
					return;
				}

				var messageType = typeof(TListener).GetGenericArguments().First();

				if (!_supportedListeners.ContainsKey(messageType))
					return;

				_supportedListeners[messageType].Invoke(target, new[] { message });
				wasHandled = true;
			}
		}


		public class Config
		{
			public Action<object> OnMessagePublishedWithZeroListeners = msg => { /* TODO: possibly Trace message?*/ };
			public Action<Action> ThreadMarshaller = action => action();
		}
	}
}
