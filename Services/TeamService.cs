using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Models;
using Microsoft.OpenApi.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.IO;
using Newtonsoft.Json;
using Task = System.Threading.Tasks.Task;
using MongoDB.Driver;
using Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using DAL;

namespace Services
{
    public interface ITeamsService
    {
        Task<Team> CreateTeam(Team team);
        Task<Team> GetTeam(int TeamId);
        Task<IEnumerable<Team>> GetAllTeams();
        Task<Team> UpdateTeam(Team team);
        Task<Team> AddUserToTeam(int TeamId, int UserId);
        Task<IEnumerable<Team>> GetUserTeams(int UserId);
        Task DeleteTeam(int TeamId);
    }

    public class TeamService : ITeamsService
    {
        private readonly ITeamRepository _teams;
        private readonly IUserRepository _users;
        public TeamService(ILogger<TeamService> Logger, ITeamRepository teamRepository, IUserRepository userRepository)
        {
            this._teams = teamRepository;
            this._users = userRepository;
        }

        public Task<Team> CreateTeam(Team team)
        {
            if (_teams.GetTeam(team.TeamId) == null)
            {
                team.CreatedOn = DateTime.UtcNow;
                team.EditedOn = DateTime.UtcNow;
                _teams.CreateTeam(team);
                return Task.FromResult(team);
            }
            return Task.FromResult<Team>(null);
        }

        public async Task<Team> GetTeam(int TeamIid)
        {
            BsonDocument retrievedTeam = _teams.GetTeam(TeamIid);
            retrievedTeam.Remove("_id");
            Team team = BsonSerializer.Deserialize<Team>(retrievedTeam);
            return team;
        }

        public Task<Team> UpdateTeam(Team team)
        {
            _teams.UpdateTeam(BsonDocument.Parse(team.ToJson()), team.TeamId);
            BsonDocument newTeam = _teams.GetTeam(team.TeamId);
            newTeam.Remove("_id");
            return Task.FromResult(BsonSerializer.Deserialize<Team>(newTeam));
        }

        public Task<Team> AddUserToTeam(int TeamId, int UserId)
        {
            BsonDocument userBson = _users.GetUser(UserId);
            BsonDocument teamBson = _teams.GetTeam(TeamId);

            if (teamBson != null && userBson != null)
            {
                User userToAdd = BsonSerializer.Deserialize<User>(userBson);
                Team teamToAdd = BsonSerializer.Deserialize<Team>(teamBson);
                if (teamToAdd.TeamMembers == null) teamToAdd.TeamMembers = new List<User>();
                teamToAdd.TeamMembers.Add(userToAdd);
                _teams.UpdateTeam(BsonDocument.Parse(teamToAdd.ToJson()), teamToAdd.TeamId);
            }

            var newTeam = _teams.GetTeam(TeamId);
            newTeam.Remove("_id");

            Team returner = BsonSerializer.Deserialize<Team>(newTeam);
            return Task.FromResult(returner);
        }

        public Task DeleteTeam(int TeamId)
        {
            if (_teams.GetTeam(TeamId) == null) return Task.FromResult(false);
            return Task.FromResult(_teams.DeleteTeam(TeamId));
        }

        public async Task<IEnumerable<Team>> GetUserTeams(int UserId)
        {
            IEnumerable<BsonDocument> bsonTeams = _teams.GetAllTeams();
            List<Team> userTeams = new List<Team>();

            foreach (BsonDocument bson in bsonTeams)
            {
                bson.Remove("_id");
                Team teamToCheck = BsonSerializer.Deserialize<Team>(bson);
                foreach (User u in teamToCheck.TeamMembers)
                {
                    if (u.UserId == UserId) userTeams.Add(teamToCheck);
                }
            }
            return userTeams;
        }

        public async Task<IEnumerable<Team>> GetAllTeams()
        {
            List<Team> teams = new List<Team>();
            IEnumerable<BsonDocument> bsonTeams = _teams.GetAllTeams();
            foreach (BsonDocument bson in bsonTeams)
            {
                bson.Remove("_id");
                Team newTeam = BsonSerializer.Deserialize<Team>(bson);
                teams.Add(newTeam);
            };
            return teams;
        }
    }
}