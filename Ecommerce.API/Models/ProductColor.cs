namespace Ecommerce.API.Models
{
    public class ProductColor
    {
        public int Id { get; set; }
        public string Color { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;


    }
}
