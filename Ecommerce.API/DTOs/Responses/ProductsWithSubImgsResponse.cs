namespace Ecommerce.API.DTOs.Responses
{
    public class ProductsWithSubImgsResponse
    {
        public Models.Product? Product { get; set; }
        public IEnumerable<ProductSubImg>? SubImgs { get; set; }
    }
}
