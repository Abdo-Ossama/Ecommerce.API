namespace Ecommerce.API.DTOs
{
    public class ProductFilterRequest
    {

        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public string ? ProductName { get; set; } = null!;
        public int? brandId { get; set; }
        public int ?categoryId { get; set; }

    }
}
