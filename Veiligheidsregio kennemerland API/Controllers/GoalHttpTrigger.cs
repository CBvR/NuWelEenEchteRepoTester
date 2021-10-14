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
	public class GoalHttpTrigger {
		ILogger Logger { get; }
		IGoalsService GoalsService { get; }

		public GoalHttpTrigger(IGoalsService GoalsService, ILogger<GoalHttpTrigger> Logger) {
			this.Logger = Logger;
			this.GoalsService = GoalsService;
		}

		[Function(nameof(GoalHttpTrigger.AddGoal))]
		[OpenApiOperation(operationId: "addGoal", tags: new[] { "goal" }, Summary = "Add a new goal to the store", Description = "This add a new goal to the store.", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("goalstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Goal), Example = typeof(DummyGoalExample), Required = true, Description = "Goal object that needs to be added to the store")]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Goal), Summary = "New goal details added", Description = "New goal details added", Example = typeof(DummyGoalExample))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.MethodNotAllowed, Summary = "Invalid input", Description = "Invalid input")]
		public async Task<HttpResponseData> AddGoal([HttpTrigger(AuthorizationLevel.Function, "POST", Route = "goal")] HttpRequestData req, FunctionContext executionContext) {
			// Parse input
			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			Goal goal = JsonConvert.DeserializeObject<Goal>(requestBody);

			goal = await GoalsService.CreateGoal(goal);

			// Generate output
			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			await response.WriteAsJsonAsync(goal);

			return response;
		}

		[Function(nameof(GoalHttpTrigger.UpdateGoal))]
		[OpenApiOperation(operationId: "updateGoal", tags: new[] { "goal" }, Summary = "Update an existing goal", Description = "This updates an existing goal.", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("goalstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Goal), Required = true, Description = "Goal object that needs to be updated to the store")]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Goal), Summary = "Goal details updated", Description = "Goal details updated", Example = typeof(DummyGoalExample))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid ID supplied", Description = "Invalid ID supplied")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Goal not found", Description = "Goal not found")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.MethodNotAllowed, Summary = "Validation exception", Description = "Validation exception")]
		public async Task<HttpResponseData> UpdateGoal([HttpTrigger(AuthorizationLevel.Function, "PUT", Route = "goal")] HttpRequestData req, FunctionContext executionContext) {
			// Parse input
			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			Goal goal = JsonConvert.DeserializeObject<Goal>(requestBody);

			// Generate output
			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			await response.WriteAsJsonAsync(goal);

			return response;
		}

		[Function(nameof(GoalHttpTrigger.AlterTasks))]
		[OpenApiOperation(operationId: "updateGoal", tags: new[] { "goal" }, Summary = "Add or remove tasks from a goal", Description = "Add or remove tasks to a goal. Tasks are ", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("goalstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Goal), Required = true, Description = "Goal object that needs to be updated to the store")]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Goal), Summary = "Goal details updated", Description = "Goal details updated", Example = typeof(DummyGoalExample))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid ID supplied", Description = "Invalid ID supplied")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Goal not found", Description = "Goal not found")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.MethodNotAllowed, Summary = "Validation exception", Description = "Validation exception")]
		public async Task<HttpResponseData> AlterTasks([HttpTrigger(AuthorizationLevel.Function, "PUT", Route = "goal/alterTasks")] HttpRequestData req, FunctionContext executionContext)
		{
			// Parse input
			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			Goal goal = JsonConvert.DeserializeObject<Goal>(requestBody);

			// Generate output
			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			await response.WriteAsJsonAsync(goal);

			return response;
		}

		[Function(nameof(GoalHttpTrigger.GetGoalById))]
		[OpenApiOperation(operationId: "getGoalById", tags: new[] { "goal" }, Summary = "Find goal by ID", Description = "Returns a single goal.", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("goalstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiParameter(name: "goalId", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID of goal to return", Description = "ID of goal to return", Visibility = OpenApiVisibilityType.Important)]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Goal), Summary = "successful operation", Description = "successful operation", Example = typeof(DummyGoalExamples))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid ID supplied", Description = "Invalid ID supplied")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Goal not found", Description = "Goal not found")]
		public async Task<HttpResponseData> GetGoalById([HttpTrigger(AuthorizationLevel.Function, "GET", Route = "goal/{goalId}")] HttpRequestData req, int goalId, FunctionContext executionContext) {
			// Generate output
			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			Goal goal = await GoalsService.GetGoal(goalId);

			await response.WriteAsJsonAsync(goal);

			return response;
		}

		[Function(nameof(GoalHttpTrigger.DeleteGoal))]
		[OpenApiOperation(operationId: "deleteGoal", tags: new[] { "goal" }, Summary = "Find goal by ID", Description = "Returns a single goal.", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("goalstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiParameter(name: "goalId", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID of goal to return", Description = "ID of goal to return", Visibility = OpenApiVisibilityType.Important)]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Goal), Summary = "successful operation", Description = "successful operation", Example = typeof(DummyGoalExamples))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid ID supplied", Description = "Invalid ID supplied")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Goal not found", Description = "Goal not found")]
		public async Task<HttpResponseData> DeleteGoal([HttpTrigger(AuthorizationLevel.Function, "DELETE", Route = "goal/{goalId}")] HttpRequestData req, int goalId, FunctionContext executionContext)
		{
			// Generate output
			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			await GoalsService.DeleteGoal(goalId);

			return response;
		}
	}
}
