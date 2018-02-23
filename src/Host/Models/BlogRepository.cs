using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.UnitOfWork;

namespace Host.Models
{
    public class BlogRepository<TContext> : Repository<Blog>, ICustomRepository<TContext>, IBlogRepository
       where TContext : DbContext
    {
        #region Constructor
        public BlogRepository(TContext dbContext) : base(dbContext)
        {
        }
        #endregion

        #region BlogAsync
        public async Task<Blog> BlogAsync(params object[] keyValues)
        {
            Blog blog = await _dbSet.FindAsync(keyValues);
            if (blog == null)
            {
                return null;
            }

            // no need of UnitOfWork<> with its inconveniences to load linked properties
            _dbContext.Entry(blog).Collection(c => c.Posts).Load();

            return blog;
        }
        #endregion
    }
}
