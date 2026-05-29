namespace Ecommerce.API.DTOs.Responses
{
    public class ProductsResponceFinalDTO
    {
        public List< ProductsResponse >Data { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }
}
