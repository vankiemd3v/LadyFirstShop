using LadyFirstShop.Data.Constants;
using LadyFirstShop.Data.Entities;
using LadyFirstShop.Data.Request;
using LadyFirstShop.Data.Services.Orders;
using LadyFirstShop.Data.Services.Products;
using LadyFirstShop.Data.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LadyFirstShop.Controllers
{
	public class CartController : Controller
	{
		private readonly IProductService _productService;
		private readonly IOrderService _orderService;
		public CartController(IProductService productService, IOrderService orderService)
		{
			_productService = productService;
			_orderService = orderService;
		}
		public IActionResult Index()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Index(OrderRequest request)
		{
			if (!ModelState.IsValid)
			{
				if(request.ShipName != null)
				{
					ViewBag.ShipName = request.ShipName;
				}
				if (request.ShipEmail != null)
				{
					ViewBag.ShipEmail = request.ShipEmail;
				}
				if (request.ShipPhoneNumber != null)
				{
					ViewBag.ShipPhoneNumber = request.ShipPhoneNumber;
				}
				if (request.ShipAddress != null)
				{
					ViewBag.ShipAddress = request.ShipAddress;
				}
				return View(request);
			}
			var session = HttpContext.Session.GetString(CartConstant.CartSession);
			if (session == null)
			{
				return Json(new
				{
					cart = false
				});
			}
			List<CartItemViewModel> currentCart = new List<CartItemViewModel>();
			if (session != null)
				currentCart = JsonConvert.DeserializeObject<List<CartItemViewModel>>(session);
			var order = new CreateOrderRequest();
			order.ShipAddress = request.ShipAddress;
			order.ShipPhoneNumber = request.ShipPhoneNumber;
			order.ShipEmail = request.ShipEmail;
			order.ShipName = request.ShipName;
			order.Status = CartConstant.Inprogess;
			var products = new List<OrderDetailRequest>();
			foreach (var item in currentCart)
			{
				order.TotalPayment += (item.Price * item.Quantity);
				products.Add(new OrderDetailRequest()
				{
					ProductColorId = item.ColorId,
					ProductId = item.ProductId,
					Price = item.Price,
					Quantity = item.Quantity,
				});
			}
			order.OrderDetails = products;
			var result = await _orderService.CreateOrder(order);
			//var sendEmail = await _orderApiClient.SendEmail(request);
			if (result != 0)
			{
				HttpContext.Session.Remove(CartConstant.CartSession);
				TempData["OrderSuccess"] = "Đặt hàng thành công";
				return RedirectToAction("CheckOrder", new { id = result });
			}
			else
			{
				return Json(new
				{
					status = false
				});
			}
		}
		public async Task<IActionResult> CheckOrder(int id, string? search)
		{
			if(id != null)
			{
				var order = await _orderService.GetById(id);
				ViewBag.InputOrderId = id;
				if (search == null)
				{
					ViewBag.SuccessMsg = TempData["OrderSuccess"];
					return View(order);
				}
				else // search
				{
					ViewBag.NotFoundOrder = "Không tìm thấy đơn hàng";
					return Json(new { status = true });
				}
			}
			else // not found order
			{
				return View();
			}
		}
		[HttpPost]
		public async Task<IActionResult> AddToCart(int id, int colorId, int quantity)
		{
			var session = HttpContext.Session.GetString(CartConstant.CartSession);
			List<CartItemViewModel> currentCart = new List<CartItemViewModel>();
			if (session != null)
				currentCart = JsonConvert.DeserializeObject<List<CartItemViewModel>>(session);
			if (currentCart.Any(x => x.ProductId == id && x.ColorId == colorId))
			{
				var item = currentCart.Where(x => x.ProductId == id && x.ColorId == colorId).SingleOrDefault();
				item.Quantity += quantity;
			}
			else
			{
				var product = await _productService.GetById(id, colorId);
				var cartItem = new CartItemViewModel()
				{
					ProductId = id,
					Image = product.Image,
					ColorId = colorId,
					Name = product.Name,
					Quantity = quantity,
					Price = product.Price,
                    ColorName = product.ColorName,
					QuantityMax = product.QuantityChild
				};
				currentCart.Add(cartItem);
			}
			HttpContext.Session.SetString(CartConstant.CartSession, JsonConvert.SerializeObject(currentCart));
			return Ok();
		}
		[HttpPost]
        public IActionResult UpdateCart(int id, int quantity, int colorId)
        {
            var session = HttpContext.Session.GetString(CartConstant.CartSession);
            List<CartItemViewModel> currentCart = new List<CartItemViewModel>();
            if (session != null)
                currentCart = JsonConvert.DeserializeObject<List<CartItemViewModel>>(session);

            foreach (var item in currentCart)
            {
                if (item.ProductId == id && item.ColorId == colorId)
                {
                    if (quantity <= 0)
                    {
                        currentCart.Remove(item);
                        break;
                    }
                    item.Quantity = quantity;
                }
            }

            HttpContext.Session.SetString(CartConstant.CartSession, JsonConvert.SerializeObject(currentCart));
            return Ok(currentCart);
        }
        public async Task<IActionResult> GetCartItem()
		{
			var session = HttpContext.Session.GetString(CartConstant.CartSession);
			List<CartItemViewModel> currentCart = new List<CartItemViewModel>();
			if (session != null)
				currentCart = JsonConvert.DeserializeObject<List<CartItemViewModel>>(session);
			return Ok(currentCart);
		}

	}
}
