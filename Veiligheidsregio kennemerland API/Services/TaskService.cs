using System.IO;
using Newtonsoft.Json;
using Task = System.Threading.Tasks.Task;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using MongoDB.Bson.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Kennemerland.DAL;

namespace Kennemerland.Services
{
    public interface ITasksService
    {
        Task<Models.Task> CreateTask(Models.Task task);
        Task<Models.Task> GetTask(int id);
        Task<IEnumerable<Models.Task>> GetAllTasks();
        Task<Models.Task> CompleteTask(int id);
        Task DeleteTask(int taskId);
    }
    public class TaskSerivice : ITasksService
    {
        private readonly ITaskRepository _tasks;
        public TaskSerivice(ILogger<ITasksService> Logger, ITaskRepository tasksRepository)
        {
            this._tasks = tasksRepository;
        }

        public Task<Models.Task> CompleteTask(int TaskId)
        {
            BsonDocument taskBson = _tasks.GetTask(TaskId);
            taskBson.Remove("_id");
            Models.Task taskToComplete = BsonSerializer.Deserialize<Models.Task>(taskBson);
            taskToComplete.Completed = true;
            taskToComplete.CompletedWhen = DateTime.UtcNow;
            taskToComplete.EditedOn = DateTime.UtcNow;
            _tasks.UpdateTask(BsonDocument.Parse(taskToComplete.ToJson()), TaskId);
            return Task.FromResult(taskToComplete);
        }

        public Task<Models.Task> CreateTask(Models.Task task)
        {
            if (_tasks.GetTask(task.TaskId) == null)
            {
                task.CreatedOn = DateTime.UtcNow;
                task.EditedOn = DateTime.UtcNow;
                _tasks.CreateTask(task);
                return Task.FromResult(task);
            }
            return Task.FromResult<Models.Task>(null);
        }

        public Task DeleteTask(int TaskId)
        {
            if (_tasks.GetTask(TaskId) == null) return Task.FromResult(false);
            return Task.FromResult(_tasks.DeleteTask(TaskId));
        }

        public Task<Models.Task> GetTask(int TaskId)
        {
            BsonDocument retrievedTask = _tasks.GetTask(TaskId);
            if (retrievedTask == null) return Task.FromResult<Models.Task>(null);
            retrievedTask.Remove("_id");
            Models.Task task = BsonSerializer.Deserialize<Models.Task>(retrievedTask);
            return Task.FromResult(task);
        }

        public async Task<IEnumerable<Models.Task>> GetAllTasks()
        {
            List<Models.Task> tasks = new List<Models.Task>();
            IEnumerable<BsonDocument> bsonTasks = _tasks.GetAllTasks();
            foreach (BsonDocument bson in bsonTasks)
            {
                bson.Remove("_id");
                Models.Task newTask = BsonSerializer.Deserialize<Models.Task>(bson);
                tasks.Add(newTask);
            };
            return tasks;
        }
    }
}
