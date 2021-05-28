using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Arch.EntityFrameworkCore.UnitOfWork.Tests.Entities;
using Xunit;

namespace Arch.EntityFrameworkCore.UnitOfWork.Tests
{
    public class TestPredicate
    {
        private static readonly InMemoryContext db;

        static TestPredicate()
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
        public async void TestGetFirstOrDefaultAsync()
        {
            var repository = new Repository<City>(db);
            var exp = PredicateWrap.Op<City>(t => t.Name == "A");
            var city = await repository.GetFirstOrDefaultAsync(predicate: exp);
            Assert.NotNull(city);
            Assert.Equal(1, city.Id);

            var predicate = PredicateBuilder<City>.Instance;
            var exp1 = predicate.Custom(t => t.Id == 3);
            var city1 = await repository.GetFirstOrDefaultAsync(predicate: exp1);
            Assert.NotNull(city1);
            Assert.Equal(3, city1.Id);
        }

        [Fact]
        public async void TestGetAll()
        {
            var repository = new Repository<City>(db);
            var predicate = PredicateBuilder<City>.Instance;
            var exp = predicate.Equal(t => t.Name,"A" );
            exp &= predicate.Equal(t => t.Id, 3);
            var cityList = await repository.GetAllAsync(predicate: exp);
            Assert.Equal(0,cityList.Count);

            
            exp = predicate.Equal(t => t.Name, "A");
            exp |= predicate.Equal(t => t.Id, 3);
            cityList = await repository.GetAllAsync(predicate: exp);
            Assert.Equal(2,cityList.Count);
        }


        protected static List<Country> TestCountries => new List<Country>
        {
            new Country {Id = 1, Name = "A"},
            new Country {Id = 2, Name = "B"}
        };

        public static List<City> TestCities => new List<City>
        {
            new City { Id = 1, Name = "A", CountryId = 1},
            new City { Id = 2, Name = "B", CountryId = 2},
            new City { Id = 3, Name = "C", CountryId = 1},
            new City { Id = 4, Name = "D", CountryId = 2},
            new City { Id = 5, Name = "E", CountryId = 1},
            new City { Id = 6, Name = "F", CountryId = 2},
        };

        public static List<Town> TestTowns => new List<Town>
        {
            new Town { Id = 1, Name="TownA", CityId = 1 },
            new Town { Id = 2, Name="TownB", CityId = 2 },
            new Town { Id = 3, Name="TownC", CityId = 3 },
            new Town { Id = 4, Name="TownD", CityId = 4 },
            new Town { Id = 5, Name="TownE", CityId = 5 },
            new Town { Id = 6, Name="TownF", CityId = 6 },
        };
    }
}
