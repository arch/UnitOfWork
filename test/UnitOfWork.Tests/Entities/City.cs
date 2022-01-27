using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arch.EntityFrameworkCore.UnitOfWork.Tests.Entities
{
    public record City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public int CountryId { get; set; }

        public Country Country { get; set; }

        public List<Town> Towns { get; set; }
    }
}