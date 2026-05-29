using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.API.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 150 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description can't exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;

        public bool Status { get; set; }

        [Required(ErrorMessage = "Main image is required")]
        [Url(ErrorMessage = "Main image must be a valid URL")]
        public string MainImg { get; set; } = string.Empty;

        [Range(1, long.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public double Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity can't be negative")]
        public int Quantity { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
        public long Discount { get; set; }

        [Range(0, 5, ErrorMessage = "Rate must be between 0 and 5")]
        public double Rate { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        [Required(ErrorMessage = "Brand is required")]
        public int BrandId { get; set; }

        public Brand Brand { get; set; } = null!;

        public ICollection<ProductSubImg> ProductSubImgs { get; set; }
    = new List<ProductSubImg>();
    }
}