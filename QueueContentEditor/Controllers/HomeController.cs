using QueueContentEditor.Models;
using System.Web.Mvc;
using QueueContentEditor.Helpers;
using Repository.Queue;

namespace QueueContentEditor.Controllers
{
    public class HomeController : Controller
    {
	    
	 //   private readonly IVisibilityHelper _visibilityHelper = new VisibilityHelper();
		//private static readonly IQueueRepository QueueRepository = new QueueRepository();
		//private readonly IQueueHelper _queueHelper = new QueueHelper(QueueRepository);


		public ActionResult Index()
		{
			//			var vm = new QueueEditorViewModel(_queueHelper, _visibilityHelper);
			var vm = new QueueEditorViewModel();
			vm.HandleRequest();
            return View(vm);
		}

	    [HttpPost]
		[ValidateInput(false)]
		public ActionResult Index(QueueEditorViewModel vm)
	    {
			vm.HandleRequest();
			ModelState.Clear();
			return View(vm);
	    }
	}
}