using FCG.Catalog.Domain.Common;
using FCG.Catalog.Infra.Context;
using FCG.Core;
using MongoDB.Driver;

namespace FCG.Catalog.Infra.Repository
{
    public class EventLogRepository(MongoDbService mongoDbService) : IEventLogRepository
    {
        private readonly IMongoCollection<CartEventLog> _cartLogs = mongoDbService.Database?.GetCollection<CartEventLog>("cartLogs");
        private readonly IMongoCollection<GameEventLog> _gameLogs = mongoDbService.Database?.GetCollection<GameEventLog>("gameLogs");
		private readonly IMongoCollection<OrderPlacedEventLog> _orderLogs = mongoDbService.Database?.GetCollection<OrderPlacedEventLog>("orderLogs");

		public async Task InsertOrderPlacedEventLog(OrderPlacedEventLog log)
        {
			await _orderLogs.InsertOneAsync(log);
		}

		public async Task<IEnumerable<CartEventLog>> GetCartEnvetLogs()
        {
            return await _cartLogs.Find(FilterDefinition<CartEventLog>.Empty).ToListAsync();
        }

        public async Task<CartEventLog?> GetCartEventLogById(string id)
        {
            var filter = Builders<CartEventLog>.Filter.Eq(x => x.Id, id);
            var log = _cartLogs.Find(filter).FirstOrDefault();
            return log;
        }

        public async Task InsertCartEventLog(CartEventLog log)
        {
            await _cartLogs.InsertOneAsync(log);
        }

        public async Task<IEnumerable<GameEventLog>> GetGameEnvetLogs()
        {
            return await _gameLogs.Find(FilterDefinition<GameEventLog>.Empty).ToListAsync();
        }

        public async Task<GameEventLog?> GetGameEventLogById(string id)
        {
            var filter = Builders<GameEventLog>.Filter.Eq(x => x.Id, id);
            var log = _gameLogs.Find(filter).FirstOrDefault();
            return log;
        }

        public async Task InsertGameEventLog(GameEventLog log)
        {
            await _gameLogs.InsertOneAsync(log);
        }
    }
}
