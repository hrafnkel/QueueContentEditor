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
		private MessageQueue _mq;

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
