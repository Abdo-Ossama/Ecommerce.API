using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

namespace Ecommerce.API.Areas.Admin
{
    [Route("[area]/[controller]")]
    [ApiController]
    [Area(SD.ADMIN)]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMOPYLEE_ROLE}")]
    public class BrandsController : ControllerBase
    {
        private IRepository<Brand> _brandRepository;

        public BrandsController(IRepository<Brand> categoryRepository)
        {
            _brandRepository = categoryRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll(string? brandname, int page = 1)
        {

            //var brandQuery = _context.Brands.AsNoTracking().AsQueryable();
            var brandQuery = await _brandRepository.GetAsync(Tracked: false);

            //Filter with Name..
            if (brandname is not null)
            {
                //brandQuery = _context.Brands.Where(e => e.Name.Contains(brandname));
                brandQuery = await _brandRepository.GetAsync(e => e.Name.Contains(brandname));
            }


            // Pagination
            int currentPage = page;
            int pageSize = 5;
            // Total Pages

            {

                double totalPages = Math.Ceiling((double)brandQuery.Count() / pageSize);
                if (page < 1)
                {
                    page = 1;
                }
                var brands = brandQuery
                            .Skip((currentPage - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                return Ok(new BrandsResponse
                {
                    brands = brands,
                    totalPages = totalPages,
                    pageSize = pageSize

                });
            }
        }

      
        [HttpPost]
        public async Task<IActionResult> Create(BrandsCreateRequest brandsRequest)
        {
            var brand = brandsRequest.Adapt<Brand>();
            if (brandsRequest.Logo is not null && brandsRequest.Logo.Length > 0)
            {
                var newFile = Guid.NewGuid().ToString().Substring(0, 7) + DateTime.UtcNow.ToString("yyyy - MM dd-") + Path.GetExtension(brandsRequest.Logo.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\brands_img", newFile);


                using (var stream = System.IO.File.Create(filePath))
                {
                    brandsRequest.Logo.CopyTo(stream);
                }

                brand.Logo = newFile;
            }

            await _brandRepository.CreateAsync(brand);
            await _brandRepository.CommitAsync();

            return Ok( brand  );
            

        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> GetOne([FromRoute] int id)
        {

          
            var brand = await _brandRepository.GetOneAsync(e => e.Id == id);
            if (brand == null) return NotFound();

            return Ok(brand);
        }



        [HttpPut]
        public async Task<IActionResult> Update(BrandsEditRequest brandeditrequest, CancellationToken cancellationToken = default)
        {

            var brand = await _brandRepository.GetOneAsync(e => e.Id == brandeditrequest.Id );

            if (brand is null)
                return NotFound();

            if (brandeditrequest.Logo is not null && brandeditrequest.Logo.Length > 0)
            {
                var newFile = Guid.NewGuid().ToString().Substring(0, 7) + DateTime.UtcNow.ToString("yyyy - MM dd-") + Path.GetExtension(brandeditrequest.Logo.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\brands_img", newFile);


                using (var stream = System.IO.File.Create(filePath))
                {
                    brandeditrequest.Logo.CopyTo(stream);
                }

                brand.Logo = newFile;
            }
            else
                brand.Logo = brand.Logo;

            brand.Name = brandeditrequest.Name;
            brand.Status = brandeditrequest.Status;

            await _brandRepository.CommitAsync();

            return Ok(new SuccessResponse<bool>
            {
              Msg = "Update Brand Successfully"
            });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ToggleStatus(int id, CancellationToken cancellationToken = default)
        {
           
            var brandInDB = await _brandRepository.GetOneAsync(e => e.Id == id);

            if (brandInDB is null)
                return NotFound();
            brandInDB.Status = !brandInDB.Status;

            await _brandRepository.CommitAsync();

            return NoContent();
        }

        //[HttpPut]
        //[Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        //public async Task<IActionResult> Edit(int id, BrandsEditRequest brandsEditRequest)
        //{


        //    var brandInDb = await _brandRepository.GetOneAsync(e => e.Id == id);
        //    if (brandInDb == null) return NotFound();

        //    if (brandsEditRequest.Logo is not null && brandsEditRequest.Logo.Length > 0)
        //    {

        //        if (brandInDb is null)
        //        {
        //            return NotFound();
        //        }
        //        var newFile = Guid.NewGuid().ToString().Substring(0, 7) + DateTime.UtcNow.ToString("yyyy - MM dd-") + Path.GetExtension(brandsEditRequest.Logo.FileName);
        //        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\brands_img", newFile);


        //        using (var stream = System.IO.File.Create(filePath))
        //        {
        //            brandsEditRequest.Logo.CopyTo(stream);
        //        }
        //        //Delete Old File 
        //        var oldPhoto = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\brands_img", brandInDb.Logo);


        //        if (System.IO.File.Exists(oldPhoto))
        //        {
        //            System.IO.File.Delete(oldPhoto);
        //        }
        //        brandInDb.Logo = newFile;
        //    }
        //    brandInDb.Name = brandsEditRequest.Name;
        //    brandInDb.Status = brandsEditRequest.Status;


        //    await _brandRepository.CommitAsync();

        //    return NoContent();
        //}
        [HttpDelete]

        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id)
        {
            //var brand = _context.Brands.Find(id);
            var brand = await _brandRepository.GetOneAsync(e => e.Id == id);

            if (brand is null)
            {
                return NotFound(); 
            }
            _brandRepository.delete(brand);
            await _brandRepository.CommitAsync();
            return NoContent();
        }
    }
}

