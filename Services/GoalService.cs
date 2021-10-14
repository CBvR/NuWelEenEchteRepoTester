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
using MongoDB.Bson;
using System;
using DAL;
using MongoDB.Bson.Serialization;

namespace Services
{
    public interface IGoalsService
    {
        Task<Goal> CreateGoal(Goal goal);
        Task<Goal> GetGoal(int goalId);
        Task<IEnumerable<Goal>> GetTeamGoals(int teamId);
        Task DeleteGoal(int goalId);
    }

    public class GoalService : IGoalsService
    {

        private readonly IGoalRepository _goals;

        public GoalService(ILogger<GoalService> Logger, IGoalRepository goalRepository)
        {
            this._goals = goalRepository;
        }

        public async Task<Goal> CreateGoal(Goal goal)
        {
            goal.CreatedOn = DateTime.UtcNow;
            goal.EditedOn = DateTime.UtcNow;
            goal.CreatorId = 101;
            goal.StartDate = DateTime.UtcNow;
            _goals.CreateGoal(goal);
            return goal;
        }

        public async Task<Goal> GetGoal(int GoalId)
        {
            BsonDocument retrievedGoal = _goals.GetGoal(GoalId);
            return BsonSerializer.Deserialize<Goal>(retrievedGoal);
        }

        public Task DeleteGoal(int GoalId)
        {
            if (_goals.GetGoal(GoalId) == null) return Task.FromResult(false);
            return Task.FromResult(_goals.DeleteGoal(GoalId));
        }

        public async Task<IEnumerable<Goal>> GetTeamGoals(int teamId)
        {
            IEnumerable<BsonDocument> bsonGoals = _goals.GetAllGoals();
            List<Goal> teamGoals = new List<Goal>();

            foreach (BsonDocument bson in bsonGoals)
            {
                bson.Remove("_id");
                Goal goalToCheck = BsonSerializer.Deserialize<Goal>(bson);
                if (goalToCheck.RelevantTeams.Contains(teamId)) teamGoals.Add(goalToCheck);
            }
            return teamGoals;
        }
    }
}