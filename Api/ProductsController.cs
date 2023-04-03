using AutoMapper;
using LadyFirstShop.Data.Entities;
using LadyFirstShop.Data.Infrastructure;
using LadyFirstShop.Data.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LadyFirstShop.Api
{
    [Route("api/products")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _unitOfWork.Products.GetAll();
            return Ok(products);
        }
        [HttpGet("post")]
        public IActionResult AddProduct()
        {
            var product = new ProductViewModel
            {
                Name = "Son BlackRouge",
                Price = 2,
                ImportPrice = 1,
                Quantity = 1,
                Sold = 0,
                Description = "Description",
                CreatedDate = DateTime.Now,
                BrandId = 1,
            };
            Product mapProduct = _mapper.Map<Product>(product);
            _unitOfWork.Products.Add(mapProduct);
            _unitOfWork.Complete();
            return Ok();
        }
    }
}
