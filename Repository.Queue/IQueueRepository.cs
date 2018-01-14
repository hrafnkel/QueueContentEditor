using System.Collections.Generic;
using System.Messaging;

namespace Repository.Queue
{
	public interface IQueueRepository
	{
		List<string> GetErrorQueues();
		List<string> GetInputQueues();
		MessageQueue GetMessageQueue(string queueName);
		List<string> GetAllLabelsFromMessageQueue(MessageQueue mq);
		Message GetMessageByLabel(MessageQueue mq, string label);
		void WriteXmlMessageOnQueue(MessageQueue mq, Message message);
		void DeleteMessageById(MessageQueue mq, string messageId);
		MessageQueue GetQueueCreateIfNeeded(string qName);
		List<Message> ReadAllXmlMessageFromQueueLeavingMessageOnQueue(MessageQueue mq);
		List<MessageQueue> GetAllPrivateQueues();
	}
}
