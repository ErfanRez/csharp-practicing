using AspMongoDB.Common;
using AspMongoDB.Entities;
using AspMongoDB.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AspMongoDB.Services
{
    public class UserService : BaseService<User>, IUserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IMongoClient client, IOptions<MongoSettings> settings) : base(client, settings)
        {
        }

    }
}