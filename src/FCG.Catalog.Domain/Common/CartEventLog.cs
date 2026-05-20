using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FCG.Catalog.Domain.Common
{
    public class CartEventLog : EventLog
    {
        [BsonElement("userId"), BsonRepresentation(BsonType.Int32)]
        public int? UserId { get; set; }

        [BsonElement("gameId"), BsonRepresentation(BsonType.String)]
        public string? GameId { get; set; }
    }
}
