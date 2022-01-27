using System.Collections.Generic;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork.Tests.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Arch.EntityFrameworkCore.UnitOfWork.Tests
{
    public class IRepositoryGetPagedListTest
    {
        private static readonly InMemoryContext Db;

        static IRepositoryGetPagedListTest()
        {
            Db = new InMemoryContext();

            Db.AddRange(TestCountries);
            Db.AddRange(TestCities);
            Db.AddRange(TestTowns);

            Db.SaveChanges();
        }

        [Fact]
        public void GetPagedList()
        {
            var repository = new Repository<City>(Db);

            var page = repository.GetPagedList(predicate: t => t.Name == "C", include: source => source.Include(t => t.Country), pageSize: 1);

            Assert.Equal(1, page.Items.Count);
            Assert.NotNull(page.Items[0].Country);

            Assert.Equal(page.Items[0].CountryId, page.Items[0].Country.Id);
            Assert.Equal("A", page.Items[0].Country.Name);
            Assert.Equal(1, page.Items[0].Country.Id);
        }

        [Fact]
        public async Task GetPagedListAsync()
        {
            var repository = new Repository<City>(Db);

            var page = await repository.GetPagedListAsync(predicate: t => t.Name == "C", include: source => source.Include(t => t.Country), pageSize: 1);

            Assert.Equal(1, page.Items.Count);
            Assert.NotNull(page.Items[0].Country);

            Assert.Equal(page.Items[0].CountryId, page.Items[0].Country.Id);
            Assert.Equal("A", page.Items[0].Country.Name);
            Assert.Equal(1, page.Items[0].Country.Id);
        }

        [Fact]
        public async Task GetPagedListWithIncludingMultipleLevelsAsync()
        {
            var repository = new Repository<Country>(Db);

            var page = await repository.GetPagedListAsync(predicate: t => t.Name == "A", include: country => country.Include(c => c.Cities).ThenInclude(city => city.Towns), pageSize: 1);

            Assert.Equal(1, page.Items.Count);
            Assert.NotNull(page.Items[0].Cities);

            Assert.NotNull(page.Items[0].Cities[0].Towns);
        }

        [Fact]
        public void GetPagedListWithoutInclude()
        {
            var repository = new Repository<City>(Db);

            var page = repository.GetPagedList(pageIndex: 0, pageSize: 1);

            Assert.Equal(1, page.Items.Count);
            Assert.Null(page.Items[0].Country);
        }

        private static IEnumerable<Country> TestCountries => new List<Country>
        {
            new() {Id = 1, Name = "A"},
            new() {Id = 2, Name = "B"}
        };

        private static IEnumerable<City> TestCities => new List<City>
        {
            new() { Id = 1, Name = "A", CountryId = 1},
            new() { Id = 2, Name = "B", CountryId = 2},
            new() { Id = 3, Name = "C", CountryId = 1},
            new() { Id = 4, Name = "D", CountryId = 2},
            new() { Id = 5, Name = "E", CountryId = 1},
            new() { Id = 6, Name = "F", CountryId = 2},
        };

        private static IEnumerable<Town> TestTowns => new List<Town>
        {
            new() { Id = 1, Name="A", CityId = 1 },
            new() { Id = 2, Name="B", CityId = 2 },
            new() { Id = 3, Name="C", CityId = 3 },
            new() { Id = 4, Name="D", CityId = 4 },
            new() { Id = 5, Name="E", CityId = 5 },
            new() { Id = 6, Name="F", CityId = 6 },
        };
    }
}
