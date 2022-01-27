namespace Arch.EntityFrameworkCore.UnitOfWork.Tests.Entities
{
    public record Town
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int CityId { get; set; }

        public City City { get; set; }
    }
}
