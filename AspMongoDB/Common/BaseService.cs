using AspMongoDB.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

//commented codes are for transactional operations
namespace AspMongoDB.Common
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : BaseEntity
    {
        private readonly IMongoCollection<TEntity> _collections;
        //private readonly MongoDBContext _context;


        //public BaseService(MongoDBContext context)
        //{
        //    _context = context;
        //    var database = context.Database;
        //    _collections = database.GetCollection<TEntity>(typeof(TEntity).Name + "s");
        //}

        // Use IMongoClient and IOptions<MongoSettings> in the constructor
        public BaseService(IMongoClient client, IOptions<MongoSettings> settings)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName); // Access settings.Value
            _collections = database.GetCollection<TEntity>(typeof(TEntity).Name + "s");
        }

        public void Insert(TEntity entity)
        {
            // _context.StartTransatction();
            // _collections.InsertOne(_context.Session, entity);

            _collections.InsertOne(entity);
        }

        public void Update(TEntity entity)
        {
            //_context.StartTransatction();
            //_collections.ReplaceOne(_context.Session, e => e.Id == entity.Id, entity);

            _collections.ReplaceOne(e => e.Id == entity.Id, entity);
        }

        public void Delete(string id)
        {
            //_context.StartTransatction();
            //_collections.DeleteOne(_context.Session, e => e.Id == id);

            _collections.DeleteOne(e => e.Id == id);
        }

        public TEntity GetById(string id)
        {
            return _collections.Find(e => e.Id == id).FirstOrDefault();
        }

        public List<TEntity> GetAll()
        {
            return _collections.Find(e => true).ToList();
        }

    }
}
