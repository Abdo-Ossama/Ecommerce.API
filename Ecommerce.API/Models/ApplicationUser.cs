using Microsoft.AspNetCore.Identity;

namespace Ecommerce.API.Models
{
    public class ApplicationUser :IdentityUser
    {
        public string Fname { get; set; } = string.Empty;
        public string Lname { get; set; } = string.Empty;
        public string? Address  { get; set; }

    }
}

