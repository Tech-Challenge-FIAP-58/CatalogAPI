namespace FCG.Core.Core.Models
{
    public class Catalog : EntityBase
    {
        public required int UserId { get; set; }
        public required int GameId { get; set; }
        public required decimal Price { get; set; }

    }
}
