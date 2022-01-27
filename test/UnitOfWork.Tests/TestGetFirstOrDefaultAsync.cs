using System.Collections.Generic;
using System.Linq;
using Arch.EntityFrameworkCore.UnitOfWork.Tests.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Arch.EntityFrameworkCore.UnitOfWork.Tests
{
    public class TestGetFirstOrDefaultAsync
    {
        private static readonly InMemoryContext db;
        
        static TestGetFirstOrDefaultAsync()
        {
            db = new InMemoryContext();
            if (db.Countries.Any() == false)
            {
                db.AddRange(TestCountries);
                db.AddRange(TestCities);
                db.AddRange(TestTowns);
                db.SaveChanges();
            }
        }


        [Fact]
        public async void TestGetFirstOrDefaultAsyncGetsCorrectItem()
        {
            var repository = new Repository<City>(db);
            var city = await repository.GetFirstOrDefaultAsync(predicate: t => t.Name == "A");
            Assert.NotNull(city);
            Assert.Equal(1, city.Id);            
        }

        [Fact]
        public async void TestGetFirstOrDefaultAsyncReturnsNullValue()
        {
            var repository = new Repository<City>(db);
            var city = await repository.GetFirstOrDefaultAsync(predicate: t => t.Name == "Easy-E");
            Assert.Null(city);            
        }

        [Fact]
        public async void TestGetFirstOrDefaultAsyncCanInclude()
        {
            var repository = new Repository<City>(db);
            var city = await repository.GetFirstOrDefaultAsync(
                predicate: c => c.Name == "A",
                include: source => source.Include(t => t.Towns));
            Assert.NotNull(city);
            Assert.NotNull(city.Towns);
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
            new() { Id = 1, Name="TownA", CityId = 1 },
            new() { Id = 2, Name="TownB", CityId = 2 },
            new() { Id = 3, Name="TownC", CityId = 3 },
            new() { Id = 4, Name="TownD", CityId = 4 },
            new() { Id = 5, Name="TownE", CityId = 5 },
            new() { Id = 6, Name="TownF", CityId = 6 },
        };
    }
}
