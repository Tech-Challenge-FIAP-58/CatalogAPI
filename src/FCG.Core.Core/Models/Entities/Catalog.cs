namespace FCG.Core.Core.Models.Entities
{
    public class Catalog : Entity
    {
        public required int UserId { get; set; }
        public required Guid GameId { get; set; }
        public required decimal Price { get; set; }

    }
}
