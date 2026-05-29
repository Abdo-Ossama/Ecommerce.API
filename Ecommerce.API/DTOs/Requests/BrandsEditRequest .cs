using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.DTOs.Requests
{
    public class BrandsEditRequest
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(10)]
        [MinLength(3)]
        //[Length(3, 10)]
        public string Name { get; set; } = string.Empty;
        public IFormFile? Logo { get; set; }
        public bool Status { get; set; }
    }
}

