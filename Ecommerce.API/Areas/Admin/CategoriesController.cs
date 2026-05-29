using EcommerceProject.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Areas.Admin
{
    [Route("[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMOPYLEE_ROLE}")]
    [Area(SD.ADMIN)]
 
   public class CategoriesController : ControllerBase
        {
            //private ApplicationDbContext _context = new ApplicationDbContext();
            private IRepository<Category> _categoryRepository;

            public CategoriesController(IRepository<Category> categoryRepository)
            {
                _categoryRepository = categoryRepository;
            }

        [HttpGet("")]
            public async Task<IActionResult> GetAll(string categoryname, int page = 1)
            {

                
                var categoriesQuery = await _categoryRepository.GetAsync(Tracked: false);
                //Filter with Name..
                if (categoryname is not null)
                {
                   
                    categoriesQuery = await _categoryRepository.GetAsync(e => e.Name.Contains(categoryname));
                }


                // Pagination
                int currentPage = page;
                int pageSize = 5;
                // Total Pages

                {

                    double totalPages = Math.Ceiling((double)categoriesQuery.Count() / pageSize);
                    if (page  < 1)
                    {
                        page = 1;
                    }
                    var categories = categoriesQuery
                                .Skip((currentPage - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

                    return Ok(new CategoriesResponse
                    {
                        categories = categories,
                        totalPages = totalPages,
                        pageSize = pageSize

                    });
                }
            }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne([FromRoute] int id)
        {
            var category = await _categoryRepository.GetOneAsync(e => e.Id == id);

            if (category == null)
                return NotFound();

            return Ok(category);
        }


        [HttpPost]
            public async Task<IActionResult> Create(Category category)
            {


                await _categoryRepository.CreateAsync(category);
                await _categoryRepository.CommitAsync();
            return Ok(new SuccessResponse<object>
            {
                Msg = "Category Added Successfully ",
                Data = new { CategoryId = category.Id },

            });
            }

       
            [HttpPut]
            [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
            public async Task<IActionResult> Update(int id  ,Category category)
            {
            var categoryInDb = await _categoryRepository.GetOneAsync(e=>e.Id== id);
            if (categoryInDb is null) return NotFound();
            categoryInDb.Name = category.Name;
            categoryInDb.Status = category.Status;
            categoryInDb.Description = category.Description;
          
             
                await _categoryRepository.CommitAsync();

            return NoContent();
            }
            [HttpDelete("{id}")]
            [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
            public async Task<IActionResult> Delete(int id)
            {
 
                var category = await _categoryRepository.GetOneAsync(e => e.Id == id);

                if (category != null)
                {
                  _categoryRepository.delete(category);

                    await _categoryRepository.CommitAsync();
                }

            return Ok(new SuccessResponse<bool>
            {
                Msg = "Category Deleted Successfully ",
                Data = true
            });
            }
        }
    }

