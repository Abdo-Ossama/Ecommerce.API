using Microsoft.AspNetCore.Identity;

namespace Ecommerce.API.Utilities
{
    public class DbIntializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public DbIntializer(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
       public async Task DbIntilizer()
        {
            await _roleManager.CreateAsync(new(SD.SUPER_ADMIN_ROLE));
            await _roleManager.CreateAsync(new(SD.ADMIN_ROLE));
            await _roleManager.CreateAsync(new(SD.EMOPYLEE_ROLE));
            await _roleManager.CreateAsync(new(SD.CUSTOMER_ROLE));

            // Create SuperAdmin
            await _userManager.CreateAsync(new ApplicationUser
            {
                Fname = "Super",
                Lname = "Admin",
                Email = "SuperAdmin@gmail.com",
                UserName = "SuperAdmin",
                EmailConfirmed = true
            }, "SuperAdmin@123");
            // Create Admin
            await _userManager.CreateAsync(new ApplicationUser
            {
                Fname = "Admin",
                Lname = "",
                Email = "Admin@gmail.com",
                UserName = "Admin",
                EmailConfirmed = true
            }, "Admin@123");
            //Add Empolyee 1
            await _userManager.CreateAsync(new ApplicationUser
            {
                Fname = "Empolyee",
                Lname = "1",
                Email = "Empolyee1@gmail.com",
                UserName = "Empolyee1",
                EmailConfirmed = true
            }, "Empolyee1@123");
            //Add Empolyee2
            await _userManager.CreateAsync(new ApplicationUser
            {
                Fname = "Empolyee",
                Lname = "2",
                Email = "Empolyee2@gmail.com",
                UserName = "Empolyee2",
                EmailConfirmed = true
            }, "Empolyee2@123");

            var user1 = await _userManager.FindByNameAsync("SuperAdmin");
            var user2 = await _userManager.FindByNameAsync("Empolyee1");
            var user3 = await _userManager.FindByNameAsync("Empolyee2");
            var user4 = await _userManager.FindByNameAsync("Admin");
            if (user1 is not null && user2 is not null && user3 is not null && user4 is not null)
            {
                await _userManager.AddToRoleAsync(user1, SD.SUPER_ADMIN_ROLE);
                await _userManager.AddToRoleAsync(user2, SD.EMOPYLEE_ROLE);
                await _userManager.AddToRoleAsync(user3, SD.EMOPYLEE_ROLE);
                await _userManager.AddToRoleAsync(user4, SD.ADMIN_ROLE);
            }

        }
    }
}
