using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Bson;

namespace DAL
{
    public interface ITaskRepository
    {
        Task CreateTask(Task task);
        BsonDocument GetTask(int id);
        IEnumerable<BsonDocument> GetAllTasks();
        void UpdateTask(BsonDocument taskToUpdate, int TaskId);
        bool DeleteTask(int TaskId);
    }

    public class TaskRepository : ITaskRepository
    {
        public Task CreateTask(Task task)
        {
            IMongoCollection<BsonDocument> collection = MongoSingleton.getMongoCollection("tasks");
            collection.InsertOne(BsonDocument.Parse(task.ToJson()));
            return task;
        }

        public BsonDocument GetTask(int TaskId)
        {
            IMongoCollection<BsonDocument> collection = MongoSingleton.getMongoCollection("tasks");
            var filter = Builders<BsonDocument>.Filter.Eq("TaskId", TaskId);
            var task = collection.Find(filter).FirstOrDefault();
            return task;
        }

        public void UpdateTask(BsonDocument taskToUpdate, int TaskId)
        {
            IMongoCollection<BsonDocument> collection = MongoSingleton.getMongoCollection("tasks");
            var filter = Builders<BsonDocument>.Filter.Eq("TaskId", TaskId);
            collection.ReplaceOne(filter, taskToUpdate);
        }

        public IEnumerable<BsonDocument> GetAllTasks()
        {
            IMongoCollection<BsonDocument> collection = MongoSingleton.getMongoCollection("tasks");
            IEnumerable<BsonDocument> documents = collection.Find(_ => true).ToList();
            return documents;
        }

        public bool DeleteTask(int TaskId)
        {
            try
            {
                IMongoCollection<BsonDocument> collection = MongoSingleton.getMongoCollection("tasks");
                var filter = Builders<BsonDocument>.Filter.Eq("TaskId", TaskId);
                var task = collection.DeleteOne(filter);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}