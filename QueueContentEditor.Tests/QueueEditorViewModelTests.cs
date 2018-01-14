using System.Collections.Generic;
using System.Messaging;
using NUnit.Framework;
using QueueContentEditor.Helpers;
using QueueContentEditor.Models;
using Repository.Queue;

namespace QueueContentEditor.Tests
{
	[TestFixture]
	public class QueueEditorViewModelTests
	{
	    private QueueEditorViewModel _vm;
	    private QueueHelper _helper;
	    private QueueRepository _repository;

		[SetUp]
		public void SetUp()
		{
			_vm = new QueueEditorViewModel();
            _repository = new QueueRepository();
            _helper = new QueueHelper(_repository);
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
			Assert.True(QueuesAreSet());
			Assert.That(GetVisibilitySettingName(), Is.EqualTo("errorqueue"));
		}

		[Test]
		public void On_GetErrorQueue_Event_With_Invalid_Selection()
		{
		    _vm.Selected = "";
			_vm.EventCommand = "geterrorqueue";
			_vm.HandleRequest();
			Assert.True(QueuesAreSet());
			Assert.That(GetVisibilitySettingName(), Is.EqualTo("errorqueue"));
		}

		[Test]
		public void On_GetErrorQueue_Event_With_Valid_Selection()
		{
            _vm.Selected = "errorqueue";
            _vm.EventCommand = "geterrorqueue";
			_vm.HandleRequest();
			Assert.True(QueuesAreSet());
			Assert.That(GetVisibilitySettingName(), Is.EqualTo("selectmessage"));
		}

	    [Test]
	    public void On_SelectLabel_With_Empty_Label_Visibility_Is_Undefined()
	    {
            _vm.Selected = "inputqueue";
            _vm.EventCommand = "selectlabel";
	        _vm.Label = "";
            _vm.HandleRequest();
	        SetMessageBodyEditorVisibility();

            Assert.True(QueuesAreSet());
            Assert.That(GetVisibilitySettingName(), Is.EqualTo("undefined"));
        }

        [Test]
        public void On_SelectLabel_With_Unknown_Label_Visibility_Is_BodyEditor()
        {
            _vm.Selected = "inputqueue";
            _vm.EventCommand = "selectlabel";
            _vm.Label = "*%R*&^V(D";
            _vm.HandleRequest();
            SetMessageBodyEditorVisibility();

            Assert.True(QueuesAreSet());
            Assert.That(GetVisibilitySettingName(), Is.EqualTo("messagebodyeditor"));
        }

        [Test]
        public void On_SelectLabel_With_Known_Label_Visibility_Is_BodyEditor()
        {
            _vm.Selected = "inputqueue";
            _vm.EventCommand = "selectlabel";
            _vm.Label = "Label";
            string text = "Message";

            MessageQueue mq = _helper.GetMessageQueue(_vm.Selected);
            Message msg = new Message(text, new XmlMessageFormatter()) {Label = _vm.Label};
            _helper.WriteXmlMessageOnQueue(mq, msg);

            _vm.HandleRequest();
            SetMessageBodyEditorVisibility();

            Assert.True(QueuesAreSet());
            Assert.That(GetVisibilitySettingName(), Is.EqualTo("messagebodyeditor"));
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
            VisibilityModel visibility = _vm.Visibility;
            if (visibility == null) return "undefined";
			if (visibility.IsSelectMessageVisible == true) return "selectmessage";
			if (visibility.IsEditorPanelVisible == true) return "messagebodyeditor";
		    if (visibility.IsErrorQueueVisible == true) return "errorqueue";
			return "unknown setting";
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
