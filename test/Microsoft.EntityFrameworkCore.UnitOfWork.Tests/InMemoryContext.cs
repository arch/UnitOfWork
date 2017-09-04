using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.UnitOfWork.Tests.Entities;

namespace Microsoft.EntityFrameworkCore.UnitOfWork.Tests
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
