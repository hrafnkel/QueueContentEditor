using QueueContentEditor.Models;
using System.Web.Mvc;
using QueueContentEditor.Helpers;

namespace QueueContentEditor.Controllers
{
    public class HomeController : Controller
    {
	    private IQueueHelper _queueHelper;
	    private IVisibilityHelper _visibilityHelper;

	    public HomeController(IQueueHelper queueHelper, IVisibilityHelper visibilityHelper)
	    {
		    _queueHelper = queueHelper;
		    _visibilityHelper = visibilityHelper;
	    }

	    public ActionResult Index()
		{
			var vm = new QueueEditorViewModel(_queueHelper, _visibilityHelper);
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