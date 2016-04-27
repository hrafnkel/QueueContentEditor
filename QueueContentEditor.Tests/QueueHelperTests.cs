using System.Collections.Generic;
using System.Messaging;
using Moq;
using NUnit.Framework;
using QueueContentEditor.Helpers;
using Repository.Queue;

namespace QueueContentEditor.Tests
{
	[TestFixture]
	public class QueueHelperTests
	{
		private Mock<IQueueRepository> _queueRepositoryMock;
		private QueueHelper _helper;

		[SetUp]
		public void SetUp()
		{
			_queueRepositoryMock = new Mock<IQueueRepository>();
			_helper = new QueueHelper(_queueRepositoryMock.Object);
		}

		[Test]
		public void A_List_Of_Error_Queues_Is_Returned()
		{
			string expectedQueueName = "queue-1";
			List<string> queueList = new List<string> {expectedQueueName};
			_queueRepositoryMock.Setup(x => x.GetErrorQueues()).Returns(queueList);
			List<string> errorQueueList = _helper.GetErrorQueues();
			string actualQueueName = errorQueueList[0];
			Assert.That(actualQueueName, Is.EqualTo(expectedQueueName));
			_queueRepositoryMock.Verify(x => x.GetErrorQueues());
		}

		[Test]
		public void A_List_Of_Input_Queues_Is_Returned()
		{
			string expectedQueueName = "queue-2";
			List<string> queueList = new List<string> {expectedQueueName};
			_queueRepositoryMock.Setup(x => x.GetInputQueues()).Returns(queueList);
			List<string> inputQueueList = _helper.GetInputQueues();
			string actualQueueName = inputQueueList[0];
			Assert.That(actualQueueName, Is.EqualTo(expectedQueueName));
			_queueRepositoryMock.Verify(x => x.GetInputQueues());
		}

		[Test]
		public void The_Body_Is_Extracted_From_A_Message()
		{
			const string bodyText = "Body Text";
			Message msg = new Message(bodyText);
			string actualBodyText = _helper.ReadMessageBody(msg);
			Assert.That(actualBodyText, Is.EqualTo(bodyText));
		}

		[Test]
		public void The_XML_Body_Is_Returned_As_Text()
		{
			const string body = "<div>Hello World</div>";
			const string expected = "Hello World";
			string actual = _helper.ConvertXmlToText(body);
			Assert.That(actual, Is.EqualTo(expected));
		}

		[TestCase("Fred", true)]
		[TestCase("-- Select --", false)]
		[TestCase("", false)]
		public void Selection_Is_Validated(string selected, bool expected)
		{
			bool result = _helper.ValidSelection(selected);
			Assert.That(result, Is.EqualTo(expected));
		}

		[Test]
		public void A_List_Of_Labels_Is_Fetched_From_The_Error_Queue()
		{
			MessageQueue mq = new MessageQueue();
			const string expected = "Label-1";
			List<string> labels = new List<string> {expected};
			_queueRepositoryMock.Setup(x => x.GetAllLabelsFromMessageQueue(It.IsAny<MessageQueue>())).Returns(labels);
			List<string> resultList = _helper.GetListOfMessageLabelsFromErrorQueue(mq);
			_queueRepositoryMock.Verify(x => x.GetAllLabelsFromMessageQueue(It.IsAny<MessageQueue>()));
			string result = resultList[0];
			Assert.That(result, Is.EqualTo(expected));
		}

		[Test]
		public void GetMessageQueue_Is_Called_From_The_Helper()
		{
			const string qname = "MyQueue";
			MessageQueue mq = new MessageQueue();
			_queueRepositoryMock.Setup(x => x.GetMessageQueue(It.IsAny<string>())).Returns(mq);
			_helper.GetMessageQueue(qname);
			_queueRepositoryMock.Verify(x => x.GetMessageQueue(It.IsAny<string>()));
		}

		[Test]
		public void GetMessageByLabel_Is_Called_From_The_Helper()
		{
			MessageQueue mq = new MessageQueue();
			const string label = "Label";
			_queueRepositoryMock.Setup(x => x.GetMessageByLabel(It.IsAny<MessageQueue>(), It.IsAny<string>()));
			_helper.GetMessageByLabel(mq, label);
			_queueRepositoryMock.Verify(x => x.GetMessageByLabel(It.IsAny<MessageQueue>(), It.IsAny<string>()));
		}

		[Test]
		public void WriteXmlMessageOnQueue_Is_Called_From_The_Helper()
		{
			MessageQueue mq = new MessageQueue();
			Message msg = new Message();
			_queueRepositoryMock.Setup(x => x.WriteXmlMessageOnQueue(It.IsAny<MessageQueue>(), It.IsAny<Message>()));
			_helper.WriteXmlMessageOnQueue(mq, msg);
			_queueRepositoryMock.Verify(x => x.WriteXmlMessageOnQueue(It.IsAny<MessageQueue>(), It.IsAny<Message>()));
		}

		[Test]
		public void DeleteMessageById_Is_Called_From_The_Helper()
		{
			MessageQueue mq = new MessageQueue();
			const string msgId = "Id";
			_queueRepositoryMock.Setup(x => x.DeleteMessageById(It.IsAny<MessageQueue>(), It.IsAny<string>()));
			_helper.DeleteMessageById(mq, msgId);
			_queueRepositoryMock.Verify(x => x.DeleteMessageById(It.IsAny<MessageQueue>(), It.IsAny<string>()));
		}
	}
}
