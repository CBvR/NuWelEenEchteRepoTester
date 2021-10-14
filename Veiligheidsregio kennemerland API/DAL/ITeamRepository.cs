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
    public interface ITeamRepository
    {
        Team CreateTeam(Team team);
        BsonDocument GetTeam(int TeamId);
        void UpdateTeam(BsonDocument TeamToUpdate, int TeamId);
        IEnumerable<BsonDocument> GetAllTeams();
        bool DeleteTeam(int teamId);
        BsonDocument GetTeamAbove(Team team);
    }

    public class TeamRepository : ITeamRepository
    {
        public Team CreateTeam(Team team)
        {
            IMongoCollection<BsonDocument> collection = MongoSingleton.getMongoCollection("teams");
            collection.InsertOne(BsonDocument.Parse(team.ToJson()));
            return team;
        }

        public BsonDocument GetTeam(int TeamId)
        {
            IMongoCollection<BsonDocument> collection = MongoSingleton.getMongoCollection("teams");
            var filter = Builders<BsonDocument>.Filter.Eq("TeamId", TeamId);
            var team = collection.Find(filter).FirstOrDefault();
            return team;
        }

        public void UpdateTeam(BsonDocument teamToUpdate, int TeamId)
        {
            IMongoCollection<BsonDocument> collection = MongoSingleton.getMongoCollection("teams");
            var filter = Builders<BsonDocument>.Filter.Eq("TeamId", TeamId);
            collection.ReplaceOne(filter, teamToUpdate);
        }

        public IEnumerable<BsonDocument> GetAllTeams()
        {
            IMongoCollection<BsonDocument> collection = MongoSingleton.getMongoCollection("teams");
            IEnumerable<BsonDocument> documents = collection.Find(_ => true).ToList();
            return documents;
        }

        public bool DeleteTeam(int TeamId)
        {
            try {
                IMongoCollection<BsonDocument> collection = MongoSingleton.getMongoCollection("teams");
                var filter = Builders<BsonDocument>.Filter.Eq("TeamId", TeamId);
                var team = collection.DeleteOne(filter);
                return true;
            }
            catch {
                return false;
            }
        }

        public BsonDocument GetTeamAbove(Team team)
        {
            return new BsonDocument();
        }
    }
}