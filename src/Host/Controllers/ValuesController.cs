using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Host.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Host.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        // 1. IRepositoryFactory used for readonly scenario;
        // 2. IUnitOfWork used for read/write scenario;
        // 3. IUnitOfWork<TContext> used for multiple databases scenario;

        public ValuesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            // seeding
            var repo = _unitOfWork.GetRepository<Blog>();
            if (repo.Count() == 0)
            {
                Enumerable.Range(1, 100)
                    .ToList()
                    .ForEach(x =>
                    {
                        repo.Insert(new Blog
                        {
                            BlogId = x,
                            Url = "/a/" + x,
                            Title = $"a{x}"
                        });
                    });
                _unitOfWork.SaveChanges();
            }
        }

        // GET api/values
        [HttpGet]
        public async Task<IPagedList<Blog>> Get()
        {
            var repo = _unitOfWork.GetRepository<Blog>();
            return await repo.GetPagedListAsync();
        }

        // GET api/values/Page/5/10
        [HttpGet("Page/{pageNumber}/{pageSize}")]
        public async Task<IEnumerable<Blog>> Get(int pageNumber, int pageSize)
        {
            var repo = _unitOfWork.GetRepository<Blog>();
            var value = await repo.GetPagedListAsync(null, null, null, pageNumber, pageSize);
            return value.Items;
        }

        // GET api/values/Search/a1
        [HttpGet("Search/{term}")]
        public async Task<IPagedList<Blog>> Get(string term)
        {
            var repo = _unitOfWork.GetRepository<Blog>();
            return await repo.GetPagedListAsync(x => x.Title.Contains(term));
        }

        // GET api/values/4
        [HttpGet("{id}")]
        public async Task<Blog> Get(int id)
        {
            var repo = _unitOfWork.GetRepository<Blog>();
            var value = await repo.FindAsync(id);
            return value;
        }

        // POST api/values
        [HttpPost]
        public async Task Post([FromBody]Blog value)
        {
            var repo = _unitOfWork.GetRepository<Blog>();
            repo.Insert(value);
            await _unitOfWork.SaveChangesAsync();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody]Blog value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}