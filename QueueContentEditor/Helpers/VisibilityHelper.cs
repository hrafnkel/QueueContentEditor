using QueueContentEditor.Models;

namespace QueueContentEditor.Helpers
{
	public class VisibilityHelper : IVisibilityHelper
	{
		public VisibilityModel SetDefaultVisibility() => new VisibilityModel
		{
			IsInputQueueVisible = false,
			IsEditorPanelVisible = false,
			IsSelectMessageVisible = false,
			IsErrorQueueVisible = true
		};

		public VisibilityModel SetSelectMessageVisibility() => new VisibilityModel
		{
			IsInputQueueVisible = false,
			IsEditorPanelVisible = false,
			IsSelectMessageVisible = true,
			IsErrorQueueVisible = false
		};

		public VisibilityModel SetMessageBodyEditorVisibility() => new VisibilityModel
		{
			IsInputQueueVisible = false,
			IsEditorPanelVisible = true,
			IsSelectMessageVisible = false,
			IsErrorQueueVisible = false
		};

		public VisibilityModel SetMessageBodyEditorQueueVisibility() => new VisibilityModel
		{
			IsInputQueueVisible = true,
			IsEditorPanelVisible = false,
			IsSelectMessageVisible = false,
			IsErrorQueueVisible = false
		};
	}
}