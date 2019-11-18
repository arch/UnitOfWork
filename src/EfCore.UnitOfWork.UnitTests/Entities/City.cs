using System.Collections.Generic;

namespace EfCore.UnitOfWork.UnitTests.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
        public List<Town> Towns { get; set; }
    }
}