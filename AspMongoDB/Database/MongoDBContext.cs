using AspMongoDB.Models;
using MongoDB.Driver;

namespace AspMongoDB.Database
{

    // Use for Transactional operations. apply on BaseService class.
    public class MongoDBContext
    {
        private readonly IClientSessionHandle _session;
        private readonly IMongoDatabase _database;
        private readonly IMongoClient _client;

        public MongoDBContext(MongoSettings settings, IMongoClient client)
        {
            _session = _client.StartSession();
            _client = client;
            _database = _client.GetDatabase(settings.DatabaseName);
        }

        public IMongoDatabase Database { get { return _database; } }

        public IClientSessionHandle Session
        {
            get
            {
                return _session;
            }
        }

        public void StartTransatction()
        {
            _session.StartTransaction();
        }

        public void Commit()
        {
            _session.CommitTransaction();
        }
    }
}
