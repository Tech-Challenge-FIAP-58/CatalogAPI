
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FIAP.FCG.CATALOG.Core.Models
{
	public class Order : EntityBase
	{
		public required DateTime OrderDate { get; set; }
		public required int UserId { get; set; }
        public required int GameId { get; set; }
		public required Decimal Price { get; set; }
        public required string PaymentStatus { get; set; }
        public required string CardName { get; set; }
        public required string CardNumber { get; set; }
        public required string ExpirationDate { get; set; }
        public required string Cvv { get; set; }

    }
}
