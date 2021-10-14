using System.Threading.Tasks;
using Kennemerland.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Bson;
using Kennemerland.Services;

namespace Kennemerland.DAL
{
    public interface IUserRepository
    {
        User CreateUser(User user);
        BsonDocument GetUser(long UserId);
        BsonDocument GetUser(string Username);
        IEnumerable<BsonDocument> GetAllUsers();
        void UpdateUser(User user);
    }

    public class UserRepository : IUserRepository
    {
        public User CreateUser(User user)
        {
            IMongoCollection<BsonDocument> collection = MongoSingleton.getMongoCollection("users");
            collection.InsertOne(BsonDocument.Parse(user.ToJson()));
            return user;
        }

        public BsonDocument GetUser(long UserId)
        {
            IMongoCollection<BsonDocument> collection = MongoSingleton.getMongoCollection("users");
            var filter = Builders<BsonDocument>.Filter.Eq("UserId", UserId);
            var user = collection.Find(filter).FirstOrDefault();
            return user;
        }

        public BsonDocument GetUser(string Username)
        {
            IMongoCollection<BsonDocument> collection = MongoSingleton.getMongoCollection("users");
            var filter = Builders<BsonDocument>.Filter.Eq("Username", Username);
            var user = collection.Find(filter).FirstOrDefault();
            return user;
        }

        public IEnumerable<BsonDocument> GetAllUsers()
        {
            IMongoCollection<BsonDocument> collection = MongoSingleton.getMongoCollection("users");
            IEnumerable<BsonDocument> documents = collection.Find(_ => true).ToList();
            return documents;
        }

        public void UpdateUser(User user)
        {
            IMongoCollection<BsonDocument> collection = MongoSingleton.getMongoCollection("users");
            var filter = Builders<BsonDocument>.Filter.Eq("UserId", user.UserId);
            collection.ReplaceOne(filter, BsonDocument.Parse(user.ToJson()));
        }
    }
}