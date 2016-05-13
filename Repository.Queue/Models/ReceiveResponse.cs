namespace Repository.Queue.Models
{
	public struct ReceiveResponse<T>
	{
		public bool TimeOut { get; private set; }
		public T Payload { get; private set; }

		public ReceiveResponse(bool timedOut, T payload)
		{
			TimeOut = timedOut;
			Payload = payload;
		}
	}
}