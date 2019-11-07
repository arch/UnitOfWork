using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using UnitOfWork.Tests.Entities;

namespace UnitOfWork.Tests
{
    public class InMemoryDbContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var name = $"{typeof(InMemoryDbContext).Name}_{Guid.NewGuid()}";

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            optionsBuilder.UseInMemoryDatabase(name)
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .UseInternalServiceProvider(serviceProvider);
        }
    }
}