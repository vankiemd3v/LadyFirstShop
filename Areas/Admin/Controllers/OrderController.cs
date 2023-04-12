using LadyFirstShop.Data.Constants;
using LadyFirstShop.Data.Entities;
using LadyFirstShop.Data.Request;
using LadyFirstShop.Data.Request.Products;
using LadyFirstShop.Data.Services.Orders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LadyFirstShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public async Task<IActionResult> Index(string keyword, int pageIndex, int pageSize = 10)
        {
			ViewBag.FullName = HttpContext.Session.GetString("FullName");
			if (pageIndex == 0)
            {
                pageIndex = 1;
            }
            var request = new GetOrdersRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _orderService.GetAllOrders(request);
            return View(data);
        }
        public async Task<IActionResult> Update(int id)
        {
			ViewBag.FullName = HttpContext.Session.GetString("FullName");
			ViewBag.ListStatus = new List<string>()
            {
                "Chờ duyệt",
                "Đang chuẩn bị hàng",
                "Đang giao",
                "Hoàn thành",
                "Bị Hủy",
            };
            ViewBag.Result = TempData["success"];
            var order = await _orderService.GetById(id);
            return View(order);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, string status)
        {
			ViewBag.FullName = HttpContext.Session.GetString("FullName");
			var result = await _orderService.UpdateStatus(id, status);
            TempData["success"] = true;
            return Json(new
            {
                status = true
            });
        }
    }
}
