using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FCG.Catalog.Domain.Common
{
    public class GameEventLog : EventLog
    {

        [BsonElement("gameId"), BsonRepresentation(BsonType.String)]
        public string? GameId { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public string? Name { get; set; }
    }
}
