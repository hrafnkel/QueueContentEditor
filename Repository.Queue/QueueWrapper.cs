using System;
using System.Collections.Generic;
using System.Diagnostics;
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
				return new ReceiveResponse<T>(false, (T)message.Body, message.Label, message.Id);
			}
			catch (MessageQueueException e)
			{
				if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
				{
					return new ReceiveResponse<T>(true, default(T), "Error: Timeout", string.Empty);
				}
				throw;
			}
		}

		public ReceiveResponse<T> ReceiveById(string messageId)
		{
			try
			{
				Message message = _messageQueue.ReceiveById(messageId);
				return new ReceiveResponse<T>(false, (T)message.Body, message.Label, message.Id);
			}
			catch (MessageQueueException e)
			{
				if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
				{
					return new ReceiveResponse<T>(true, default(T), "Error: Timeout", string.Empty);
				}
				throw;
			}
		}

		public ReceiveResponse<T> ReceiveTransactional()
		{
			if (_messageQueue.Transactional)
			{
				return new ReceiveResponse<T>(true, default(T), "Error: Queue is not transactional type" , string.Empty);
			}

			try
			{
				Message message = _messageQueue.Receive(TimeSpan.FromSeconds(5), MessageQueueTransactionType.Automatic);
				ReceiveResponse<T> response = new ReceiveResponse<T>(false, (T)message.Body, message.Label, message.Id);
				return response;
			}
			catch (MessageQueueException e)
			{
				if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
				{
					return new ReceiveResponse<T>(true, default(T), "Error: Timeout", string.Empty);
				}
				if (e.MessageQueueErrorCode == MessageQueueErrorCode.TransactionUsage)
				{
					return new ReceiveResponse<T>(true, default(T), "Error: Transaction Usage", string.Empty);
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

		public bool SendBatchTransactional(List<T> payloads, string lablel)
		{
			try
			{
				MessageQueueTransaction myTransaction = new MessageQueueTransaction();
				myTransaction.Begin();

				foreach (T payload in payloads)
				{
					_messageQueue.Send(payload, lablel, myTransaction);
				}

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

		public List<ReceiveResponse<T>> PeekAllMessages(int timeoutSeconds)
		{
			List<ReceiveResponse<T>> list = new List<ReceiveResponse<T>>();
			Cursor cursor = _messageQueue.CreateCursor();
			PeekAction action = PeekAction.Current;
			Message message;
			try
			{
				while ((message = _messageQueue.Peek(TimeSpan.FromSeconds(timeoutSeconds), cursor, action)) != null)
				{
					ReceiveResponse<T> response = new ReceiveResponse<T>(false, (T)message.Body, message.Label, message.Id);
					list.Add(response);
					action = PeekAction.Next;
				}
			}
			catch (MessageQueueException e)
			{
				if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
				{
					return list;
				}
				Debug.WriteLine(e);
				throw;
			}

			return list;
		}

		public List<T> PeekAllMessagePayloads(int timeoutSeconds)
		{
			var list = new List<T>();
			Cursor cursor = _messageQueue.CreateCursor(); 
			PeekAction action = PeekAction.Current;
			Message message;
			try
			{
				while ((message = _messageQueue.Peek(TimeSpan.FromSeconds(timeoutSeconds), cursor, action)) != null)
				{
					list.Add((T)message.Body);
					action = PeekAction.Next;
				}
			}
			catch (MessageQueueException e)
			{
				if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
				{
					return list;
				}
				Debug.WriteLine(e);
				throw;
			}

			return list;
		}

		public int GetQueueDepth()
		{
			int count = 0;
			var enumerator = _messageQueue.GetMessageEnumerator2();
			while (enumerator.MoveNext())
				count++;

			return count;
		}

		public void DeleteAllMessagesFromQueue()
		{
			_messageQueue.Purge();
		}
	}
}
