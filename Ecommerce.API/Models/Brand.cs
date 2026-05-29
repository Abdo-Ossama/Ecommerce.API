using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.Models
{
    public class Brand
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        public bool Status { get; set; }

        [Required(ErrorMessage = "Logo is required")]
        [Url(ErrorMessage = "Logo must be a valid URL")]
        public string Logo { get; set; } = string.Empty;

        //public ICollection<Product> products { get; set; } = new List<Product>();
    }
}