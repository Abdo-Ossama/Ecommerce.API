using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description can't exceed 500 characters")]
        public string? Description { get; set; }

        public bool Status { get; set; }

        //public ICollection<Product> products { get; set; } = new List<Product>();
    }
}