using FCG.Catalog.Domain.Common;
using FCG.Core;

namespace FCG.Catalog.Infra.Repository
{
    public interface IEventLogRepository
    {
        Task<IEnumerable<CartEventLog>> GetCartEnvetLogs();
        Task<CartEventLog?> GetCartEventLogById(string id);
        Task InsertCartEventLog(CartEventLog log);
        Task<IEnumerable<GameEventLog>> GetGameEnvetLogs();
        Task<GameEventLog?> GetGameEventLogById(string id);
        Task InsertGameEventLog(GameEventLog log);
        Task InsertOrderPlacedEventLog(OrderPlacedEventLog log);
	}
}
