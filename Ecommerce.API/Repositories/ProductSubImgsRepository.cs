using Ecommerce.API.Repositories.IRepositorires;

namespace Ecommerce.API.Repositories
{
    public class ProductSubImgsRepository
    : Repository<ProductSubImg>, IProductSubImgsRepository
    {
        public ProductSubImgsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void DeleteRange(List<ProductSubImg> productSubImgs)
        {
            _context.ProductSubImgs.RemoveRange(productSubImgs);
           
        }
    }
}
