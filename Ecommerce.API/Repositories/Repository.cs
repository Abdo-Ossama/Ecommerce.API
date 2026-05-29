using System.Linq.Expressions;
using Ecommerce.API.Repositories.IRepositorires;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Repositories
{
    public class Repository<T> :IRepository<T> where T : class 
    {
        protected ApplicationDbContext _context;
        private DbSet<T> dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
        }

   

        //Read .. 
        public async Task<List<T>> GetAsync(Expression<Func<T , bool>>?expression = null, Expression<Func<T, object>>[]?include=null, bool Tracked = true  )
        {
            var categories = dbSet.AsQueryable();
            if (expression is not null)
            {
                categories = categories.Where(expression); 
            }
            if (!Tracked)
            {
              categories = categories.AsNoTracking();
            }
            if (include is not null)
            {
                foreach (var item in include)
                {
                    categories = categories.Include(item);
                }
            }
           
            return await categories.ToListAsync();
        }
        public async Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>[]? include = null, bool Tracked = true)
        {
            return (await GetAsync(expression,include,Tracked)).FirstOrDefault();

                                 
        }



        public IQueryable<T> GetQuery(
    Expression<Func<T, bool>>? expression = null,
    Expression<Func<T, object>>[]? include = null,
    bool tracked = true)
        {
            var query = dbSet.AsQueryable();

            // فلتر أساسي (اختياري)
            if (expression is not null)
            {
                query = query.Where(expression);
            }

            // Tracking
            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            // Includes
            if (include is not null)
            {
                foreach (var item in include)
                {
                    query = query.Include(item);
                }
            }

            return query;
        }

        public async Task CreateAsync( T entity)
        {
           await dbSet.AddAsync(entity);
           
        }
        public void Update( T entity)
        {
            dbSet.Update(entity);
          
        }
        public void delete( T entity)
        {
            dbSet.Remove(entity);
            
        }
        public async Task<int> CommitAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                Console.WriteLine($"Error .. {ex.Message}");
                return 0;
            
            }
          

        }
    }
}
