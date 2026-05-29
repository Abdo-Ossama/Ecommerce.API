namespace Ecommerce.API.DTOs.Requests
{
    public class ReviewCreateRequest
    {
        public int ProductId { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;

        public string Comment { get; set; } = string.Empty;
        public int Rate { get; set; }
        public IFormFile Img { get; set; } = null!;
    }
}
