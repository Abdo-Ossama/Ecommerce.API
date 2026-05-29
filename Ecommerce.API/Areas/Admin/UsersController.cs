using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Areas.Admin
{
    [Route("[area]/[controller]")]
    [ApiController]
    [Area(SD.ADMIN)]

    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet("")]
        public async Task<IActionResult> GetAll(int page = 1)
        {

            int pageSize = 5;
            var totalCount = _userManager.Users.Count() / pageSize;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (page <= 0)
                page = 1;
            var users = await _userManager.Users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new UsersResponse
            {
                Users = users,
                TotalPages = totalPages,
                pageSize = pageSize,
                CurrentPage = page

            }
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(string id)
        {

            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            return Ok(new UserResponse
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCtreateRequest userCtreateRequest)
        {
            var user = new ApplicationUser
            {
                UserName = userCtreateRequest.UserName,
                Email = userCtreateRequest.Email
            };

            var result = await _userManager.CreateAsync(user, userCtreateRequest.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            return Ok(new
            {
                Message = "User created successfully",
                UserId = user.Id
            });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UserEditRequest userEditRequest)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            user.UserName = userEditRequest.UserName??user.UserName;
            user.Email = userEditRequest.Email??user.Email;
            user.Fname = userEditRequest.Fname ?? user.Fname;
            user.Lname = userEditRequest.Lname ?? user.Lname;
            user.Address = userEditRequest.Address ?? user.Address;

          
            if (!string.IsNullOrEmpty(userEditRequest.NewPassword) &&
                !string.IsNullOrEmpty(userEditRequest.OldPassword))
            {
                var passwordResult = await _userManager.ChangePasswordAsync(
                    user,
                    userEditRequest.OldPassword,
                    userEditRequest.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    return BadRequest(passwordResult.Errors.Select(e => e.Description));
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id) {
            var user = await _userManager.FindByIdAsync(id);
            if ( user is null ) return NotFound();

           var result =  await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));

            }
            return NoContent();
        }
    }
};
   
    
   
  

