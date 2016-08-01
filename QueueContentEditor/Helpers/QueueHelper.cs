using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Windows.Forms;
using System.Xml;
using Repository.Queue;
using Message = System.Messaging.Message;

namespace QueueContentEditor.Helpers
{
	public class QueueHelper : IQueueHelper
	{
		private readonly IQueueRepository _queueRepository;

		private static readonly bool IsRunningFromNUnit = AppDomain.CurrentDomain.GetAssemblies().Any(
			a => a.FullName.ToLowerInvariant().StartsWith("nunit.framework"));

		public QueueHelper(IQueueRepository queueRepository)
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
			try
			{
				long len = msg.BodyStream.Length;
				byte[] buffer = new byte[len];

				using (MemoryStream ms = new MemoryStream())
				{
					int read;
					while ((read = msg.BodyStream.Read(buffer, 0, buffer.Length)) > 0)
					{
						ms.Write(buffer, 0, read);
					}
					
					return System.Text.Encoding.UTF8.GetString(ms.ToArray());
				}
			}
			catch (NullReferenceException)
			{
				return msg.Body.ToString();
			}
		}

		public string ConvertXmlToText(string messageBody)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(messageBody);
			return xmlDocument.InnerText;
		}

		public bool ValidSelection(string selected)
		{
			if ((selected == "-- Select --") || (string.IsNullOrWhiteSpace(selected)))
			{
				var text = $"Selection error: {selected}\nPlease try again.";
				if(!IsRunningFromNUnit) MessageBox.Show(text, "Invalid Selection", MessageBoxButtons.OK);
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

		public List<MessageQueue> GetAllPrivateQueues()
		{
			return _queueRepository.GetAllPrivateQueues();
		}

	    public MessageQueue GetQueueByName(string name)
	    {
	        return _queueRepository.GetMessageQueue(name);
	    }
	}
}