using System;

namespace EventAggregator.Net.SampleUsage
{
	public class SampleEventMessage
	{
		public SampleEventMessage()
			: this(DateTime.Now, string.Empty)
		{
		}

		public SampleEventMessage(DateTime eventPublished, string message)
		{
			EventPublished = eventPublished;
			Message = message;
		}

		public DateTime EventPublished { get; set; }
		public string Message { get; set; }
	}
}