using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AspMongoDB.Common
{
    public class BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
    }
}
