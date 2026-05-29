namespace Ecommerce.API.DTOs.Responses
{
    public class CartResponse
    {
        public IEnumerable<Cart> Carts { get; set; } = null!;

        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }

        public string? Query { get; set; }

        public string? Message { get; set; } = null!;
    }
}
