using QueueContentEditor.Models;
using System.Web.Mvc;
using QueueContentEditor.Helpers;
using Repository.Queue;

namespace QueueContentEditor.Controllers
{
    public class HomeController : Controller
    {
	    public ActionResult Index()
		{
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