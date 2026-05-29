namespace Ecommerce.API.DTOs.Responses
{
    public class ProductWithRelatedCategoriesResponse
    {
        public Models.Product Product { get; set; } = default!;
        public List<Models.Product> relatedProducts { get; set; } = default!;
        public List<Models.Product> productsWithSameName { get; set; } = default!;
        public List<Models.Product> productsWithSamePrices { get; set; } = default!;
    }
}
