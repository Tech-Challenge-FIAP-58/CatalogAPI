namespace FCG.Core.Core.Inputs
{
    public class PaymentProcessedDto
    {

        public Guid orderId { get; set; }
        public Guid paymentId { get; set; }
        public decimal amount { get; set; }
        public int status { get; set; }

        public string? reason { get; set; }

        /*
        public int OrderId { get; set; }
        public string PaymentStatus { get; set; }*/
    }
}
