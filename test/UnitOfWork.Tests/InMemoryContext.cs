using Arch.EntityFrameworkCore.UnitOfWork.Tests.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arch.EntityFrameworkCore.UnitOfWork.Tests
{
    public class InMemoryContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("test");
        }
    }
}
