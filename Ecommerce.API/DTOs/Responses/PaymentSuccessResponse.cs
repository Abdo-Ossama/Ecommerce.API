namespace Ecommerce.API.DTOs.Responses
{
    public class PaymentSuccessResponse
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalPrice { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
