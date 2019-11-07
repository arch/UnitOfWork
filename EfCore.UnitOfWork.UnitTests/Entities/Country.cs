using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UnitOfWork.Tests.Entities
{
    public class Country
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public List<City> Cities { get; set; }
    }
}
