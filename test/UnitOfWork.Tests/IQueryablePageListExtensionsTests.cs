using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using Arch.EntityFrameworkCore.UnitOfWork.Tests;
using Arch.EntityFrameworkCore.UnitOfWork.Tests.Entities;
using Xunit;

namespace Arch.EntityFrameworkCore.UnitOfWork.Tests
{
    public class IQueryablePageListExtensionsTests
    {
        [Fact]
        public async Task ToPagedListAsyncTest()
        {
            await using var db = new InMemoryContext();
            var testItems = TestItems();
            await db.AddRangeAsync(testItems);
            await db.SaveChangesAsync();

            var items = db.Customers.Where(t => t.Age > 1);

            var page = await items.ToPagedListAsync(1, 2);
            Assert.NotNull(page);

            Assert.Equal(4, page.TotalCount);
            Assert.Equal(2, page.Items.Count);
            Assert.Equal("E", page.Items[0].Name);

            page = await items.ToPagedListAsync(0, 2);
            Assert.NotNull(page);
            Assert.Equal(4, page.TotalCount);
            Assert.Equal(2, page.Items.Count);
            Assert.Equal("C", page.Items[0].Name);
        }

        private static IEnumerable<Customer> TestItems() => new List<Customer>()
            {
                new(){Name="A", Age=1},
                new(){Name="B", Age=1},
                new(){Name="C", Age=2},
                new(){Name="D", Age=3},
                new(){Name="E", Age=4},
                new(){Name="F", Age=5},
            };
    }
}
