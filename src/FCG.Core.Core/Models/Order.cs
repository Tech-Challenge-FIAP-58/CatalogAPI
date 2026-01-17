namespace FCG.Core.Core.Models
{
    public class Order : EntityBase
    {
        public required DateTime OrderDate { get; set; }
        public required Guid UserId { get; set; }
        public required Guid GameId { get; set; }
        public required decimal Price { get; set; }
        public required string PaymentStatus { get; set; }
        public required string CardName { get; set; }
        public required string CardNumber { get; set; }
        public required string ExpirationDate { get; set; }
        public required string Cvv { get; set; }

    }
}
