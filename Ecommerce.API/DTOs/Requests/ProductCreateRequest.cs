namespace Ecommerce.API.DTOs.Requests
{
    public class ProductCreateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public long Discount { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }

        public IFormFile MainImg { get; set; }
        public List<IFormFile> subImgs { get; set; }
    }
}
