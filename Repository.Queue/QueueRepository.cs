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
		    List<Message> messages = ReadAllXmlMessageFromQueueLeavingMessageOnQueue(mq, 10);
		    return messages.Select(message => message.Label).ToList();
	    }

	    public Message GetMessageByLabel(MessageQueue mq, string label)
	    {
			List<Message> messages = ReadAllXmlMessageFromQueueLeavingMessageOnQueue(mq, 10);
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

		public MessageQueue GetQueueCreateIfNeeded(string qName)
		{
			MessageQueue mq = null;
			if (string.IsNullOrEmpty(qName)) return mq;

			string path = @".\private$\" + qName;

			if (!MessageQueue.Exists(path))
			{
				mq = MessageQueue.Create(path, false);
			}
			else
			{
				mq = new MessageQueue(path);
			}
			mq.Label = qName;
			mq.MessageReadPropertyFilter.SetAll();
			mq.DefaultPropertiesToSend.Recoverable = true;
			return mq;
		}

		public List<Message> ReadAllXmlMessageFromQueueLeavingMessageOnQueue(MessageQueue mq, int timeout)
		{
			if (!mq.CanRead) return null;
			mq.Formatter = new XmlMessageFormatter();
			var messages = new List<Message>();
			try
			{
				messages = mq.GetAllMessages().ToList();
			}
			catch (Exception)
			{
				
			}
			return messages;
		}
	}
}
