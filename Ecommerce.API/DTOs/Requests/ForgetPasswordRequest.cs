namespace Ecommerce.API.DTOs.Requests
{
    public class ForgetPasswordRequest
    {
        public int Id { get; set; }
        public string UsernameOrEmail { get; set; } = string.Empty; 
    }
}
