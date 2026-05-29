using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace Ecommerce.API.Repositories.IRepositorires
{
    public interface IProductSubImgsRepository : IRepository<ProductSubImg>
    {
        void DeleteRange(List<ProductSubImg> productSubImgs);
      
    }
}
