using MongoDB.Bson.Serialization.Attributes;

namespace AspProMongoDb.Web.Entities
{
    public class User
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }
        public string Family { get; set; }

        [BsonIgnore]
        public string Fullname => $"{Name} {Family}";
    }
}
