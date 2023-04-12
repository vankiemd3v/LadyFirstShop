using LadyFirstShop.Data.Request;
using LadyFirstShop.Data.Services.Contacts;
using Microsoft.AspNetCore.Mvc;

namespace LadyFirstShop.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ContactController : BaseController
	{
		private readonly IContactService _contactService;
        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }
		public async Task<IActionResult> Index(int pageIndex, int pageSize = 10)
		{
			ViewBag.FullName = HttpContext.Session.GetString("FullName");
			if (pageIndex == 0)
			{
				pageIndex = 1;
			}
			var data = await _contactService.GetAllContact(pageIndex, pageSize);
			return View(data);
		}
		public async Task<IActionResult> Detail(int id)
		{
			ViewBag.FullName = HttpContext.Session.GetString("FullName");
			var detail = await _contactService.GetContactById(id);
			return View(detail);
		}
	}
}
