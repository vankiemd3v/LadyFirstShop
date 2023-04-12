using LadyFirstShop.Data.Request.Products;
using LadyFirstShop.Data.Services.Products;
using Microsoft.AspNetCore.Mvc;

namespace LadyFirstShop.Controllers
{
	
	public class ProductController : Controller
	{
		private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
			_productService = productService;
        }
        public IActionResult Index()
		{
			return View();
		}
		[Route("chi-tiet-san-pham")]
		public async Task<IActionResult> Detail(int id, int colorId) 
		{
			var productDetail = await _productService.GetById(id, colorId);
			ViewBag.RelateProducts = await _productService.RelateProducts(id,productDetail.BrandId);
			ViewBag.BestSellProducts = await _productService.BestSellerProducts();
			return View(productDetail);
		}
		[Route("san-pham")]
		public async Task<IActionResult> ProductByBrand(int id, int pageIndex, string? keyword, int? min, int? max)
		{
			ViewBag.BrandId = id;
			ViewBag.Min = min;
			ViewBag.Max = max;
			if (pageIndex == 0)
				pageIndex = 1;
			if(min == null)
			{
				min = 0;
			}
			if (max == null)
			{
				max = 2000000;
			}
			var request = new GetProductsPagingRequest()
			{
				Keyword = keyword,
				BrandId = id,
				PageIndex = pageIndex,
				PageSize = 8,
				Min = min,
				Max = max
			};
			ViewBag.Keyword = keyword;
			ViewBag.Brand = await _productService.GetListBrand();
			var products = await _productService.GetListProductByBrandId(request);
			return View(products);
		}
	}
}
