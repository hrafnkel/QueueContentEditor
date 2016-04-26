using System.Collections.Generic;
using System.Messaging;
using System.Windows.Forms;
using System.Xml;
using Repository.Queue;
using Message = System.Messaging.Message;

namespace QueueContentEditor.Helpers
{
	public class QueueHelper : IQueueHelper
	{
		private readonly QueueRepository _queueRepository;

		public QueueHelper(QueueRepository queueRepository)
		{
			_queueRepository = queueRepository;
		}

		public List<string> GetErrorQueues()
		{
			return _queueRepository.GetErrorQueues();
		}

		public List<string> GetInputQueues()
		{
			return _queueRepository.GetInputQueues();
		}

		public string ReadMessageBody(Message msg)
		{
			var bodyLength = (int)msg.BodyStream.Length;
			byte[] messageBytes = new byte[bodyLength];
			msg.BodyStream.Read(messageBytes, 0, bodyLength);
			return System.Text.Encoding.UTF8.GetString(messageBytes);
		}

		public string ConvertXmlToText(string editedMessageBody)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(editedMessageBody);
			return xmlDocument.InnerText;
		}

		public bool ValidSelection(string selected)
		{
			if ((selected == "-- Select --") || (string.IsNullOrWhiteSpace(selected)))
			{
				var text = $"Selection error: {selected}\nPlease try again.";
				MessageBox.Show(text, "Invalid Selection", MessageBoxButtons.OK);
				return false;
			}
			return true;
		}

		public List<string> GetListOfMessageLabelsFromErrorQueue(MessageQueue errorQueue)
		{
			return _queueRepository.GetAllLabelsFromMessageQueue(errorQueue);
		}

		public MessageQueue GetMessageQueue(string queue)
		{
			return _queueRepository.GetMessageQueue(queue);
		}

		public Message GetMessageByLabel(MessageQueue mq, string label)
		{
			return _queueRepository.GetMessageByLabel(mq, label);
		}

		public void WriteXmlMessageOnQueue(MessageQueue mq, Message message)
		{
			_queueRepository.WriteXmlMessageOnQueue(mq, message);
		}

		public void DeleteMessageById(MessageQueue mq, string messageId)
		{
			_queueRepository.DeleteMessageById(mq, messageId);
		}
	}
}