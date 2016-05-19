using System.Collections.Generic;
using System.Messaging;
using Moq;
using NUnit.Framework;
using QueueContentEditor.Helpers;
using QueueContentEditor.Models;

namespace QueueContentEditor.Tests
{
	[TestFixture]
	public class QueueEditorViewModelTests
	{
		private Mock<IQueueHelper> _queueHelperMock;
		private Mock<IVisibilityHelper> _visibilityHelperMock;
		QueueEditorViewModel _vm;

		[SetUp]
		public void SetUp()
		{
			_queueHelperMock = new Mock<IQueueHelper>();
			_visibilityHelperMock = new Mock<IVisibilityHelper>();
			//_vm = new QueueEditorViewModel(_queueHelperMock.Object, _visibilityHelperMock.Object);
			_vm = new QueueEditorViewModel();
		}

		[Test]
		public void At_Initialisation_Editor_Has_No_Queue_References()
		{
			QueueEditorModel qem = _vm.Editor;
			List<string> messageLabels = qem.MessageLabels;

			Assert.False(!QueuesAreSet());
			Assert.That(messageLabels.Count, Is.EqualTo(0));
		}

		[Test]
		public void At_Initialisation_Visibility_Is_Not_Defined()
		{
			VisibilityModel visibility = _vm.Visibility;
			Assert.Null(visibility);
		}

		[Test]
		public void At_Initialisation_MessageBody_Is_Empty()
		{
			string body = _vm.MessageBody;
			Assert.True(string.IsNullOrEmpty(body));
		}

		[Test]
		public void At_Initialisation_EventCommand_Is_Reset()
		{
			string eventCommand = _vm.EventCommand;
			Assert.That(eventCommand, Is.EqualTo("Reset"));
		}

		[Test]
		public void On_Reset_Event()
		{
			_vm.EventCommand = "Reset";
			_vm.HandleRequest();
			Assert.True(!QueuesAreSet());
			Assert.That(GetVisibilitySettingName(), Is.EqualTo("undefined"));
		}

		[Test]
		public void On_GetErrorQueue_Event_With_Invalid_Selection()
		{
			_queueHelperMock.Setup(x => x.ValidSelection(It.IsAny<string>())).Returns(false);
			_vm.EventCommand = "geterrorqueue";
			_vm.HandleRequest();
			Assert.True(!QueuesAreSet());
			Assert.That(GetVisibilitySettingName(), Is.EqualTo("undefined"));
			_queueHelperMock.Verify(x => x.ValidSelection(It.IsAny<string>()));
		}

		[Test]
		public void On_GetErrorQueue_Event_With_Valid_Selection_And_No_Labels()
		{
			List<string> messageLabels = new List<string>();
			_queueHelperMock.Setup(x => x.ValidSelection(It.IsAny<string>())).Returns(true);
			_queueHelperMock.Setup(x => x.GetListOfMessageLabelsFromErrorQueue(It.IsAny<MessageQueue>())).Returns(messageLabels);
			_vm.EventCommand = "geterrorqueue";
			_vm.HandleRequest();
			Assert.True(!QueuesAreSet());
			Assert.That(GetVisibilitySettingName(), Is.EqualTo("undefined"));
			_queueHelperMock.Verify(x => x.GetListOfMessageLabelsFromErrorQueue(It.IsAny<MessageQueue>()));
			_queueHelperMock.Verify(x => x.ValidSelection(It.IsAny<string>()));
		}

		[Test]
		public void On_GetErrorQueue_Event_With_Valid_Selection()
		{
			List<string> messageLabels = new List<string> {"Label 1"};
			_queueHelperMock.Setup(x => x.ValidSelection(It.IsAny<string>())).Returns(true);
			_queueHelperMock.Setup(x => x.GetListOfMessageLabelsFromErrorQueue(It.IsAny<MessageQueue>())).Returns(messageLabels);
			_vm.EventCommand = "geterrorqueue";
			_vm.HandleRequest();
			Assert.True(QueuesAreSet());
			Assert.That(GetVisibilitySettingName(), Is.EqualTo("undefined"));
			_queueHelperMock.Verify(x => x.GetListOfMessageLabelsFromErrorQueue(It.IsAny<MessageQueue>()));
			_queueHelperMock.Verify(x => x.ValidSelection(It.IsAny<string>()));
		}

		[Test]
		public void On_SelectLabel_With_Valid_Selection()
		{
			Message msg = new Message();
			_queueHelperMock.Setup(x => x.ValidSelection(It.IsAny<string>())).Returns(true);
			_queueHelperMock.Setup(x => x.GetMessageByLabel(It.IsAny<MessageQueue>(), It.IsAny<string>())).Returns(msg);
			_visibilityHelperMock.Setup(x => x.SetMessageBodyEditorVisibility()).Returns(SetMessageBodyEditorVisibility());
			_vm.EventCommand = "selectlabel";
			_vm.HandleRequest();
			Assert.True(QueuesAreSet());
			Assert.That(GetVisibilitySettingName(), Is.EqualTo("messagebodyeditor"));
			_queueHelperMock.Verify(x => x.GetMessageByLabel(It.IsAny<MessageQueue>(), It.IsAny<string>()));
			_queueHelperMock.Verify(x => x.ValidSelection(It.IsAny<string>()));
		}

		[Test]
		public void On_SelectLabel_With_Invalid_Selection()
		{
			_queueHelperMock.Setup(x => x.ValidSelection(It.IsAny<string>())).Returns(false);
			_vm.EventCommand = "selectlabel";
			_vm.HandleRequest();
			Assert.True(QueuesAreSet());
			Assert.That(GetVisibilitySettingName(),Is.EqualTo("undefined"));
			_queueHelperMock.Verify(x => x.ValidSelection(It.IsAny<string>()));
		}

		private bool QueuesAreSet()
		{
			QueueEditorModel qem = _vm.Editor;
			List<string> errorQueueNames = qem.ErrorQueueNames;
			List<string> inputQueueNames = qem.InputQueueNames;
			if((errorQueueNames != null) && (inputQueueNames != null)) return true;
			return false;
		}

		private string GetVisibilitySettingName()
		{
			if (VisibilityIsUndefined()) return "undefined";
			if (IsSelectMessageVisibility()) return "selectmessage";
			if (IsMessageBodyEditorVisibility()) return "messagebodyeditor";
			return "error";
		}

		private bool VisibilityIsUndefined()
		{
			VisibilityModel visibility = _vm.Visibility;
			if (visibility == null) return true;
			return false;
		}

		private bool IsSelectMessageVisibility()
		{
			VisibilityModel visibility = _vm.Visibility;
			return ((visibility.IsInputQueueVisible == false) &&
			(visibility.IsEditorPanelVisible == false) &&
			(visibility.IsSelectMessageVisible == true) &&
			(visibility.IsErrorQueueVisible == false));
		}

		private bool IsMessageBodyEditorVisibility()
		{
			VisibilityModel visibility = _vm.Visibility;
			return ((visibility.IsInputQueueVisible == false) &&
			(visibility.IsEditorPanelVisible == true) &&
			(visibility.IsSelectMessageVisible == false) &&
			(visibility.IsErrorQueueVisible == false));
		}

		private VisibilityModel SetMessageBodyEditorVisibility() => new VisibilityModel
		{
			IsInputQueueVisible = false,
			IsEditorPanelVisible = true,
			IsSelectMessageVisible = false,
			IsErrorQueueVisible = false
		};
	}
}
