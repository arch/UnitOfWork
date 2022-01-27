using System.Collections.Generic;

namespace Arch.EntityFrameworkCore.UnitOfWork.Tests.Entities
{
    public record Country
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<City> Cities { get; set; }
    }
}
