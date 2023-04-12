using LadyFirstShop.Data.Request;
using LadyFirstShop.Data.Services.Contacts;
using Microsoft.AspNetCore.Mvc;

namespace LadyFirstShop.Controllers
{
	public class ContactController : Controller
	{
		private readonly IContactService _contactService;
        public ContactController(IContactService contactService)
        {
			_contactService = contactService;
        }
		[Route("lien-he")]
		[HttpGet]
		public IActionResult Index()
		{
            return View();
		}
		[Route("lien-he")]
		[HttpPost]
		public async Task<IActionResult> Index(ContactRequest request)
		{
			if (!ModelState.IsValid)
			{
				if(request.Name != null)
				{
					ViewBag.Name = request.Name;
				}
				if (request.Email != null)
				{
					ViewBag.Email = request.Email;
				}
				if (request.Subject != null)
				{
					ViewBag.Subject = request.Subject;
				}
				if (request.Content != null)
				{
					ViewBag.Content = request.Content;
				}
				return View(request);
			}
			var result = await _contactService.Create(request);
			if (result)
			{
				// Gửi phản hồi thành công
				ViewBag.Success = "Gửi phản hồi thành công";
				return View();
			}

			return View();
		}
	}
}
