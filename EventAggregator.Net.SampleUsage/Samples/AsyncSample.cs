﻿using System.Threading;
using System.Threading.Tasks;
using EventAggregator.Net.SampleUsage;
using EventAggregatorSpike.Events;

namespace EventAggregatorNet.SampleUsage.Samples
{
    public class AsyncSample
    {
        public static void Run()
        {
            var config = new EventAggregatorSpike.Events.EventAggregator.Config
            {
                // Make the marshaler run in the background thread
                DefaultThreadMarshaler = action => Task.Factory.StartNew(action),
            };

            var eventAggregationManager = new EventAggregatorSpike.Events.EventAggregator(config);
            eventAggregationManager.AddListener(new LongRunningHandler());

            "EventAggregator setup complete".Log();

            eventAggregationManager.SendMessage<SampleEventMessage>();
        }
    }


    public class LongRunningHandler : IListener<SampleEventMessage>
    {
        public void Handle(SampleEventMessage message)
        {
            "LongRunningHandler - Received event".Log();
            Thread.Sleep(1000);
            "LongRunningHandler - Done with work".Log();
        }
    }
}