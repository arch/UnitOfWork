using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Host.Models
{
    public interface IBlogRepository : IRepository<Blog>
    {
        IQueryable<Blog> GetRequiresReview();
    }
    public class BlogRepository : Repository<Blog>, IBlogRepository
    {
        public BlogRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<Blog> GetRequiresReview()
        {
            return _dbSet.Where(x => !x.Reviewed.HasValue);
        }
    }
}