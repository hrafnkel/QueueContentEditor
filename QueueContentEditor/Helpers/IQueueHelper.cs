using System.Collections.Generic;
using System.Messaging;

namespace QueueContentEditor.Helpers
{
	public interface IQueueHelper
	{
		List<string> GetErrorQueues();
		List<string> GetInputQueues();
		string ReadMessageBody(Message msg);
		string ConvertXmlToText(string editedMessageBody);
		bool ValidSelection(string selected);
		List<string> GetListOfMessageLabelsFromErrorQueue(MessageQueue errorQueue);
		MessageQueue GetMessageQueue(string queue);
		Message GetMessageByLabel(MessageQueue mq, string label);
		void WriteXmlMessageOnQueue(MessageQueue mq, Message message);
		void DeleteMessageById(MessageQueue mq, string messageId);
	}
}