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
		public async Task<IActionResult> Detail(int id, int colorId) 
		{
			var productDetail = await _productService.GetById(id, colorId);
			ViewBag.RelateProducts = await _productService.RelateProducts(id,productDetail.BrandId);
			return View(productDetail);
		}
	}
}
