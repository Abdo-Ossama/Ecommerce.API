using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.DTOs.Requests
{
    public class RegisterRequest
    {
        [Required]
        [Display(Name = "First Name")]
        public string Fname { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string Lname { get; set; } = string.Empty;

        [Required]
       public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;


        public string? Address { get; set; }
    }
}