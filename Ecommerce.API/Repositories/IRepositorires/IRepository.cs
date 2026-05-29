using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ecommerce.API.Repositories.IRepositorires
{
    public interface IRepository <T> where T : class
    {
        Task<List<T>> GetAsync(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>[]? include = null, bool Tracked = true);
        
        Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>[]? include = null, bool Tracked = true);


        IQueryable<T> GetQuery(
        Expression<Func<T, bool>>? expression = null,
        Expression<Func<T, object>>[]? include = null,
        bool tracked = true);

        Task CreateAsync(T entity);
       
         void Update(T entity);


         void delete(T entity);
      
         Task<int> CommitAsync();
        


        }
    }

