using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LadyFirstShop.Areas.Admin.Controllers
{
	public class BaseController : Controller
	{
		public override void OnActionExecuted(ActionExecutedContext context)
		{
			// check sessions nếu null thì đưa về trang login
			var sessions = context.HttpContext.Session.GetString("Token");
			if (sessions == null)
			{
				context.Result = new RedirectToActionResult("Index", "Login", null);
			}
			base.OnActionExecuted(context);
		}
	}
}
