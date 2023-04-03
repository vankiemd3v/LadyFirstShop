using LadyFirstShop.Data.Constants;
using LadyFirstShop.Data.Entities;
using LadyFirstShop.Data.Request;
using LadyFirstShop.Data.Request.Products;
using LadyFirstShop.Data.Services.Brands;
using LadyFirstShop.Data.Services.Products;
using LadyFirstShop.Data.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LadyFirstShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        public ProductController(IProductService productService, IBrandService brandService)
        {
            _productService = productService;
            _brandService = brandService;
        }
        public async Task<IActionResult> Index(int brandId, string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetAdminProductRequest()
            {
                BrandId = brandId,
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            ViewBag.Keyword = keyword;
            if (TempData["success"] != null)
            {
                ViewBag.SuccessMsg = TempData["success"];
            }
            ViewBag.CountProduct = await _brandService.CountProductByBrand();
            var data = await _productService.GetAllPagingProduct(request);
            return View(data);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var brands = await _brandService.GetAll();
            ViewBag.Brands = brands;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Brands = await _brandService.GetAll();
                return View(request);
            }    
            var create = await _productService.CreateProduct(request);
            if (create)
            {
                TempData["success"] = ProductConstant.CreateProductSuccess;
                return RedirectToAction("Index","Product");
            }
            ViewBag.Brands = await _brandService.GetAll();
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var brands = await _brandService.GetAll();
            ViewBag.Brands = brands;
            var product = await _productService.GetById(id, 0);
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UpdateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Brands = await _brandService.GetAll();
                var productVm = new ProductViewModel();
                productVm.Id = request.Id;
                productVm.Name = request.Name;
                productVm.Description = request.Description;
                productVm.ImportPrice = request.ImportPrice;
                productVm.Price = request.Price;
                productVm.Quantity = request.Quantity;
                productVm.BrandId = request.BrandId;
                return View(productVm);
            }
            var update = await _productService.UpdateProduct(request);
            if (update)
            {
                TempData["success"] = ProductConstant.UpdateProductSuccess;
                return RedirectToAction("Index", "Product");
            }
            ViewBag.Brands = await _brandService.GetAll();
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> CreateProductColor(int id)
        {
            var product = await _productService.GetById(id, 0);
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> CreateProductColor(CreateProductColorRequest request)
        {
             if (!ModelState.IsValid)
            {
                var getById = await _productService.GetById(request.Id, 0);
                return View(getById);
            }
            var create = await _productService.CreateProductColor(request);
            if (create)
            {
                ViewBag.SuccessMsg = ProductConstant.CreateProductColorSuccess;
            }
            else
            {
                ViewBag.FaildMsg = ProductConstant.CreateProductColorFaild;
            }
            var product = await _productService.GetById(request.Id, 0);
            return View(product);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var delete = await _productService.DeleteProduct(id);
            return RedirectToAction("Index", "Product");
            
        }
        [HttpGet]
        public async Task<IActionResult> DeleteProductColor(int id)
        {
            var delete = await _productService.DeleteProductColor(id);
            if (delete)
            {
                return Json(new { status = true });
            }
            return Json(new { status = false });
        }
    }
}
