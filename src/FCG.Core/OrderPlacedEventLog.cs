using FCG.Core.Integration;

namespace FCG.Core
{
	public class OrderPlacedEventLog : OrderPlacedEvent
	{
		public OrderPlacedEventLog(int clientId, Guid orderId, PaymentMethod paymentMethod, decimal amount, string cardName, string cardNumber, string expirationDate, string cvv) : base(clientId, orderId, paymentMethod, amount, cardName, cardNumber, expirationDate, cvv)
		{
		}

		public string Message { get; set; }
		public DateTime DtCpu { get; set; }
	}
}
