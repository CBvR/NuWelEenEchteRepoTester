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
using Services;

namespace Kennemerland.Controllers {
	public class TaskHttpTrigger {
		ILogger Logger { get; }
		ITasksService TasksService { get; }

		public TaskHttpTrigger(ITasksService TasksService, ILogger<TaskHttpTrigger> Logger) {
			this.Logger = Logger;
			this.TasksService = TasksService;
		}

		[Function(nameof(TaskHttpTrigger.AddTask))]
		[OpenApiOperation(operationId: "addTask", tags: new[] { "task" }, Summary = "Add a new task to the store", Description = "This add a new task to the store.", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("taskstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Models.Task), Example = typeof(DummyTaskExample), Required = true, Description = "Task object that needs to be added to the store")]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Models.Task), Summary = "New task details added", Description = "New task details added", Example = typeof(DummyTaskExample))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.MethodNotAllowed, Summary = "Invalid input", Description = "Invalid input")]
		public async Task<HttpResponseData> AddTask([HttpTrigger(AuthorizationLevel.Function, "POST", Route = "task")] HttpRequestData req, FunctionContext executionContext) {
			// Parse input
			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Models.Task task = JsonConvert.DeserializeObject<Models.Task>(requestBody);

			task = await TasksService.CreateTask(task);

			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			await response.WriteAsJsonAsync(task);

			return response;
		}

		[Function(nameof(TaskHttpTrigger.UpdateTask))]
		[OpenApiOperation(operationId: "updateTask", tags: new[] { "task" }, Summary = "Update an existing task", Description = "This updates an existing task.", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("taskstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Models.Task), Required = true, Description = "Task object that needs to be updated to the store")]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Models.Task), Summary = "Task details updated", Description = "Task details updated", Example = typeof(DummyTaskExample))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid ID supplied", Description = "Invalid ID supplied")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Task not found", Description = "Task not found")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.MethodNotAllowed, Summary = "Validation exception", Description = "Validation exception")]
		public async Task<HttpResponseData> UpdateTask([HttpTrigger(AuthorizationLevel.Function, "PUT", Route = "task")] HttpRequestData req, FunctionContext executionContext) {
			// Parse input
			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			Models.Task task = JsonConvert.DeserializeObject<Models.Task>(requestBody);

			// Generate output
			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			await response.WriteAsJsonAsync(task);

			return response;
		}

		[Function(nameof(TaskHttpTrigger.CompletedTask))]
		[OpenApiOperation(operationId: "completedTask", tags: new[] { "task" }, Summary = "Complete a task by running this endpoint", Description = "This completes a task", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("taskstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiParameter(name: "taskId", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID of task to complete", Description = "ID of task to complete", Visibility = OpenApiVisibilityType.Important)]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Models.Task), Summary = "Task details updated", Description = "Task details updated", Example = typeof(DummyTaskExample))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid ID supplied", Description = "Invalid ID supplied")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Task not found", Description = "Task not found")]
		public async Task<HttpResponseData> CompletedTask([HttpTrigger(AuthorizationLevel.Function, "PUT", Route = "task/completed")] HttpRequestData req, int taskId, FunctionContext executionContext)
		{
			// Generate output
			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			Models.Task completedTask = await TasksService.CompleteTask(taskId);

			await response.WriteAsJsonAsync(completedTask);

			return response;
		}

		[Function(nameof(TaskHttpTrigger.GetTaskById))]
		[OpenApiOperation(operationId: "getTaskById", tags: new[] { "task" }, Summary = "Find task by ID", Description = "Returns a single task.", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("taskstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiParameter(name: "taskId", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID of task to return", Description = "ID of task to return", Visibility = OpenApiVisibilityType.Important)]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Models.Task), Summary = "successful operation", Description = "successful operation", Example = typeof(DummyTaskExamples))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid ID supplied", Description = "Invalid ID supplied")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Task not found", Description = "Task not found")]
		public async Task<HttpResponseData> GetTaskById([HttpTrigger(AuthorizationLevel.Function, "GET", Route = "task/{taskId}")] HttpRequestData req, int taskId, FunctionContext executionContext) {
			// Generate output
			HttpResponseData response;

			Models.Task task = await TasksService.GetTask(taskId);

			if (task == null) response = req.CreateResponse(HttpStatusCode.BadRequest);
			else response = req.CreateResponse(HttpStatusCode.OK);

			await response.WriteAsJsonAsync(task);

			return response;
		}

		[Function(nameof(TaskHttpTrigger.GetAllTasks))]
		[OpenApiOperation(operationId: "getTaskById", tags: new[] { "task" }, Summary = "Find task by ID", Description = "Returns a single task.", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("taskstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Models.Task), Summary = "successful operation", Description = "successful operation", Example = typeof(DummyTaskExamples))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid ID supplied", Description = "Invalid ID supplied")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Task not found", Description = "Task not found")]
		public async Task<HttpResponseData> GetAllTasks([HttpTrigger(AuthorizationLevel.Function, "GET", Route = "task")] HttpRequestData req, FunctionContext executionContext)
		{
			// Generate output
			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			IEnumerable<Models.Task> tasks = await TasksService.GetAllTasks();

			await response.WriteAsJsonAsync(tasks);

			return response;
		}

		[Function(nameof(TaskHttpTrigger.DeleteTask))]
		[OpenApiOperation(operationId: "deleteTask", tags: new[] { "task" }, Summary = "Find task by ID", Description = "Deletes a single task.", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("taskstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiParameter(name: "taskId", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID of task to Delte", Description = "ID of task to Delete", Visibility = OpenApiVisibilityType.Important)]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Models.Task), Summary = "Deletion operation", Description = "Deletion operation", Example = typeof(DummyTaskExamples))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid ID supplied", Description = "Invalid ID supplied")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Task not found", Description = "Task not found")]
		public async Task<HttpResponseData> DeleteTask([HttpTrigger(AuthorizationLevel.Function, "DELETE", Route = "task/{taskId}")] HttpRequestData req, int taskId, FunctionContext executionContext)
		{
			// Generate output
			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			await TasksService.DeleteTask(taskId);

			return response;
		}
	}
}
