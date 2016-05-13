using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using QueueContentEditor.Helpers;
using Repository.Queue;

namespace QueueContentEditor.Controllers
{
    public class ServiceController : Controller
    {
		private static readonly IQueueRepository QueueRepository = new QueueRepository();
		private readonly IQueueHelper _queueHelper = new QueueHelper(QueueRepository);

		// GET: Service
		public ActionResult Index()
        {
            return GetAllQueueNames();
        }

	    private ActionResult GetAllQueueNames()
	    {
		    List<MessageQueue> queues = _queueHelper.GetAllPrivateQueues();
		    var json = Content(ConvertToJsonThing(queues), "application/json");
		    return json;
	    }

	    private string ConvertToJsonThing(List<MessageQueue> queues)
	    {
		    List<Thing> things = new List<Thing>();
		    int i = 0;
		    foreach (MessageQueue messageQueue in queues)
		    {
			    Thing thing = new Thing
			    {
				    id = i++.ToString(),
				    text = messageQueue.Label
			    };
				things.Add(thing);
			}

		    return JsonConvert.SerializeObject(things);
	    }
    }

	class Thing
	{
		public string id { get; set; }
		public string text { get; set; }
	}
}