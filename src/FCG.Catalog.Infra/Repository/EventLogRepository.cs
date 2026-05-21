using FCG.Catalog.Domain.Common;
using FCG.Catalog.Infra.Context;
using MongoDB.Driver;

namespace FCG.Catalog.Infra.Repository
{
    public class EventLogRepository(MongoDbService mongoDbService) : IEventLogRepository
    {
        private readonly IMongoCollection<CartEventLog>? _cartLogs = mongoDbService.Database?.GetCollection<CartEventLog>("cartLogs");
        private readonly IMongoCollection<GameEventLog>? _gameLogs = mongoDbService.Database?.GetCollection<GameEventLog>("gameLogs");

        public async Task<IEnumerable<CartEventLog>> GetCartEnvetLogs()
        {
            if (_cartLogs is null) return [];
            return await _cartLogs.Find(FilterDefinition<CartEventLog>.Empty).ToListAsync();
        }

        public async Task<CartEventLog?> GetCartEventLogById(string id)
        {
            if (_cartLogs is null) return null;
            var filter = Builders<CartEventLog>.Filter.Eq(x => x.Id, id);
            return _cartLogs.Find(filter).FirstOrDefault();
        }

        public async Task InsertCartEventLog(CartEventLog log)
        {
            if (_cartLogs is null) return;
            await _cartLogs.InsertOneAsync(log);
        }

        public async Task<IEnumerable<GameEventLog>> GetGameEnvetLogs()
        {
            if (_gameLogs is null) return [];
            return await _gameLogs.Find(FilterDefinition<GameEventLog>.Empty).ToListAsync();
        }

        public async Task<GameEventLog?> GetGameEventLogById(string id)
        {
            if (_gameLogs is null) return null;
            var filter = Builders<GameEventLog>.Filter.Eq(x => x.Id, id);
            return _gameLogs.Find(filter).FirstOrDefault();
        }

        public async Task InsertGameEventLog(GameEventLog log)
        {
            if (_gameLogs is null) return;
            await _gameLogs.InsertOneAsync(log);
        }
    }
}
