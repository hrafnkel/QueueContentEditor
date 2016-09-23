using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Messaging;
using NUnit.Framework;
using Repository.Queue;
using Repository.Queue.Models;

namespace QueueContentEditor.Tests
{
	[TestFixture]
	public class QueueWrapperTests
	{
		private string _inputQueueName = "InputQueue";
		private static MessageQueue _mq;
		private QueueWrapper<string> _textQueueWrapper;
		private QueueWrapper<int> _intQueueWrapper;

		[SetUp]
		public void SetUp()
		{
			_mq = GetMessageQueue(_inputQueueName);
			_textQueueWrapper = new QueueWrapper<string>(_mq);
			_intQueueWrapper = new QueueWrapper<int>(_mq);
			DeleteAllMessagesFromQueues();
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
			DeleteAllMessagesFromQueues();
			int depth = _textQueueWrapper.GetQueueDepth();
			Assert.That(depth, Is.EqualTo(0));

			bool wasSent = _textQueueWrapper.Send("Payload", label);
			Assert.True(wasSent);

			depth = _textQueueWrapper.GetQueueDepth();
			Assert.That(depth, Is.EqualTo(1));
		}

		[Test]
		public void A_Message_Put_On_The_Queue_Wth_Null_Label_Is_Not_Sent()
		{
			const string label = null;
			DeleteAllMessagesFromQueues();
			int depth = _textQueueWrapper.GetQueueDepth();
			Assert.That(depth, Is.EqualTo(0));

			bool wasSent = _textQueueWrapper.Send("Payload", label);
			Assert.False(wasSent);

			depth = _intQueueWrapper.GetQueueDepth();
			Assert.That(depth, Is.EqualTo(0));
		}

		[Test]
		public void An_Integer_Message_Is_Put_On_The_Queue()
		{
			const string label = "Number";
			DeleteAllMessagesFromQueues();
			int depth = _intQueueWrapper.GetQueueDepth();
			Assert.That(depth, Is.EqualTo(0));

			bool wasSent = _intQueueWrapper.Send(1234, label);
			Assert.True(wasSent);

			depth = _intQueueWrapper.GetQueueDepth();
			Assert.That(depth, Is.EqualTo(1));
		}

		[Test]
		public void A_Message_Put_On_A_Queue_In_A_Transaction()
		{
			DeleteAllMessagesFromQueues();
			bool wasSent = _textQueueWrapper.SendTransactional("Payload", "Label");
			Assert.True(wasSent);
		}

		[Test]
		public void A_Message_Received_From_A_Queue_In_A_Transaction()
		{
			string payload = "Payload 0";
			string label = "Label 0";
			DeleteAllMessagesFromQueues();
			WriteManyToQueue(1);
			ReceiveResponse<string> result = _textQueueWrapper.ReceiveTransactional();
			Assert.That(result.Label, Is.EqualTo(label));
			Assert.That(result.Payload, Is.EqualTo(payload));
		}

        [Test, Explicit]
        public void Write_Fifty_Thousand_Pause_Then_Read()
        {
            int target = 50000;
            Stopwatch sw = Stopwatch.StartNew();
            WriteManyToQueue(target);
            sw.Stop();
            long elapsed = sw.ElapsedMilliseconds;
            Debug.WriteLine($"{elapsed} ms");

            System.Threading.Thread.Sleep(10000);

            sw = Stopwatch.StartNew();
            ReadManyStringFromQueue(target);
            sw.Stop();
            elapsed = sw.ElapsedMilliseconds;
            Debug.WriteLine($"{elapsed} ms");
        }

        [Test, Explicit]
		public void Write_One_Million()
		{
			int target = 1000000;
			Stopwatch sw = Stopwatch.StartNew();
			WriteManyToQueue(target);
			sw.Stop();
			long elapsed = sw.ElapsedMilliseconds;
			Debug.WriteLine($"{elapsed} ms");
			DeleteAllMessagesFromQueues();
		}

		[Test, Explicit]
		public void Read_One_Million()
		{
			int target = 1000000;
			WriteManyToQueue(target);
			Stopwatch sw = Stopwatch.StartNew();
			List<ReceiveResponse<string>> responses = ReadManyStringFromQueue(target);
			sw.Stop();
			long elapsed = sw.ElapsedMilliseconds;
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

		[Test]
		public void All_Messages_Are_Read_Non_Destrictively()
		{
			int numberOfMessages = 2;
			int timeoutSeconds = 5;
			WriteManyToQueue(numberOfMessages);
			List<string> payloads = _textQueueWrapper.PeekAllMessagePayloads(timeoutSeconds);
			int numberOfPayloads = payloads.Count;
			Assert.That(numberOfPayloads, Is.EqualTo(numberOfMessages));
		}

		[Test]
		public void A_Message_Is_Deleted_By_Id()
		{
			int numberOfMessages = 1;
			WriteManyToQueue(numberOfMessages);
			int queueDepth = _textQueueWrapper.GetQueueDepth();
			Assert.That(queueDepth, Is.EqualTo(numberOfMessages));

			List<ReceiveResponse<string>> responses = _textQueueWrapper.PeekAllMessages(5);
			ReceiveResponse<string> response = responses.First();
			string id = response.Id;

			ReceiveResponse<string> receiveById = _textQueueWrapper.ReceiveById(id);
			Assert.That(receiveById.Id, Is.EqualTo(id));

			queueDepth = _textQueueWrapper.GetQueueDepth();
			Assert.That(queueDepth, Is.LessThan(numberOfMessages));
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
		
		private void DeleteAllMessagesFromQueues()
		{
			_textQueueWrapper.DeleteAllMessagesFromQueue();
			_intQueueWrapper.DeleteAllMessagesFromQueue();
		}

		private void WriteManyToQueue(int i)
		{
			for (int j = 0; j < i; j++)
			{
				_textQueueWrapper.Send($"Payload {j}", $"Label {j}");
			}
		}
		
		private MessageQueue GetMessageQueue(string queueName)
		{
			return GetQueueCreateIfNeeded(queueName);
		}

		private MessageQueue GetQueueCreateIfNeeded(string qName)
		{
			MessageQueue mq = null;
			if (string.IsNullOrEmpty(qName)) return null;

			string path = @".\private$\" + qName;

			mq = !MessageQueue.Exists(path) ? MessageQueue.Create(path, false) : new MessageQueue(path);
			mq.Label = qName;
			mq.MessageReadPropertyFilter.SetAll();
			mq.DefaultPropertiesToSend.Recoverable = true;
			return mq;
		}
	}
}
