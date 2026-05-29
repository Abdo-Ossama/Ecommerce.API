namespace Ecommerce.API.DTOs.Requests
{
    public class ReviewUpdateRequest
    {
        public string Comment { get; set; } = string.Empty;
        public int Rate { get; set; }
        public IFormFile? Img { get; set; }
    }
}
