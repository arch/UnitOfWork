# UnitOfWork
A plugin for Microsoft.EntityFrameworkCore to support repository, unit of work patterns, and multiple database with distributed transaction supported.

# Quickly start

## How to use UnitOfWork

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // use in memory for testing.
    services
        .AddDbContext<QuickStartContext>(opt => opt.UseInMemoryDatabase())
        .AddUnitOfWork<QuickStartContext>();
}

private readonly IUnitOfWork _unitOfWork;

// 1. IRepositoryFactory used for readonly scenario;
// 2. IUnitOfWork used for read/write scenario;
// 3. IUnitOfWork<TContext> used for multiple databases scenario;
public ValuesController(IUnitOfWork unitOfWork)
{
    _unitOfWork = unitOfWork;
}
```
