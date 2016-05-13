using System;
using System.Collections.Generic;
using System.Messaging;
using Repository.Queue.Models;

namespace Repository.Queue
{
	public class QueueWrapper<T> : IQueueWrapper<T>
	{
		private readonly MessageQueue _messageQueue;

		public QueueWrapper(MessageQueue messageQueue)
		{
			_messageQueue = messageQueue;
			_messageQueue.Formatter = new XmlMessageFormatter(new[] { typeof(T) });
		}

		public ReceiveResponse<T> Receive()
		{
			try
			{
				Message message = _messageQueue.Receive(TimeSpan.FromSeconds(5));
				return new ReceiveResponse<T>(false, (T)message.Body);
			}
			catch (MessageQueueException e)
			{
				if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
				{
					return new ReceiveResponse<T>(true, default(T));
				}
				throw;
			}
		}

		public List<T> GetAllMessagePayloads()
		{
			var list = new List<T>();
			Message[] messages = _messageQueue.GetAllMessages();
			foreach (Message message in messages)
			{
				list.Add((T)message.Body);
			}
			return list;
		}
	}
}
