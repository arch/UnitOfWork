using Arch.EntityFrameworkCore.UnitOfWork.Tests.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arch.EntityFrameworkCore.UnitOfWork.Tests
{
    public class InMemoryContext : DbContext
    {
        public DbSet<Country> Countries => Set<Country>();
        public DbSet<Customer> Customers => Set<Customer>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseInMemoryDatabase("test");
    }
}
