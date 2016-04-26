using QueueContentEditor.Models;

namespace QueueContentEditor.Helpers
{
	public interface IVisibilityHelper
	{
		VisibilityModel SetDefaultVisibility();
		VisibilityModel SetSelectMessageVisibility();
		VisibilityModel SetMessageBodyEditorVisibility();
		VisibilityModel SetMessageBodyEditorQueueVisibility();

	}
}