
# EfCore.UnitOfWork
A library for Microsoft.EntityFrameworkCore to support repository, unit of work patterns, and multiple database with transaction support. The project is an initial fork from https://github.com/Arch/UnitOfWork/ in order to make the library easier to use.

## Doumentation

Full documentation is available [here](https://github.com/moattarwork/UnitOfWork/wiki)

## Quickly start

Install the unit of work package from nuget.org.

```shell
dotnet add package EfCore.UnitOfWork
```

```shell
Install-Package EfCore.UnitOfWork
```

Configure relevant services in DI container and start using it:

```csharp
public void ConfigureServices(IServiceCollection services)
{

    services
        .AddDbContext<QuickStartContext>(opt => opt.UseInMemoryDatabase())
        .AddUnitOfWork<QuickStartContext>();
}

public class ValuesController
{
    private readonly IUnitOfWork<QuickStartDbContext> _unitOfWork;

    public ValuesController(IUnitOfWork<QuickStartDbContext> unitOfWork) =>
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task AddNewPostForNewUserAsync()
    {
        var user = new User {UserName = "sample-user", Password = "password"};
        _unitOfWork.GetRepository<User>().Insert(user);

        var post = new Post
        {
            UserId = user.UserId,
            Content = "Some comments",
            PublishDate = DateTime.Now,
            Tags = new List<PostTag> {new PostTag {Label = "Interesting"}, new PostTag {Label = "Social"}}
        };
        _unitOfWork.GetRepository<Post>().Insert(post);

        await _unitOfWork.SaveChangesAsync();
    }        
    
    public async Task<IEnumerable<Post>> LoadPostsAndRelevantTagsForGivenUser(int userId) =>
        await _unitOfWork.GetRepository<Post>()
            .GetListAsync(p => p.UserId == userId, q => q.OrderBy(m => m.PublishDate).Include(m => m.Tags));
}

```
