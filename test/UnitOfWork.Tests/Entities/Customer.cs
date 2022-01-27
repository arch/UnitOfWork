namespace Arch.EntityFrameworkCore.UnitOfWork.Tests.Entities
{
    public record Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
