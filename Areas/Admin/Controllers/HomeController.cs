using Microsoft.AspNetCore.Mvc;

namespace LadyFirstShop.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class HomeController : BaseController
	{
		public IActionResult Index()
		{
			ViewBag.FullName = HttpContext.Session.GetString("FullName");
            return View();
		}
	}
}
