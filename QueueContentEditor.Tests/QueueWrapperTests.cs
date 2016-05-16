using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Repository.Queue;
using Repository.Queue.Models;

namespace QueueContentEditor.Tests
{
	

	[TestFixture]
	public class QueueWrapperTests
	{
		readonly QueueRepository _repository = new QueueRepository();
		private string _inputQueueName = "InputQueue";
		private static MessageQueue _mq;
		private QueueWrapper<string> _textQueueWrapper;
		private QueueWrapper<int> _intQueueWrapper;

		[SetUp]
		public void SetUp()
		{
			_mq = _repository.GetMessageQueue(_inputQueueName);
			_textQueueWrapper = new QueueWrapper<string>(_mq);
			_intQueueWrapper = new QueueWrapper<int>(_mq);
			DeleteAllMessagesFromQueue();
		}

		[Test]
		public void The_Text_Queue_Wrapper_Is_Correctly_Initialised()
		{
			Assert.That(_textQueueWrapper, Is.TypeOf(typeof(QueueWrapper<string>)));
		}

		[Test]
		public void The_Integer_Queue_Wrapper_Is_Correctly_Initialised()
		{
			Assert.That(_intQueueWrapper, Is.TypeOf(typeof(QueueWrapper<int>)));
		}

		[Test]
		public void An_Empty_Queue_Returns_An_Empty_Payload_List()
		{
			List<string> payloads = _textQueueWrapper.GetAllMessagePayloads();
			int numberOfPayloads = payloads.Count;
			Assert.That(numberOfPayloads, Is.EqualTo(0));
		}

		[Test]
		public void A_Text_Message_Is_Put_On_The_Queue()
		{
			string label = string.Empty;
			DeleteAllMessagesFromQueue();
			int depth = _repository.GetQueueDepth(_mq);
			Assert.That(depth, Is.EqualTo(0));

			var wasSent = _textQueueWrapper.Send("Payload", label);

			Assert.True(wasSent);

			depth = _repository.GetQueueDepth(_mq);
			Assert.That(depth, Is.EqualTo(1));
		}

		[Test]
		public void A_Message_Put_On_The_Queue_Wth_Null_Label_Is_Not_Sent()
		{
			string label = null;
			DeleteAllMessagesFromQueue();
			int depth = _repository.GetQueueDepth(_mq);
			Assert.That(depth, Is.EqualTo(0));

			var wasSent = _textQueueWrapper.Send("Payload", label);

			Assert.False(wasSent);
			depth = _repository.GetQueueDepth(_mq);
			Assert.That(depth, Is.EqualTo(0));
		}

		[Test]
		public void An_Integer_Message_Is_Put_On_The_Queue()
		{
			string label = "Number";
			DeleteAllMessagesFromQueue();
			int depth = _repository.GetQueueDepth(_mq);
			Assert.That(depth, Is.EqualTo(0));

			var wasSent = _intQueueWrapper.Send(1234, label);

			Assert.True(wasSent);

			depth = _repository.GetQueueDepth(_mq);
			Assert.That(depth, Is.EqualTo(1));
		}

		[Test]
		public void A_Message_Put_On_A_Queue_In_A_Transaction()
		{
			DeleteAllMessagesFromQueue();
			bool wasSent = _textQueueWrapper.SendTransactional("Payload", "Label");
			Assert.True(wasSent);
		}

		[Test]
		public void A_Message_Received_From_A_Queue_In_A_Transaction()
		{
			string payload = "Payload";
			DeleteAllMessagesFromQueue();
			bool wasSent = _textQueueWrapper.SendTransactional(payload, "Label");
			ReceiveResponse<string> result = _textQueueWrapper.ReceiveTransactional();
			Assert.True(wasSent);
			Assert.That(result.Payload, Is.EqualTo(payload));
		}

		private void DeleteAllMessagesFromQueue()
		{
			_repository.DeleteAllMessagesFromQueue(_mq);

		}
	}
}
