using Ecommerce.API.DTOs;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Areas.Admin
{
    [Area(SD.ADMIN)]
    [Route("[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMOPYLEE_ROLE}")]
    public class ProductsController : ControllerBase
    {
        private IRepository<Models.Product> _productRepository;
        private IRepository<Brand> _brandRepository;
        private IRepository<Category> _categoryRepository;
        private IProductSubImgsRepository _productSubImgsRepository;

        private readonly string[] _allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

        public ProductsController(
            IRepository<Models.Product> productRepository,
            IRepository<Brand> brandRepository,
            IRepository<Category> categoryRepository,
            IProductSubImgsRepository productSubImgsRepository)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
            _productSubImgsRepository = productSubImgsRepository;
        }

        private bool IsValidImage(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedExtensions.Contains(ext);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromQuery] ProductFilterRequest productFilterRequest, [FromQuery] int page = 1)
        {
            int pageSize = 5;

            if (page <= 0)
                page = 1;

            var productQuery = _productRepository.GetQuery(
                include: [e => e.Brand, e => e.Category],
                tracked: false
            );

            if (productFilterRequest.ProductName is not null)
                productQuery = productQuery.Where(e => e.Name.Contains(productFilterRequest.ProductName));

            if (productFilterRequest.MinPrice is not null)
                productQuery = productQuery.Where(e => e.Price >= productFilterRequest.MinPrice);

            if (productFilterRequest.MaxPrice is not null)
                productQuery = productQuery.Where(e => e.Price <= productFilterRequest.MaxPrice);

            if (productFilterRequest.brandId is not null)
                productQuery = productQuery.Where(e => e.BrandId == productFilterRequest.brandId);

            if (productFilterRequest.categoryId is not null)
                productQuery = productQuery.Where(e => e.CategoryId == productFilterRequest.categoryId);

            var totalItems = await productQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            productQuery = productQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var products = await productQuery.ToListAsync();

            return Ok(new ProductsResponceFinalDTO
            {
                Data = products.Adapt<List<ProductsResponse>>(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest productCreateRequest)
        {
           
            var brand = await _brandRepository.GetOneAsync(e => e.Id == productCreateRequest.BrandId);
            if (brand is null) return NotFound();

            var category = await _categoryRepository.GetOneAsync(e => e.Id == productCreateRequest.CategoryId);
            if (category is null) return NotFound();

            var product = productCreateRequest.Adapt<Models.Product>();

            if (productCreateRequest.MainImg is not null && productCreateRequest.MainImg.Length > 0)
            {
                if (!IsValidImage(productCreateRequest.MainImg))
                    return BadRequest("Invalid main image extension. Allowed: jpg, jpeg, png, webp");

                var newFile =
                    Guid.NewGuid().ToString().Substring(0, 7) +
                    DateTime.UtcNow.ToString("yyyy-MM-dd") +
                    Path.GetExtension(productCreateRequest.MainImg.FileName);

                var filePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "img",
                    "products_img",
                    newFile
                );

                using (var stream = System.IO.File.Create(filePath))
                {
                    await productCreateRequest.MainImg.CopyToAsync(stream);
                }

                product.MainImg = newFile;
            }

            await _productRepository.CreateAsync(product);
            await _productRepository.CommitAsync();

            if (productCreateRequest.subImgs != null && productCreateRequest.subImgs.Any())
            {
                foreach (var item in productCreateRequest.subImgs)
                {
                    if (!IsValidImage(item))
                        return BadRequest("Invalid sub image extension. Allowed: jpg, jpeg, png, webp");

                    var newFile =
                        Guid.NewGuid().ToString().Substring(0, 7) +
                        DateTime.UtcNow.ToString("yyyy-MM-dd") +
                        Path.GetExtension(item.FileName);

                    var filePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "img",
                        "products_img",
                        "subImgs",
                        newFile
                    );

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await item.CopyToAsync(stream);
                    }

                    await _productSubImgsRepository.CreateAsync(new ProductSubImg()
                    {
                        ProductId = product.Id,
                        SumImg = newFile,
                    });
                }

                await _productSubImgsRepository.CommitAsync();
            }

            return Ok(new SuccessResponse<bool>
            {
                Msg = "Product Created Successfully",
                Data = true
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var product = await _productRepository.GetOneAsync(e => e.Id == id);
            if (product is null) return NotFound();

            var subImgs = await _productSubImgsRepository.GetAsync(e => e.ProductId == id);

            return Ok(new ProductsWithSubImgsResponse
            {
                Product = product,
                SubImgs = subImgs
            });
        }

        [HttpPut]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Update(int id , [FromForm] ProductEditRequest productEditRequest)
        {
  var productInDb = await _productRepository.GetOneAsync(
                e => e.Id == id,
                Tracked: false
            );

            if (productInDb is null)
                return NotFound();

            if (productEditRequest.MainImg is not null && productEditRequest.MainImg.Length > 0)
            {
                if (!IsValidImage(productEditRequest.MainImg))
                    return BadRequest("Invalid image extension");

                var newFile =
                    Guid.NewGuid().ToString().Substring(0, 7) +
                    DateTime.UtcNow.ToString("yyyy-MM-dd") +
                    Path.GetExtension(productEditRequest.MainImg.FileName);

                var filePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "img",
                    "products_img",
                    newFile
                );

                using (var stream = System.IO.File.Create(filePath))
                {
                    await productEditRequest.MainImg.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(productInDb.MainImg))
                {
                    var oldPhoto = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "img",
                        "products_img",
                        productInDb.MainImg
                    );

                    if (System.IO.File.Exists(oldPhoto))
                        System.IO.File.Delete(oldPhoto);
                }

                productInDb.MainImg = newFile;
            }
        
           
   productInDb.Name = productEditRequest.Name ?? productInDb.Name;

productInDb.Price = productEditRequest.Price ?? productInDb.Price;

productInDb.Quantity = productEditRequest.Quantity ?? productInDb.Quantity;

productInDb.Description = productEditRequest.Description ?? productInDb.Description;

productInDb.Discount = productEditRequest.Discount ?? productInDb.Discount;

productInDb.BrandId = productEditRequest.BrandId ?? productInDb.BrandId;

productInDb.CategoryId = productEditRequest.CategoryId ?? productInDb.CategoryId;

            _productRepository.Update(productInDb);
            await _productRepository.CommitAsync();

            if (productEditRequest.subImgs != null && productEditRequest.subImgs.Any())
            {
                var productsSubImgs = await _productSubImgsRepository.GetAsync(e => e.ProductId == productInDb.Id);

                foreach (var item in productsSubImgs)
                {
                    if (!string.IsNullOrEmpty(item.SumImg))
                    {
                        var oldPhotoPath = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            "img",
                            "products_img",
                            "subImgs",
                            item.SumImg
                        );

                        if (System.IO.File.Exists(oldPhotoPath))
                            System.IO.File.Delete(oldPhotoPath);
                    }
                }

                _productSubImgsRepository.DeleteRange(productsSubImgs);

                foreach (var file in productEditRequest.subImgs)
                {
                    if (!IsValidImage(file))
                        return BadRequest("Invalid sub image extension");

                    var newFile =
                        Guid.NewGuid().ToString().Substring(0, 7) +
                        DateTime.UtcNow.ToString("yyyy-MM-dd-") +
                        Path.GetExtension(file.FileName);

                    var filePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "img",
                        "products_img",
                        "subImgs",
                        newFile
                    );

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }

                    await _productSubImgsRepository.CreateAsync(new ProductSubImg
                    {
                        ProductId = id,
                        SumImg = newFile
                    });
                }

                await _productSubImgsRepository.CommitAsync();
            }

            return NoContent();
        }



        [HttpDelete("{id}/images/{imgId}")]
        public async Task<IActionResult> DeleteImg(int id, int imgId)
        {
            // Get image and ensure it belongs to the product
            var productImg = await _productSubImgsRepository
                .GetOneAsync(e => e.Id == imgId && e.ProductId == id);

            if (productImg == null)
                return NotFound();

            // Delete image from wwwroot
            var oldPhotoPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "img",
                "products_img",
                "subImgs",
                productImg.SumImg
            );

            if (System.IO.File.Exists(oldPhotoPath))
            {
                System.IO.File.Delete(oldPhotoPath);
            }

            // Delete from database
            _productSubImgsRepository.delete(productImg);

            await _productSubImgsRepository.CommitAsync();

            return Ok(new SuccessResponse<bool>
            {
                Msg = "Image deleted successfully",
                Data = true
            });
        }




        [HttpDelete("{id}")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Get Product
            var product = await _productRepository
                .GetOneAsync(e => e.Id == id);

            if (product == null)
                return NotFound();

            // Delete Main Image From wwwroot
            if (!string.IsNullOrEmpty(product.MainImg))
            {
                var oldPhoto = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "img",
                    "products_img",
                    product.MainImg
                );

                if (System.IO.File.Exists(oldPhoto))
                {
                    System.IO.File.Delete(oldPhoto);
                }
            }

            // Get Sub Images
            var productsSubImgs = await _productSubImgsRepository
                .GetAsync(e => e.ProductId == product.Id);

            // Delete Sub Images From wwwroot
            foreach (var item in productsSubImgs)
            {
                if (!string.IsNullOrEmpty(item.SumImg))
                {
                    var oldPhotoPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "img",
                        "products_img",
                        "subImgs",
                        item.SumImg
                    );

                    if (System.IO.File.Exists(oldPhotoPath))
                    {
                        System.IO.File.Delete(oldPhotoPath);
                    }
                }
            }

            // Delete Product From Database
            // Cascade Delete مش محتاج تحذف الـ SubImgs يدويًا
            _productRepository.delete(product);

            await _productRepository.CommitAsync();

            return NoContent();
        }




    }
}