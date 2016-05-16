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
				return new ReceiveResponse<T>(false, (T)message.Body, message.Label);
			}
			catch (MessageQueueException e)
			{
				if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
				{
					return new ReceiveResponse<T>(true, default(T), "Error");
				}
				throw;
			}
		}

		public ReceiveResponse<T> ReceiveTransactional()
		{
			MessageQueueTransaction myTransaction = new MessageQueueTransaction();
			try
			{
				myTransaction.Begin();
				Message message = _messageQueue.Receive(TimeSpan.FromSeconds(5), myTransaction);
				ReceiveResponse<T> response = new ReceiveResponse<T>(false, (T)message.Body, message.Label);
				myTransaction.Commit();
				return response;
			}
			catch (MessageQueueException e)
			{
				if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
				{
					return new ReceiveResponse<T>(true, default(T), "Error");
				}
				throw;
			}
		}

		public bool Send(T payload, string label)
		{
			try
			{
				_messageQueue.Send(payload, label);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool SendTransactional(T payload, string lablel)
		{
			try
			{
				MessageQueueTransaction myTransaction = new MessageQueueTransaction();
				myTransaction.Begin();
				_messageQueue.Send(payload, lablel, myTransaction);
				myTransaction.Commit();
				return true;
			}
			catch (Exception)
			{
				return false;
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
