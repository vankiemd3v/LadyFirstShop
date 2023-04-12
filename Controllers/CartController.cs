using LadyFirstShop.Data.Constants;
using LadyFirstShop.Data.Entities;
using LadyFirstShop.Data.Request;
using LadyFirstShop.Data.Services.Orders;
using LadyFirstShop.Data.Services.Products;
using LadyFirstShop.Data.ViewModel;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;
using System.Text.Json;
using System.Drawing;

namespace LadyFirstShop.Controllers
{
	public class CartController : Controller
	{
		private readonly IProductService _productService;
		private readonly IOrderService _orderService;
		private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;
		public CartController(IProductService productService, IOrderService orderService, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
		{
			_productService = productService;
			_orderService = orderService;
			_env = env;
		}
		[Route("gio-hang")]
		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}
		[Route("gio-hang")]
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
			order.ShipNote = request.ShipNote;
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
			if(result == -1)
			{
				//ViewBag.OutStock = "Sản phẩm đã hết hàng";
				return Json(new
				{
					outstock = false
				});
			}
			if (result != 0)
			{
				var getOrderById = await _orderService.GetById(result);
				var mail = new CreateOrderRequest()
				{
					Id = getOrderById.Id,
					ShipName = request.ShipName,
					ShipAddress = request.ShipAddress,
					ShipEmail = request.ShipEmail,
					ShipNote = request.ShipNote,
					ShipPhoneNumber = request.ShipPhoneNumber,
					TotalPayment = getOrderById.TotalPayment,
					OrderDetails = order.OrderDetails
				};
				await ConfigEmail(mail);
				HttpContext.Session.Remove(CartConstant.CartSession);
				TempData["OrderSuccess"] = "Đặt hàng thành công";
				return Json(new
				{
					status = true,
					id = result
				});
			}
			else
			{
				return Json(new
				{
					status = false
				});
			}
		}
		[Route("don-hang")]
		public async Task<IActionResult> CheckOrder(int? id, string? search)
		{
			if(id != null)
			{
				var order = await _orderService.GetById(id);
				if(order == null)
				{
					ViewBag.NotFoundOrder = "Không tìm thấy đơn hàng: "+ id +"";
				}
				ViewBag.InputOrderId = id;
				if (search == null)
				{
					ViewBag.SuccessMsg = TempData["OrderSuccess"];
					return View(order);
				}
				else // search
				{
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
		public async Task<bool> SendEmail(string mess, string? emailCustomer, string subject)
		{
			var email = new MimeMessage();
			email.From.Add(MailboxAddress.Parse("s2xbladeno1@gmail.com"));
			if (emailCustomer != null)
			{
				email.To.Add(MailboxAddress.Parse(emailCustomer));
			}
			else
			{
				email.To.Add(MailboxAddress.Parse("vankiemd3v@gmail.com"));
			}
			email.Subject = subject;
			email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
			{
				Text = mess
			};
			using var smtp = new MailKit.Net.Smtp.SmtpClient();
			smtp.Connect("smtp.office365.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
			smtp.Authenticate("s2xbladeno1@gmail.com", "VanKiem06041998");
			smtp.Send(email);
			smtp.Disconnect(true);
			return true;
		}
		public async Task<bool> ConfigEmail(CreateOrderRequest request)
		{
			// Gửi mail cho shop
			var mess = await System.IO.File.ReadAllTextAsync(_env.WebRootPath + "/html/order.html");
			mess = mess.Replace("{{Id}}", request.Id.ToString());
			mess = mess.Replace("{{Name}}", request.ShipName);
			mess = mess.Replace("{{PhoneNumber}}", request.ShipPhoneNumber);
			mess = mess.Replace("{{Email}}", request.ShipEmail);
			mess = mess.Replace("{{Address}}", request.ShipAddress);
			mess = mess.Replace("{{TotalPayment}}", request.TotalPayment.ToString("N0"));
			var listProductName = new List<ProductInMailViewModel>();
			foreach (var item in request.OrderDetails)
			{
				var product = await _productService.GetById(item.ProductId, item.ProductColorId);
				listProductName.Add(new ProductInMailViewModel()
				{
					Name = product.Name,
					Quantity = item.Quantity,
				});
			}
			var json = System.Text.Json.JsonSerializer.Serialize(listProductName);
			mess = mess.Replace("{{ListProductName}}", json);
			await SendEmail(mess, null, CartConstant.NewOrder);

			// Gửi mail cho Customer
			var messCus = await System.IO.File.ReadAllTextAsync(_env.WebRootPath + "/html/orderCustomer.html");
			mess = mess.Replace("{{Id}}", request.Id.ToString());
			messCus = messCus.Replace("{{Name}}", request.ShipName);
			messCus = messCus.Replace("{{PhoneNumber}}", request.ShipPhoneNumber);
			messCus = messCus.Replace("{{Email}}", request.ShipEmail);
			messCus = messCus.Replace("{{Address}}", request.ShipAddress);
			messCus = messCus.Replace("{{TotalPayment}}", request.TotalPayment.ToString("N0")); ;
			var products = new List<ProductInMailViewModel>();
			foreach (var item in request.OrderDetails)
			{
				var product = await _productService.GetById(item.ProductId, item.ProductColorId);
				products.Add(new ProductInMailViewModel()
				{
					Name = product.Name,
					Quantity = item.Quantity,
				});
			}
			var jsonCus = System.Text.Json.JsonSerializer.Serialize(products);
			mess = mess.Replace("{{ListProductName}}", jsonCus);
			await SendEmail(mess, request.ShipEmail, CartConstant.OrderSuccess);
			return true;
		}

	}
}
