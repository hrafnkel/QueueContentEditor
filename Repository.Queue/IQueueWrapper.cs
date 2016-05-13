using System.Collections.Generic;
using Repository.Queue.Models;

public interface IQueueWrapper<T>
{
	ReceiveResponse<T> Receive();
	List<T> GetAllMessagePayloads();
}
