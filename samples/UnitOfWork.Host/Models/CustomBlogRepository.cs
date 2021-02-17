using Microsoft.EntityFrameworkCore;

namespace Host.Models
{
    public class CustomBlogRepository : Repository<Blog>, IRepository<Blog>
    {
        public CustomBlogRepository(BloggingContext dbContext) : base(dbContext)
        {

        }
    }
}
