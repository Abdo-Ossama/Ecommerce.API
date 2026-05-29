using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.DTOs.Requests
{
    public class BrandsCreateRequest
    {
       
        public string Name { get; set; } = string.Empty;

        public bool Status { get; set; }


        public IFormFile Logo { get; set; } = null!;
    }
}

