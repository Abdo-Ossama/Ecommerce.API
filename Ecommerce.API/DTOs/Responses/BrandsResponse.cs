namespace Ecommerce.API.DTOs.Responses
{
    public class BrandsResponse
    {
        public List<Brand> brands { get; set; }
        public double totalPages { get; set; }
        public double currentPage { get; set; }
        public double pageSize { get; set; }
    }
}
