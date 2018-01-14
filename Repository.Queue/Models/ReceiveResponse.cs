using System.Runtime.InteropServices;

namespace Repository.Queue.Models
{
	public struct ReceiveResponse<T>
	{
		public bool TimeOut { get; private set; }
		public T Payload { get; private set; }
		public string Label { get; set; }
		public string Id { get; set; }

		public ReceiveResponse(bool timedOut, T payload, string label, string id)
		{
			TimeOut = timedOut;
			Payload = payload;
			Label = label;
			Id = id;
		}
	}
}