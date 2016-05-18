using System.Collections.Generic;
using System.Diagnostics;
using System.Messaging;
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
			depth = _repository.GetQueueDepth(_mq);

			Assert.True(wasSent);
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
			string payload = "Payload 0";
			string label = "Label 0";
			DeleteAllMessagesFromQueue();
			WriteManyToQueue(1);
			ReceiveResponse<string> result = _textQueueWrapper.ReceiveTransactional();
			Assert.That(result.Label, Is.EqualTo(label));
			Assert.That(result.Payload, Is.EqualTo(payload));
		}

		[Test, Explicit]
		public void Write_One_Million()
		{
			int target = 1000000;
			var sw = Stopwatch.StartNew();
			WriteManyToQueue(target);
			sw.Stop();
			var elapsed = sw.ElapsedMilliseconds;
			Debug.WriteLine($"{elapsed} ms");
			DeleteAllMessagesFromQueue();
		}

		[Test, Explicit]
		public void Read_One_Million()
		{
			int target = 1000000;
			WriteManyToQueue(target);
			var sw = Stopwatch.StartNew();
			List<ReceiveResponse<string>> responses = ReadManyStringFromQueue(1000000);
			sw.Stop();
			var elapsed = sw.ElapsedMilliseconds;
			Debug.WriteLine($"{elapsed} ms");
			Assert.That(responses.Count,Is.EqualTo(target));
		}

		[Test]
		public void A_List_Of_String_Messages_Is_Sent()
		{
			string label = "Transaction group messages";
			List<string> payloadList = new List<string> {"p1","p2"};
			bool wasSent = _textQueueWrapper.SendBatchTransactional(payloadList, label);
			Assert.True(wasSent);
		}

		[Test]
		public void A_List_Of_Integer_Messages_Is_Sent()
		{
			string label = "Transaction group messages";
			List<int> payloadList = new List<int> { 1,2,3,4 };
			bool wasSent = _intQueueWrapper.SendBatchTransactional(payloadList, label);
			Assert.True(wasSent);
		}

		private List<ReceiveResponse<string>> ReadManyStringFromQueue(int i)
		{
			List<ReceiveResponse<string>> responses = new List<ReceiveResponse<string>>();

			for (int j = 0; j < i; j++)
			{
				ReceiveResponse<string> r = _textQueueWrapper.Receive();
				responses.Add(r);
			}

			return responses;
		}
		
		private void DeleteAllMessagesFromQueue()
		{
			_repository.DeleteAllMessagesFromQueue(_mq);

		}

		private void WriteManyToQueue(int i)
		{
			for (int j = 0; j < i; j++)
			{
				_textQueueWrapper.Send($"Payload {j}", $"Label {j}");
			}
		}
		
	}
}
