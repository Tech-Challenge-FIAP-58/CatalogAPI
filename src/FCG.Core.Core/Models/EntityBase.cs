namespace FCG.Core.Core.Models
{
    public class EntityBase
    {
        public Guid Id { get; set; }
        public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    }
}
