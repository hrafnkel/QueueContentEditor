using System.Collections.Generic;

namespace QueueContentEditor.Models
{
	public class QueueEditorModel
	{
		public List<string> ErrorQueueNames { get; set; }
		public List<string> InputQueueNames { get; set; }
		public List<string> MessageLabels { get; set; }

		public QueueEditorModel()
		{
			MessageLabels = new List<string>();
			ErrorQueueNames = new List<string>();
			InputQueueNames = new List<string>();
		}
	}
}