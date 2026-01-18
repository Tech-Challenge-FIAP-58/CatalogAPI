namespace FCG.Core.Core.Models.Entities
{
    public class Game : Entity
    {
        public required string Name { get; set; }
        public required string Platform { get; set; }
        public required string PublisherName { get; set; }
        public required string Description { get; set; }
        public required double Price { get; set; }
    }
}
