using NUnit.Framework;
using QueueContentEditor.Helpers;
using QueueContentEditor.Models;

namespace QueueContentEditor.Tests
{
	[TestFixture]
	public class VisibilityHelperTests
	{
		IVisibilityHelper _visibilityHelper;

		[SetUp]
		public void SetUp()
		{
			_visibilityHelper = new VisibilityHelper();
		}

		[Test]
		public void Default_Visibility_Sets_Only_Error_Queue_Visible()
		{
			VisibilityModel defaultModel = _visibilityHelper.SetDefaultVisibility();
			Assert.False(defaultModel.IsInputQueueVisible);
			Assert.False(defaultModel.IsEditorPanelVisible);
			Assert.False(defaultModel.IsSelectMessageVisible);
			Assert.True(defaultModel.IsErrorQueueVisible);
		}

		[Test]
		public void SelectMessage_Visibility_Sets_Only_SelectMessage_Visible()
		{
			VisibilityModel defaultModel = _visibilityHelper.SetSelectMessageVisibility();
			Assert.False(defaultModel.IsInputQueueVisible);
			Assert.False(defaultModel.IsEditorPanelVisible);
			Assert.True(defaultModel.IsSelectMessageVisible);
			Assert.False(defaultModel.IsErrorQueueVisible);
		}

		[Test]
		public void Editor_Visibility_Sets_Only_BodyEditor_Visible()
		{
			VisibilityModel defaultModel = _visibilityHelper.SetMessageBodyEditorVisibility();
			Assert.False(defaultModel.IsInputQueueVisible);
			Assert.True(defaultModel.IsEditorPanelVisible);
			Assert.False(defaultModel.IsSelectMessageVisible);
			Assert.False(defaultModel.IsErrorQueueVisible);
		}

		[Test]
		public void EditorQueue_Visibility_Sets_BodyEditorQueue_Visible()
		{
			VisibilityModel defaultModel = _visibilityHelper.SetMessageBodyEditorQueueVisibility();
			Assert.True(defaultModel.IsInputQueueVisible);
			Assert.False(defaultModel.IsEditorPanelVisible);
			Assert.False(defaultModel.IsSelectMessageVisible);
			Assert.False(defaultModel.IsErrorQueueVisible);
		}
	}
}
