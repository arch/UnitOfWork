using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Host.Models
{
    public interface IBlogRepository
    {
        Task<Blog> BlogAsync(params object[] keyValues);
    }
}
