using AspMongoDB.Common;
using MongoDB.Bson.Serialization.Attributes;

namespace AspMongoDB.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string Family { get; set; }

        [BsonIgnore]
        public string Fullname => $"{Name} {Family}";
    }
}
