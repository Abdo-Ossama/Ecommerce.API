using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Areas.Customer
{

    [Route("[area]/[controller]")]
    [ApiController]
    [Area(SD.CUSTOMER)]
    public class StoreController : ControllerBase
    {
        private readonly IRepository<Models.Product> _productRepository;
        public StoreController(IRepository<Models.Product> productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet("")]
        public IActionResult GetAll(int? categoryId)
        {

            var products = _productRepository.GetQuery(e => e.Discount > 50, include: [e => e.Category]);

            if (categoryId is not null)
            {
                products = products.Where(e => e.CategoryId == categoryId);
            }

            products = products.Skip(0).Take(8);

            return Ok(products);

        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            //var product = _context.products.SingleOrDefault(e => e.Id == id);
            var product = await _productRepository.GetOneAsync(e => e.Id == id);

            if (product == null) return NotFound();


            //var relatedProducts = _context.products.Where(e => e.CategoryId == product.CategoryId && e.Id != product.Id)
            //    .Skip(0)
            //    .Take(4);
          var relatedProducts = await _productRepository.GetAsync(e => e.CategoryId == product.CategoryId && e.Id != product.Id);

            var minPrice = (product.Price - (product.Price * 0.10));
            var maxPrice = (product.Price + (product.Price * 0.10));
           
            var productsWithSamePrices = await _productRepository.GetAsync(e => e.Price >= minPrice && e.Price <= maxPrice);
          var productsWithSameName = await _productRepository.GetAsync(e => e.Name.Contains(product.Name));

            return Ok(new ProductWithRelatedCategoriesResponse
            {

                Product = product,
                relatedProducts = relatedProducts.ToList(),
                productsWithSameName = productsWithSameName.ToList(),
                productsWithSamePrices = productsWithSamePrices.ToList()
            });
        }

    }
}
