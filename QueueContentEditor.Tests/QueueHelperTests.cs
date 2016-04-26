using System.Collections.Generic;
using System.Messaging;
using NUnit.Framework;
using Repository.Queue;

namespace QueueContentEditor.Tests
{
	[TestFixture]
    public class QueueHelperTests
    {
		readonly QueueRepository _repository = new QueueRepository();
		private string _inputQueueName = "InputQueue";
		private static MessageQueue _mq;
		private double _secondsTimeout = 1;

		[SetUp]
		public void SetUp()
		{
			_mq = _repository.GetMessageQueue(_inputQueueName);
			DeleteAllMessagesFromQueue();
		}

		[Test]
		public void An_Error_Queue_List_Is_Returned()
		{
			List<string> queueList = _repository.GetErrorQueues();
			Assert.That(queueList.Count, Is.EqualTo(1));
		}

		[Test]
		public void An_Input_Queue_List_Is_Returned()
		{
			List<string> queueList = _repository.GetInputQueues();
			Assert.That(queueList.Count, Is.EqualTo(3));
		}

		[Test]
		public void Message_Queue_Is_Returned()
		{
			string expected = "private$\\InputQueue";
            MessageQueue mq = _repository.GetMessageQueue(_inputQueueName);
			Assert.That(mq.QueueName, Is.EqualTo(expected));
		}

		[Test]
		public void A_Message_Is_Written_To_The_Queue()
		{
			WriteXmlToQueue();
			int depth = _repository.GetQueueDepth(_mq);
			Assert.That(depth, Is.EqualTo(1));
		}

		[Test]
		public void The_Queue_Is_Emptied()
		{
			const int howMany = 10;
			WriteManyToQueue(howMany);
			int depth = _repository.GetQueueDepth(_mq);
			Assert.That(depth, Is.EqualTo(howMany));

			DeleteAllMessagesFromQueue();
			depth = _repository.GetQueueDepth(_mq);
			Assert.That(depth, Is.EqualTo(0));
		}

		[Test]
		public void Can_Non_Destructively_Read_All_Messages_From_Queue()
		{
			List<Message> messages = _repository.ReadAllXmlMessageFromQueueLeavingMessageOnQueue(_mq);
			int howManyMessagesFetched = messages.Count;
			int howManyOnQueue = _repository.GetQueueDepth(_mq);
			Assert.That(howManyOnQueue, Is.EqualTo(howManyMessagesFetched));
		}

		[Test]
		public void Bad_Queue_Returns_No_Messages_From_Queue()
		{
			MessageQueue mq2 = null;
			List<Message> messages = _repository.ReadAllXmlMessageFromQueueLeavingMessageOnQueue(mq2);
			Assert.That(messages, Is.Null);
		}

		[Test]
		public void The_Id_Can_Be_Got_For_The_Next_Message()
		{
			WriteXmlToQueue();
			string id = _repository.GetNextMessageIdFromQueue(_mq, _secondsTimeout);
			Assert.False(string.IsNullOrEmpty(id));
		}

		[Test]
		public void No_Id_Can_Be_Got_For_An_Empty_Queue()
		{
			DeleteAllMessagesFromQueue();
			string id = _repository.GetNextMessageIdFromQueue(_mq, _secondsTimeout);
			Assert.True(string.IsNullOrEmpty(id));
		}

		[Test]
		public void A_Message_Is_Fetched_By_Id()
		{
			WriteXmlToQueue();
			string expectedId = _repository.GetNextMessageIdFromQueue(_mq, _secondsTimeout);
			Message message = _repository.GetMessageById(_mq, expectedId);
			string actualId = message.Id;
			Assert.That(expectedId, Is.EqualTo(actualId));
		}

		[Test]
		public void A_Message_Is_Deleted_By_Id()
		{
			WriteXmlToQueue();
			string id = _repository.GetNextMessageIdFromQueue(_mq, _secondsTimeout);
			_repository.DeleteMessageById(_mq, id);
			Message message = _repository.GetMessageById(_mq, id);
			Assert.That(message, Is.Null);
		}

		[Test]
		public void A_List_Of_Labels_Is_Fetched_From_A_Queue()
		{
			WriteXmlToQueue();
			List<string> labelList = _repository.GetAllLabelsFromMessageQueue(_mq);
			Assert.That(labelList.Count, Is.GreaterThan(0));
		}

		[Test]
		public void A_Message_Is_Fetched_From_A_Queue_By_Label()
		{
			WriteXmlToQueue();
			List<string> labelList = _repository.GetAllLabelsFromMessageQueue(_mq);
			string label = labelList[0];
			Message message = _repository.GetMessageByLabel(_mq, label);
			Assert.False(string.IsNullOrEmpty(message.Id));
		}

		[TearDown]
		public void TearDown()
		{
			DeleteAllMessagesFromQueue();
		}

		private void WriteManyToQueue(int i)
		{
			for (int j = 0; j < i; j++)
			{
				WriteXmlToQueue();
			}
		}

		private void WriteXmlToQueue()
		{
			string xmlText = @"<p>Blah</p>";
			Message message = new Message(xmlText, new XmlMessageFormatter());
			_repository.WriteXmlMessageOnQueue(_mq, message);
		}

		private void DeleteAllMessagesFromQueue()
		{
			_repository.DeleteAllMessagesFromQueue(_mq);
		}
	}
}
