using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
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
using MongoDB.Bson;
using Services;
using MongoDB.Bson.Serialization;
using System;
using System.Linq;
using System.Text;
using DAL;

namespace Services
{
    public interface IUsersService
    {
        Task<User> CreateUser(User user);
        Task<User> GetUser(long userId);
        Task<IEnumerable<User>> GetAllUsers();
        Task<IEnumerable<Models.Task>> GetUserTasks(int userId);
        Task<User> UpdateUser(User user);
        string GetHashString(string inputString);
    }

    public class UserService : IUsersService
    {
        private readonly IUserRepository _users;
        private readonly ITeamsService _teamsService;
        private readonly IGoalsService _goalsService;
        private readonly ITasksService _tasksService;

        public UserService(ILogger<UserService> Logger, IUserRepository userRepository, IGoalsService goalService, ITeamsService teamService, ITasksService taskService)
        {
            this._users = userRepository;
            this._teamsService = teamService;
            this._goalsService = goalService;
            this._tasksService = taskService;
        }

        public Task<User> CreateUser(User user)
        {
            if (_users.GetUser(user.UserId) == null) {
                user.CreatedOn = DateTime.UtcNow;
                user.EditedOn = DateTime.UtcNow;
                user.Password = GetHashString(user.Password);
                _users.CreateUser(user);
                return Task.FromResult(user);
            }
            return Task.FromResult<User>(null);
        }

        public async Task<User> GetUser(long UserId)
        {
            BsonDocument retrievedUser = _users.GetUser(UserId);
            retrievedUser.Remove("_id");
            User user = BsonSerializer.Deserialize<User>(retrievedUser);
            return user;
        }

        public async Task<User> GetUser(string UserName)
        {
            BsonDocument retrievedUser = _users.GetUser(UserName);
            retrievedUser.Remove("_id");
            User user = BsonSerializer.Deserialize<User>(retrievedUser);
            return user;
        }
        
        public async Task<IEnumerable<User>> GetAllUsers()
        { 
            List<User> users = new List<User>();
            IEnumerable<BsonDocument> bsonUsers = _users.GetAllUsers();
            foreach (BsonDocument bson in bsonUsers)
            {
                bson.Remove("_id");
                User newUser = BsonSerializer.Deserialize<User>(bson);
                users.Add(newUser);
            };
            return users;
        }

        public Task<User> UpdateUser(User user)
        {
            user.Password = GetHashString(user.Password);
            _users.UpdateUser(user);
            BsonDocument newUser = _users.GetUser(user.UserId);
            newUser.Remove("_id");
            return Task.FromResult(BsonSerializer.Deserialize<User>(newUser));
        }

        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public async Task<IEnumerable<Models.Task>> GetUserTasks(int userId)
        {
            IEnumerable<Team> userTeams = await _teamsService.GetUserTeams(userId);
            List<Models.Task> userTasks = new List<Models.Task>();
            foreach (Team userTeam in userTeams)
            {
                IEnumerable<Goal> userTeamGoals = await _goalsService.GetTeamGoals(userTeam.TeamId);
                foreach (Goal teamGoal in userTeamGoals)
                {
                    userTasks.Concat(await _tasksService.GetGoalTasks(teamGoal.GoalId));
                }
            }
            return userTasks;
        }
    }
}
