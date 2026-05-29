namespace Ecommerce.API.Models
{
    public class ProductSubImg
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string SumImg { get; set; } = string.Empty;
        public Product Product { get; set; } = default!;
    }
}
