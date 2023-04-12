using LadyFirstShop.Data.Services.Brands;
using LadyFirstShop.Data.Services.Products;
using LadyFirstShop.Data.ViewModel;
using LadyFirstShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LadyFirstShop.Controllers
{
	
	public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
		private readonly IBrandService _brandService;
		public HomeController(ILogger<HomeController> logger, IProductService productService, IBrandService brandService)
        {
            _logger = logger;
            _productService = productService;
            _brandService = brandService;
        }
		[Route("trang-chu")]
		public async Task<ActionResult> Index()
        {
            // 12 sp
            // sp bán chạy nhất
            // sp mới nhất
            var products = await _productService.GetListProduct();
			ViewBag.BestSellerProducts = await _productService.BestSellerProducts();
            var brands = await _productService.GetListBrand();
            ViewBag.SaleProducts = await _productService.GetListSaleProduct();
			var homeVm = new HomeViewModel()
            {
                Products = products,
                Brands = brands
            };
			return View(homeVm);
        }

        private async Task<HomeViewModel> GetAllProduct()   // Hàm Gọi API trả về list product
        {
            string baseUrl = string.Format("{0}://{1}",
                       HttpContext.Request.Scheme, HttpContext.Request.Host);
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage res = await httpClient.GetAsync(baseUrl + "/api/products");
                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var  list = new HomeViewModel()
                    {
                        Products = res.Content.ReadAsAsync<List<ProductViewModel>>().Result
                    };
                    return list;

                }
                return null;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}