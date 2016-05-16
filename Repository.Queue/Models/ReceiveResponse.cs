namespace Repository.Queue.Models
{
	public struct ReceiveResponse<T>
	{
		public bool TimeOut { get; private set; }
		public T Payload { get; private set; }
		public string Label { get; set; }

		public ReceiveResponse(bool timedOut, T payload, string label)
		{
			TimeOut = timedOut;
			Payload = payload;
			Label = label;
		}
	}
}