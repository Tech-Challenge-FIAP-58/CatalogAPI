namespace FIAP.FCG.CATALOG.Core.Models
{
	public class EntityBase
	{
		public int Id { get; set; }
        public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    }
}
