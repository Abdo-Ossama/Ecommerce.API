namespace Ecommerce.API.Models
{
    public class ApplicationUserOTP
    {
        public int Id { get; set; }
        public string OTP { get; set; } = string.Empty;
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiredAt { get; set; } = DateTime.UtcNow.AddMonths(12);
        public bool IsValid => ExpiredAt > DateTime.Now; // بدل م كل مرة هعمل Check على الExpiredAt

        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; } = null!; // معناها متقلقش عمره م هيجي ب null 
    }
}
