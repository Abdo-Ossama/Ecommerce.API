namespace Ecommerce.API.DTOs.Responses
{
    public class UsersResponse
    {
        public List<ApplicationUser> Users { get; set; }
        public int TotalPages { get; set; }
        public int pageSize { get; set; }
        public int CurrentPage { get; set; }
    }
}
