using System.Messaging;
using System.Web.Mvc;
using System.Windows.Forms;
using QueueContentEditor.Helpers;
using Repository.Queue;
using Message = System.Messaging.Message;

namespace QueueContentEditor.Models
{
	public class QueueEditorViewModel
	{
		readonly QueueHelper _queueHelper = new QueueHelper(new QueueRepository());
		readonly VisibilityHelper _visibility = new VisibilityHelper();

		public QueueEditorModel Editor { get; set; }
		public VisibilityModel Visibility { get; private set; }

		[HiddenInput(DisplayValue = false)]
		public string EventCommand { get; set; }

		private static MessageQueue ErrorQueue { get; set; }
		private static MessageQueue NewQueue { get; set; }
		private static Message Msg { get; set; }

		private string Selected { get; set; }
		public string MessageBody { get; private set; }

		private static string EditedMessageBody { get; set; }
		private static string MsgId { get; set; }

		public QueueEditorViewModel()
		{
			Editor = new QueueEditorModel();
			EventCommand = "Reset";
		}

		private void Reset()
		{
			GetQueues();
			Visibility = _visibility.SetDefaultVisibility();
		}

		private void GetQueues()
		{
			Editor.ErrorQueueNames = _queueHelper.GetErrorQueues();
			Editor.InputQueueNames = _queueHelper.GetInputQueues();
		}

		private void GetSelectedErrorQueue()
		{
			if (_queueHelper.ValidSelection(Selected))
			{
				ErrorQueue = _queueHelper.GetMessageQueue(Selected);
				Visibility = _visibility.SetSelectMessageVisibility();
				Editor.MessageLabels = _queueHelper.GetListOfMessageLabelsFromErrorQueue(ErrorQueue);
				if (Editor.MessageLabels.Count != 0) return;
				MessageBox.Show("There are no messages in the queue.", "Empty", MessageBoxButtons.OK);
			}
			Reset();
		}

		private void GetSelectedMessage()
		{
			if (_queueHelper.ValidSelection(Selected))
			{
				Msg = _queueHelper.GetMessageByLabel(ErrorQueue, Selected);
				MsgId = Msg.Id;
				MessageBody = _queueHelper.ReadMessageBody(Msg);
				Visibility = _visibility.SetMessageBodyEditorVisibility();
			}
		}

		private void SelectOutputQueue()
		{
			if (_queueHelper.ValidSelection(Selected))
			{
				MessageBody = Selected;
				EditedMessageBody = Selected;
				Visibility = _visibility.SetMessageBodyEditorQueueVisibility();
				GetQueues();
			}
		}
		
		private void SaveMessage()
		{
			if(_queueHelper.ValidSelection(Selected))
			{
				NewQueue = _queueHelper.GetMessageQueue(Selected);
				Message message = new Message {Label = Msg.Label, Body = _queueHelper.ConvertXmlToText(EditedMessageBody) };
				_queueHelper.WriteXmlMessageOnQueue(NewQueue, message);
				_queueHelper.DeleteMessageById(ErrorQueue, MsgId);
				var text = $"Message Posted To Queue";
				MessageBox.Show(text, "Invalid Selection", MessageBoxButtons.OK);
			}
		}
		
		public void HandleRequest()
		{
			string cmd = EventCommand.ToLower();
			
			switch (cmd)
			{
				case "reset":
					Reset();
					break;
				case "geterrorqueue":
					GetSelectedErrorQueue();
					break;
				case "selectlabel":
					GetSelectedMessage();
					break;
				case "editbodyselectq":
					SelectOutputQueue();
					break;
				case "editbodycancel":
					Reset();
					break;
				case "post":
					SaveMessage();
					Reset();
					break;
				default:
					var text = $"Command error: {cmd}";
					MessageBox.Show(text, "Unknown Command", MessageBoxButtons.OK);
					Reset();
					break;
			}
		}
	}
}