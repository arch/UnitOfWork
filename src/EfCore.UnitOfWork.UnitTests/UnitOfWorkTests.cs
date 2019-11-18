using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EfCore.UnitOfWork.UnitTests.Entities;
using FluentAssertions;
using Xunit;

namespace EfCore.UnitOfWork.UnitTests
{
    public class UnitOfWorkTests : IClassFixture<RepositoryFixture>
    {
        private readonly RepositoryFixture _fixture;

        public UnitOfWorkTests(RepositoryFixture fixture) => _fixture = fixture;

        [Fact]
        public void Should_SaveChanges_StoreTheDataCorrectly()
        {
            // Arrange
            var uow = _fixture.CreateUnitOfWork();

            // Act
            var repository = uow.GetRepository<Country>();
            repository.Insert(new Country() {Id = 100, Name = "Country 100"});
            repository.Insert(new Country() {Id = 101, Name = "Country 101"});
            repository.Insert(new Country() {Id = 102, Name = "Country 102"});
            uow.SaveChanges();
            
            var actual = repository.GetList();

            // Assert

            actual.Should().HaveCount(5);
        }           
        
        [Fact]
        public async Task Should_SaveChangesAsync_StoreTheDataCorrectly()
        {
            // Arrange
            var uow = _fixture.CreateUnitOfWork();

            // Act
            var repository = uow.GetRepository<Country>();
            repository.Insert(new Country() {Id = 100, Name = "Country 100"});
            repository.Insert(new Country() {Id = 101, Name = "Country 101"});
            repository.Insert(new Country() {Id = 102, Name = "Country 102"});
            await uow.SaveChangesAsync();
            
            var actual = await repository.GetListAsync();

            // Assert
            actual.Should().HaveCount(5);
        }    
        
        [Fact]
        public void Should_SaveChanges_WhenMultipleOperation_ThenChangeTheSateOfTheDataSourceAccordingly()
        {
            // Arrange
            var uow = _fixture.CreateUnitOfWork();
            var repository = uow.GetRepository<Country>();
            
            // Act
            var list = repository.GetList().ToList();
            var entityForUpdate = list.First();
            entityForUpdate.Name = $"Country {entityForUpdate.Id}";
            
            repository.Insert(new List<Country>
            {
                new Country {Id = 100, Name = "Country 100"},
                new Country {Id = 101, Name = "Country 101"},
                new Country {Id = 102, Name = "Country 102"}
            });

            repository.Update(entityForUpdate);
            repository.Delete(list.Last().Id);
            
            uow.SaveChanges();

            var actual = repository.GetList();

            // Assert
            actual.Should().HaveCount(4)
                .And.Contain(m => m.Id == 1 && m.Name == "Country 1")
                .And.Contain(m => m.Id == 100 && m.Name == "Country 100")
                .And.Contain(m => m.Id == 101 && m.Name == "Country 101")
                .And.Contain(m => m.Id == 102 && m.Name == "Country 102");
        }
    }
}