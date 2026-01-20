using FCG.Core.Integration;

namespace FCG.Catalog.Application.Interfaces
{
	public interface IOrderPlacedEventProducer
	{
		Task Send(OrderPlacedEvent message);
	}
}
