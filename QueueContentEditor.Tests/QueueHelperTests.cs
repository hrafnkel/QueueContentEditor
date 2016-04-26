using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Repository.Queue;

namespace QueueContentEditor.Tests
{
	[TestFixture]
    public class QueueHelperTests
    {
		readonly QueueRepository _repository = new QueueRepository();
		
		[SetUp]
		public void SetUp()
		{
		}

		[Test]
		public void An_Error_Queue_List_Is_Returned()
		{
			List<string> queueList = _repository.GetErrorQueues();
			Assert.That(queueList.Count(), Is.EqualTo(1));
		}

		[Test]
		public void An_Input_Queue_List_Is_Returned()
		{
			List<string> queueList = _repository.GetInputQueues();
			Assert.That(queueList.Count(), Is.EqualTo(3));
		}

		[Test]
		public void Message_Queue_Is_Returned()
		{
			string name = "InputQueue";
			string expected = "private$\\InputQueue";
            MessageQueue mq = _repository.GetMessageQueue(name);
			Assert.That(mq.QueueName, Is.EqualTo(expected));
		}
	}
}
