using System.Collections.Generic;
using EfCore.UnitOfWork.UnitTests.Entities;

namespace EfCore.UnitOfWork.UnitTests
{
    public class RepositoryFixture
    {
        private static IEnumerable<Country> TestCountries => new List<Country>
        {
            new Country {Id = 1, Name = "A"},
            new Country {Id = 2, Name = "B"}
        };

        private static IEnumerable<City> TestCities => new List<City>
        {
            new City { Id = 1, Name = "A", CountryId = 1},
            new City { Id = 2, Name = "B", CountryId = 2},
            new City { Id = 3, Name = "C", CountryId = 1},
            new City { Id = 4, Name = "D", CountryId = 2},
            new City { Id = 5, Name = "E", CountryId = 1},
            new City { Id = 6, Name = "F", CountryId = 2},
        };

        private static IEnumerable<Town> TestTowns => new List<Town>
        {
            new Town { Id = 1, Name="A", CityId = 1 },
            new Town { Id = 2, Name="B", CityId = 2 },
            new Town { Id = 3, Name="C", CityId = 3 },
            new Town { Id = 4, Name="D", CityId = 4 },
            new Town { Id = 5, Name="E", CityId = 5 },
            new Town { Id = 6, Name="F", CityId = 6 },
        };

        public InMemoryDbContext DbContext()
        {
             var dbContext = new InMemoryDbContext();

            dbContext.AddRange(TestCountries);
            dbContext.AddRange(TestCities);
            dbContext.AddRange(TestTowns);

            dbContext.SaveChanges();

            return dbContext;
        }

        public IUnitOfWork CreateUnitOfWork() => new UnitOfWork<InMemoryDbContext>(DbContext());
        
        public IRepository<T> CreateRepository<T>() where T : class => new Repository<T>(DbContext());
    }
}