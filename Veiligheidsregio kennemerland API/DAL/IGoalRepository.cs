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
    public interface IGoalRepository
    {
        Goal CreateGoal(Goal goal);
        BsonDocument GetGoal(long goalId);
        bool DeleteGoal(long goalId);
    }

    public class GoalRepository : IGoalRepository
    {
        public Goal CreateGoal(Goal goal)
        {
            IMongoCollection<BsonDocument> collection = MongoSingleton.getMongoCollection("goals");
            var newGoal = BsonDocument.Parse(goal.ToJson());
            collection.InsertOne(newGoal);
            return goal;
        }

        public bool DeleteGoal(long GoalId)
        {
            IMongoCollection<BsonDocument> collection = MongoSingleton.getMongoCollection("goals");
            var filter = Builders<BsonDocument>.Filter.Eq("GoalId", GoalId);
            try { 
                collection.DeleteOne(filter);
                return true;
            }
            catch {
                return false;
            }
        }

        public BsonDocument GetGoal(long GoalId)
        {
            IMongoCollection<BsonDocument> collection = MongoSingleton.getMongoCollection("goals");

            var filter = Builders<BsonDocument>.Filter.Eq("GoalId", GoalId);
            var goal = collection.Find(filter).FirstOrDefault();
            goal.Remove("_id");
            return goal;
        }
    }
}