using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.UnitOfWork.Tests.Entities;
using Xunit;

namespace Microsoft.EntityFrameworkCore.UnitOfWork.Tests
{
    public class IRepositoryGetPagedListTest
    {
        private static readonly InMemoryContext db;

        static IRepositoryGetPagedListTest()
        {
            db = new InMemoryContext();

            db.AddRange(TestCountries);
            db.AddRange(TestCities);

            db.SaveChanges();
        }

        [Fact]
        public void GetPagedList()
        {
            var repository = new Repository<City>(db);

            var page = repository.GetPagedList(t => t.Name == "C", include: source => source.Include(t => t.Country), pageSize: 1);

            Assert.Equal(1, page.Items.Count);
            Assert.NotNull(page.Items[0].Country);

            Assert.Equal(page.Items[0].CountryId, page.Items[0].Country.Id);
            Assert.Equal("A", page.Items[0].Country.Name);
            Assert.Equal(1, page.Items[0].Country.Id);
        }

        [Fact]
        public async Task GetPagedListAsync()
        {
            var repository = new Repository<City>(db);

            var page = await repository.GetPagedListAsync(t => t.Name == "C", include: source => source.Include(t => t.Country), pageSize: 1);

            Assert.Equal(1, page.Items.Count);
            Assert.NotNull(page.Items[0].Country);

            Assert.Equal(page.Items[0].CountryId, page.Items[0].Country.Id);
            Assert.Equal("A", page.Items[0].Country.Name);
            Assert.Equal(1, page.Items[0].Country.Id);
        }

        [Fact]
        public void GetPagedListWithoutInclude()
        {
            var repository = new Repository<City>(db);

            var page = repository.GetPagedList(pageIndex: 0, pageSize: 1);

            Assert.Equal(1, page.Items.Count);
            Assert.Null(page.Items[0].Country);
        }

        protected static List<Country> TestCountries => new List<Country>
        {
            new Country {Id = 1, Name = "A"},
            new Country {Id = 2, Name = "B"}
        };

        public static List<City> TestCities => new List<City>
        {
            new City {Name = "A", CountryId = 1},
            new City {Name = "B", CountryId = 2},
            new City {Name = "C", CountryId = 1},
            new City {Name = "D", CountryId = 2},
            new City {Name = "E", CountryId = 1},
            new City {Name = "F", CountryId = 2},
        };
    }
}
