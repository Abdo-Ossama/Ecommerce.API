namespace Ecommerce.API.DTOs.Requests
{
    public class UserCtreateRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Fname { get; set; } = string.Empty;
        public string Lname { get; set; } = string.Empty;
        public string ? Address { get; set; }

        public string NewPassword { get; set; } = string.Empty;
    }
}
