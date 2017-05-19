using System.ComponentModel.DataAnnotations;

namespace Microsoft.EntityFrameworkCore.UnitOfWork.Tests.Entities
{
    public class Customer
    {
        [Key]
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
