
# UnitOfWork
A plugin for Microsoft.EntityFrameworkCore to support repository, unit of work patterns, and multiple database with distributed transaction supported.

## Support MySQL multiple databases/tables sharding

> In MySQL, physically, a schema is synonymous with a database. You can substitute the keyword SCHEMA instead of DATABASE in MySQL SQL syntax, for example using CREATE SCHEMA instead of CREATE DATABASE. Some other database products draw a distinction. For example, in the Oracle Database product, a schema represents only a part of a database: the tables and other objects owned by a single user.

So, for MySQL, the easy way to support this feature is to dynamically change the SCHEMA at the runtime.

After [v1.1.2](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.UnitOfWork/1.1.2) had support MySQL multiple databases/tables sharding in the same model in the same machine. For different machine, you can use DbContextFactory to dynamically create DbContext. 
You can use [Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql) to test this feature. @[PomeloFoundation](https://github.com/PomeloFoundation)

# Quickly start

## How to use UnitOfWork

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // use in memory for testing.
    services
        .AddDbContext<QuickStartContext>(opt => opt.UseInMemoryDatabase())
        .AddUnitOfWork<QuickStartContext>()
        .AddCustomRepository<Blog, CustomBlogRepository>();
}

private readonly IUnitOfWork _unitOfWork;

// 1. IRepositoryFactory used for readonly scenario;
// 2. IUnitOfWork used for read/write scenario;
// 3. IUnitOfWork<TContext> used for multiple databases scenario;
public ValuesController(IUnitOfWork unitOfWork)
{
    _unitOfWork = unitOfWork;

    // Change database only work for MySQL right now.
    unitOfWork.ChangeDatabase($"uow_db_{DateTime.Now.Year}");

    var userRepo = unitOfWork.GetRepository<User>();
    var postRepo = unitOfWork.GetRepository<Post>();

    var ym = DateTime.Now.ToString("yyyyMM");

    userRepo.ChangeTable($"user_{ym}");
    postRepo.ChangeTable($"post_{ym}");

    var user = new User
    {
        UserName = "rigofunc",
        Password = "password"
    };

    userRepo.Insert(user);

    var post = new Post
    {
        UserId = user.UserId,
        Content = "What a piece of junk!"
    };

    postRepo.Insert(post);

    unitOfWork.SaveChanges();

    var find = userRepo.Find(user.UserId);

    find.Password = "p@ssword";

    unitOfWork.SaveChanges();

    // projection

}
```

## Projection & Including

```csharp
// projection
var pagedList = _unitOfWork.GetRepository<Blog>().GetPagedList(b => new { Name = b.Title, Link = b.Url }, pageIndex: pageIndex, pageSize: pageSize);
var projection = _unitOfWork.GetRepository<Blog>().GetFirstOrDefault(b => new { Name = b.Title, Link = b.Url }, predicate: x => x.Title.Contains(term));

// including
[HttpGet]
public async Task<IPagedList<Blog>> Get()
{
    return await _unitOfWork.GetRepository<Blog>().GetPagedListAsync(include: source => source.Include(blog => blog.Posts).ThenInclude(post => post.Comments));
}
```
