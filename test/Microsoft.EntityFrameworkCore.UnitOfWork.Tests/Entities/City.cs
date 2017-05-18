using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.EntityFrameworkCore.UnitOfWork.Tests.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        [ForeignKey(nameof(Country))]
        public int CountryId { get; set; }

        public virtual Country Country { get; set; }
    }
}