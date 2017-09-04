using System.Collections.Generic;

namespace Microsoft.EntityFrameworkCore.UnitOfWork.Tests.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<City> Cities { get; set; }
    }
}
