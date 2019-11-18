using System.Threading.Tasks;
using EfCore.UnitOfWork.Extensions;
using EfCore.UnitOfWork.UnitTests.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EfCore.UnitOfWork.UnitTests
{
    public class RepositoryTests : IClassFixture<RepositoryFixture>
    {
        private readonly RepositoryFixture _fixture;

        public RepositoryTests(RepositoryFixture fixture) => _fixture = fixture;

        [Fact]
        public void Should_GetList_ResolveTheRightSetOfDataFromContext()
        {
            // Arrange
            var repository = _fixture.CreateRepository<City>();

            // Act
            var actual = repository.GetList(t => t.Name == "C", q => q.Include(t => t.Country).PagedBy(0,1));

            // Assert

            actual.Should().HaveCount(1)
                .And.Contain(m => m.Country != null && m.CountryId != 0 && m.Country.Name == "A" && m.Country.Id == 1);
        }          
        
        [Fact]
        public async Task Should_GetListAsync_ResolveTheRightSetOfDataFromContext()
        {
            // Arrange
            var repository = _fixture.CreateRepository<City>();

            // Act
            var actual = await repository.GetListAsync(t => t.Name == "C", q => q.Include(t => t.Country).PagedBy(0,1));

            // Assert

            actual.Should().HaveCount(1)
                .And.Contain(m => m.Country != null && m.CountryId != 0 && m.Country.Name == "A" && m.Country.Id == 1);
        }            
        
        [Fact]
        public void Should_GetProjectedList_WithProjection_ResolveTheSmallerSetOfDataFromContext()
        {
            // Arrange
            var repository = _fixture.CreateRepository<City>();

            // Act
            var actual = repository.GetProjectedList(t => t.Name == "C", m => m.Name, q => q.Include(t => t.Country));

            // Assert
            actual.Should().HaveCount(1)
                .And.Contain(m => m == "C");
        } 
        
        [Fact]
        public async Task Should_GetProjectedListAsync_WithProjection_ResolveTheSmallerSetOfDataFromContext()
        {
            // Arrange
            var repository = _fixture.CreateRepository<City>();

            // Act
            var actual = await repository.GetProjectedListAsync(t => t.Name == "C", m => m.Name, q => q.Include(t => t.Country));

            // Assert
            actual.Should().HaveCount(1)
                .And.Contain(m => m == "C");
        }        
        
        [Fact]
        public void Should_GetFirstOrDefault_ResolveTheRightSetOfDataFromContext()
        {
            // Arrange
            var repository = _fixture.CreateRepository<City>();

            // Act
            var actual = repository.GetFirstOrDefault(t => t.Name == "C", q => q.Include(t => t.Country));

            // Assert
            actual.Should().NotBeNull();
            actual.Name.Should().Be("C");
            actual.CountryId.Should().Be(1);
            actual.Country.Should().BeEquivalentTo(new Country{ Name= "A", Id = 1}, m => m.Excluding(t => t.Cities));
        }          
        
        [Fact]
        public async Task Should_GetFirstOrDefaultAsync_ResolveTheRightSetOfDataFromContext()
        {
            // Arrange
            var repository = _fixture.CreateRepository<City>();

            // Act
            var actual = await repository.GetFirstOrDefaultAsync(t => t.Name == "C", q => q.Include(t => t.Country));

            // Assert
            actual.Should().NotBeNull();
            actual.Name.Should().Be("C");
            actual.CountryId.Should().Be(1);
            actual.Country.Should().BeEquivalentTo(new Country{ Name= "A", Id = 1}, m => m.Excluding(t => t.Cities));
        }   
        
        [Fact]
        public async Task Should_GetFirstOrDefaultAsync_ReturnsMultipleLevelOfHierarchyWhenItsIncludedInQuery()
        {
            // Arrange
            var repository = _fixture.CreateRepository<Country>();
            
            // Act
            var actual = await repository.GetFirstOrDefaultAsync(t => t.Name == "A", country => country.Include(c => c.Cities).ThenInclude(city => city.Towns));

            // Assert
            actual.Should().NotBeNull();
            actual.Cities.Should().HaveCount(3).And.Contain(m => m.Towns.Count == 1);
        }        
        
        [Fact]
        public void Should_Find_ReturnsCertainEntityByIdentifier()
        {
            // Arrange
            var repository = _fixture.CreateRepository<Country>();
            
            // Act
            var actual = repository.Find(1);

            // Assert
            actual.Should().NotBeNull();
            actual.Name.Should().Be("A");
        }  
        
        [Fact]
        public async Task Should_FindAsync_ReturnsCertainEntityByIdentifier()
        {
            // Arrange
            var repository = _fixture.CreateRepository<Country>();
            
            // Act
            var actual = await repository.FindAsync(1);

            // Assert
            actual.Should().NotBeNull();
            actual.Name.Should().Be("A");
        }  
        
        [Theory]
        [InlineData("A", 1)]
        [InlineData("B", 1)]
        [InlineData("C", 0)]
        public void Should_Count_ReturnsTheNumberOfRecordsAccordingly(string phrase, int expectedCount)
        {
            // Arrange
            var repository = _fixture.CreateRepository<Country>();
            
            // Act
            var actual = repository.Count(m => m.Name == phrase);

            // Assert
            actual.Should().Be(expectedCount);
        }   
    }
}
