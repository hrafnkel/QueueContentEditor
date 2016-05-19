using System.Collections.Generic;
using Repository.Queue.Models;

public interface IQueueWrapper<T>
{
	ReceiveResponse<T> Receive();
	List<T> GetAllMessagePayloads();
	ReceiveResponse<T> ReceiveTransactional();
	bool Send(T payload, string label);
	bool SendTransactional(T payload, string lablel);
	bool SendBatchTransactional(List<T> payloads, string lablel);
	List<T> PeekAllMessagePayloads(int timeoutSeconds);
	ReceiveResponse<T> ReceiveById(string messageId);
	int GetQueueDepth();
	List<ReceiveResponse<T>> PeekAllMessages(int timeoutSeconds);
	void DeleteAllMessagesFromQueue();
}
