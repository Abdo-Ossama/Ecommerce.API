namespace Ecommerce.API.DTOs.Responses
{
    public class ErrorResponse
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }
    }
}
