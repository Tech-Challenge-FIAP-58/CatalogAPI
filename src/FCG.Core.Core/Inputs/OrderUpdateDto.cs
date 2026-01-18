namespace FCG.Core.Core.Inputs
{
    public sealed record OrderUpdateDto
    {
        public DateTime OrderDate { get; set; }
        public int UserId { get; set; }
        public Guid GameId { get; set; }
        public decimal Price { get; set; }
        public string PaymentStatus { get; set; }
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string Cvv { get; set; }
    }
}
