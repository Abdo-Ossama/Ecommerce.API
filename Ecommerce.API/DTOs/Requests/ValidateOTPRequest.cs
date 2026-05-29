namespace Ecommerce.API.DTOs.Requests
{
    public class ValidateOTPRequest
    {
        public int Id { get; set; }
        public string OTP { get; set; } = string.Empty;
        public string ApplicationUserId { get; set; } = string.Empty; // Hidden Feild
    }
}
