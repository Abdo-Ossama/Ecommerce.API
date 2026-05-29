namespace Ecommerce.API.DTOs.Responses
{
    public class SuccessResponse<T>
    {
        public bool Success { get; set; }
        public string? Msg { get; set; }
        public T? Data { get; set; }
    }
}
