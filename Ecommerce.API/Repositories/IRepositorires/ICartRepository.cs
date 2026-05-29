namespace Ecommerce.API.Repositories.IRepositorires
{
    public interface ICartRepository : IRepository<Cart>
    {
        void DeleteRange(IEnumerable<Cart> carts);
    }
}
