namespace FCG.Core.Core.Models
{
    public class Catalog : EntityBase
    {
        public required Guid UserId { get; set; }
        public required Guid GameId { get; set; }
        public required decimal Price { get; set; }

    }
}
