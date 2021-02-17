using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using Arch.EntityFrameworkCore.UnitOfWork.Host.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Arch.EntityFrameworkCore.UnitOfWork.Host.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private ILogger<ValuesController> _logger;

        // 1. IRepositoryFactory used for readonly scenario;
        // 2. IUnitOfWork used for read/write scenario;
        // 3. IUnitOfWork<TContext> used for multiple databases scenario;
        public ValuesController(IUnitOfWork unitOfWork, ILogger<ValuesController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

            // seeding
            var repo = _unitOfWork.GetRepository<Blog>(hasCustomRepository: true);
            if (repo.Count() == 0)
            {
                repo.Insert(new Blog
                {
                    Id = 1,
                    Url = "/a/" + 1,
                    Title = $"a{1}",
                    Posts = new List<Post>{
                                new Post
                                {
                                    Id = 1,
                                    Title = "A",
                                    Content = "A's content",
                                    Comments = new List<Comment>
                                    {
                                        new Comment
                                        {
                                            Id = 1,
                                            Title = "A",
                                            Content = "A's content",
                                        },
                                        new Comment
                                        {
                                            Id = 2,
                                            Title = "b",
                                            Content = "b's content",
                                        },
                                        new Comment
                                        {
                                            Id = 3,
                                            Title = "c",
                                            Content = "c's content",
                                        }
                                    },
                                },
                                new Post
                                {
                                    Id = 2,
                                    Title = "B",
                                    Content = "B's content",
                                    Comments = new List<Comment>
                                    {
                                        new Comment
                                        {
                                            Id = 4,
                                            Title = "A",
                                            Content = "A's content",
                                        },
                                        new Comment
                                        {
                                            Id = 5,
                                            Title = "b",
                                            Content = "b's content",
                                        },
                                        new Comment
                                        {
                                            Id = 6,
                                            Title = "c",
                                            Content = "c's content",
                                        }
                                    },
                                },
                                new Post
                                {
                                    Id = 3,
                                    Title = "C",
                                    Content = "C's content",
                                    Comments = new List<Comment>
                                    {
                                        new Comment
                                        {
                                            Id = 7,
                                            Title = "A",
                                            Content = "A's content",
                                        },
                                        new Comment
                                        {
                                            Id = 8,
                                            Title = "b",
                                            Content = "b's content",
                                        },
                                        new Comment
                                        {
                                            Id = 9,
                                            Title = "c",
                                            Content = "c's content",
                                        }
                                    },
                                },
                                new Post
                                {
                                    Id = 4,
                                    Title = "D",
                                    Content = "D's content",
                                    Comments = new List<Comment>
                                    {
                                        new Comment
                                        {
                                            Id = 10,
                                            Title = "A",
                                            Content = "A's content",
                                        },
                                        new Comment
                                        {
                                            Id = 11,
                                            Title = "b",
                                            Content = "b's content",
                                        },
                                        new Comment
                                        {
                                            Id = 12,
                                            Title = "c",
                                            Content = "c's content",
                                        }
                                    },
                                }
                            },
                });
                _unitOfWork.SaveChanges();
            }
        }

        // GET api/values
        [HttpGet]
        public async Task<IList<Blog>> Get()
        {
            return await _unitOfWork.GetRepository<Blog>().GetAllAsync(include: source => source.Include(blog => blog.Posts).ThenInclude(post => post.Comments));
        }

        // GET api/values/Page/5/10
        [HttpGet("Page/{pageIndex}/{pageSize}")]
        public async Task<IPagedList<Blog>> Get(int pageIndex, int pageSize)
        {
            // projection
            var items = _unitOfWork.GetRepository<Blog>().GetPagedList(b => new { Name = b.Title, Link = b.Url });

            return await _unitOfWork.GetRepository<Blog>().GetPagedListAsync(pageIndex: pageIndex, pageSize: pageSize);
        }

        // GET api/values/Search/a1
        [HttpGet("Search/{term}")]
        public async Task<IPagedList<Blog>> Get(string term)
        {
            _logger.LogInformation("demo about first or default with include");

            var item = _unitOfWork.GetRepository<Blog>().GetFirstOrDefault(predicate: x => x.Title.Contains(term), include: source => source.Include(blog => blog.Posts).ThenInclude(post => post.Comments));

            _logger.LogInformation("demo about first or default without include");

            item = _unitOfWork.GetRepository<Blog>().GetFirstOrDefault(predicate: x => x.Title.Contains(term), orderBy: source => source.OrderByDescending(b => b.Id));

            _logger.LogInformation("demo about first or default with projection");

            var projection = _unitOfWork.GetRepository<Blog>().GetFirstOrDefault(b => new { Name = b.Title, Link = b.Url }, predicate: x => x.Title.Contains(term));

            return await _unitOfWork.GetRepository<Blog>().GetPagedListAsync(predicate: x => x.Title.Contains(term));
        }

        // GET api/values/4
        [HttpGet("{id}")]
        public async Task<Blog> Get(int id)
        {
            return await _unitOfWork.GetRepository<Blog>().FindAsync(id);
        }

        // POST api/values
        [HttpPost]
        public async Task Post([FromBody]Blog value)
        {
            var repo = _unitOfWork.GetRepository<Blog>();
            repo.Insert(value);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}