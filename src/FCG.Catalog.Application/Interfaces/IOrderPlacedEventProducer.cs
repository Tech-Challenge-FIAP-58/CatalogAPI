using FCG.Core.Messages.Integration;

namespace FCG.Catalog.Application.Interfaces
{
	public interface IOrderPlacedEventProducer
	{
		Task Send(OrderPlacedEvent message);
	}
}
