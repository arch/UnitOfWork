namespace Microsoft.EntityFrameworkCore.UnitOfWork.Tests.Entities
{
    public class Town
    {
		public int Id { get; set; }
		public string Name { get; set; }

        public int CityId { get; set; }

        public City City { get; set; }
    }
}
