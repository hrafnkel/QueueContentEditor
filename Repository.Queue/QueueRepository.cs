using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Messaging;

namespace Repository.Queue
{
    public class QueueRepository : IQueueRepository
	{
		public List<string> GetErrorQueues()
		{
			List<string> errorQueueList = ConfigurationManager.AppSettings.AllKeys
							 .Where(key => key.Contains("Error"))
							 .Select(key => ConfigurationManager.AppSettings[key])
							 .ToList();
			return errorQueueList;
		}

		public List<string> GetInputQueues()
		{
			List<string> inputQueueList = ConfigurationManager.AppSettings.AllKeys
							 .Where(key => key.Contains("Input"))
							 .Select(key => ConfigurationManager.AppSettings[key])
							 .ToList();
			return inputQueueList;
		}

	    public MessageQueue GetMessageQueue(string queueName)
	    {
		    return GetQueueCreateIfNeeded(queueName);
	    }

	    public List<string> GetAllLabelsFromMessageQueue(MessageQueue mq)
	    {
		    List<Message> messages = ReadAllXmlMessageFromQueueLeavingMessageOnQueue(mq);
		    return messages.Select(message => message.Label).ToList();
	    }

	    public Message GetMessageByLabel(MessageQueue mq, string label)
	    {
			List<Message> messages = ReadAllXmlMessageFromQueueLeavingMessageOnQueue(mq);
			return messages.First(message => message.Label == label);
	    }

		public void WriteXmlMessageOnQueue(MessageQueue mq, Message message)
		{
			if (!mq.CanWrite) return;
			mq.Formatter = new XmlMessageFormatter();
			mq.Send(message);
		}

		public void DeleteMessageById(MessageQueue mq, string messageId)
		{
			mq.Formatter = new XmlMessageFormatter();
			mq.ReceiveById(messageId);
		}

		public Message GetMessageById(MessageQueue mq, string messageId)
		{
			try
			{
				mq.Formatter = new XmlMessageFormatter();
				return mq.ReceiveById(messageId);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public MessageQueue GetQueueCreateIfNeeded(string qName)
		{
			MessageQueue mq = null;
			if (string.IsNullOrEmpty(qName)) return null;

			string path = @".\private$\" + qName;

			mq = !MessageQueue.Exists(path) ? MessageQueue.Create(path, false) : new MessageQueue(path);
			mq.Label = qName;
			mq.MessageReadPropertyFilter.SetAll();
			mq.DefaultPropertiesToSend.Recoverable = true;
			return mq;
		}

		public List<Message> ReadAllXmlMessageFromQueueLeavingMessageOnQueue(MessageQueue mq)
		{
			List<Message> messages;
			try
			{
				if (!mq.CanRead) return null;
				mq.Formatter = new XmlMessageFormatter();
				messages = mq.GetAllMessages().ToList();
			}
			catch (Exception)
			{
				return null;
			}
			return messages;
		}

	    public int GetQueueDepth(MessageQueue mq)
	    {
			int count = 0;
			var enumerator = mq.GetMessageEnumerator2();
			while (enumerator.MoveNext())
				count++;

			return count;
		}

	    public void DeleteAllMessagesFromQueue(MessageQueue mq)
	    {
		    mq.Purge();
	    }

	    public string GetNextMessageIdFromQueue(MessageQueue mq, double timeout)
	    {
		    Message message;
			TimeSpan ts = TimeSpan.FromSeconds(timeout);
		    try
		    {
			    message = mq.Peek(timeout: ts);
			}
		    catch (MessageQueueException)
		    {
				return string.Empty;
			}
		    if (message != null) return message.Id;
		    return string.Empty;
	    }
	}
}
