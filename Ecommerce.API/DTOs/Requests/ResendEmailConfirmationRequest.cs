namespace Ecommerce.API.DTOs.Requests
{
    public class ResendEmailConfirmationRequest
    {
        public int Id { get; set; }
        public string UsernameOrEmail { get; set; } = string.Empty; 
    }
}
