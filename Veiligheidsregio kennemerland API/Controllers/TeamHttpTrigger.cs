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
using System;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System.Collections;

namespace Kennemerland.Controllers {
	public class TeamHttpTrigger {
		ILogger Logger { get; }
		ITeamsService TeamsService { get; }

		public TeamHttpTrigger(ITeamsService TeamsService, ILogger<TeamHttpTrigger> Logger) {
			this.Logger = Logger;
			this.TeamsService = TeamsService;
		}

		[Function(nameof(TeamHttpTrigger.AddTeam))]
		[OpenApiOperation(operationId: "addTeam", tags: new[] { "team" }, Summary = "Add a new team to the store", Description = "This add a new team to the store.", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("teamstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Team), Example = typeof(DummyTeamExample), Required = true, Description = "Team object that needs to be added to the store")]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Team), Summary = "New team details added", Description = "New team details added", Example = typeof(DummyTeamExample))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Conflict, Summary = "Team id already exists", Description = "There is already a team with this userid")]
		public async Task<HttpResponseData> AddTeam([HttpTrigger(AuthorizationLevel.Function, "POST", Route = "team")] HttpRequestData req, FunctionContext executionContext) {
			// Parse input
			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			Team team = JsonConvert.DeserializeObject<Team>(requestBody);
			team = await TeamsService.CreateTeam(team);

			HttpResponseData response;
			// Generate output
			if (team != null) response = req.CreateResponse(HttpStatusCode.OK);
			else response = req.CreateResponse(HttpStatusCode.Conflict);

			await response.WriteAsJsonAsync(team);

			return response;
		}

		[Function(nameof(TeamHttpTrigger.UpdateTeam))]
		[OpenApiOperation(operationId: "updateTeam", tags: new[] { "team" }, Summary = "Update an existing team", Description = "This updates an existing team.", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("teamstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Team), Required = true, Description = "Team object that needs to be updated to the store")]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Team), Summary = "Team details updated", Description = "Team details updated", Example = typeof(DummyTeamExample))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid ID supplied", Description = "Invalid ID supplied")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Team not found", Description = "Team not found")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.MethodNotAllowed, Summary = "Validation exception", Description = "Validation exception")]
		public async Task<HttpResponseData> UpdateTeam([HttpTrigger(AuthorizationLevel.Function, "PUT", Route = "team")] HttpRequestData req, FunctionContext executionContext) {
			// Parse input
			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			Team team = JsonConvert.DeserializeObject<Team>(requestBody);

			team = await TeamsService.UpdateTeam(team);

			// Generate output
			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			await response.WriteAsJsonAsync(team);

			return response;
		}

		[Function(nameof(TeamHttpTrigger.AddUserToTeam))]
		[OpenApiOperation(operationId: "addUserToTeam", tags: new[] { "team" }, Summary = "Assign a user to the team", Description = "This endpoint receives a team id and a user id to assign the user to a team", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("teamstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiRequestBody(contentType: "application/json", bodyType: typeof(AddUserToTeamObject), Required = true, Description = "Team object that needs to be updated to the store")]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(AddUserToTeamObject), Summary = "Team details updated", Description = "Team details updated", Example = typeof(DummyAddUserToTeamObjectExample))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid ID supplied", Description = "Invalid ID supplied")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Team not found", Description = "Team not found")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.MethodNotAllowed, Summary = "Validation exception", Description = "Validation exception")]
		public async Task<HttpResponseData> AddUserToTeam([HttpTrigger(AuthorizationLevel.Function, "PUT", Route = "team/addUser")] HttpRequestData req, FunctionContext executionContext)
		{
			// Parse input
			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			AddUserToTeamObject addUserObject = JsonConvert.DeserializeObject<AddUserToTeamObject>(requestBody);

			var team = await TeamsService.AddUserToTeam(addUserObject.TeamId, addUserObject.UserId);

			// Generate output
			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			await response.WriteAsJsonAsync(addUserObject);

			return response;
		}

		[Function(nameof(TeamHttpTrigger.GetTeamById))]
		[OpenApiOperation(operationId: "getTeamById", tags: new[] { "team" }, Summary = "Find team by ID", Description = "Returns a single team.", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("teamstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiParameter(name: "teamId", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID of team to return", Description = "ID of team to return", Visibility = OpenApiVisibilityType.Important)]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Team), Summary = "successful operation", Description = "successful operation", Example = typeof(DummyTeamExamples))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid ID supplied", Description = "Invalid ID supplied")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Team not found", Description = "Team not found")]
		public async Task<HttpResponseData> GetTeamById([HttpTrigger(AuthorizationLevel.Function, "GET", Route = "team/{teamId}")] HttpRequestData req, int teamId, FunctionContext executionContext) {
			// Generate output
			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			Team team = await TeamsService.GetTeam(teamId);

			await response.WriteAsJsonAsync(team);

			return response;
		}

		[Function(nameof(TeamHttpTrigger.GetTeams))]
		[OpenApiOperation(operationId: "getTeamById", tags: new[] { "team" }, Summary = "Get all teams from the system", Description = "Recovers all the teams from the system <br />This should only be available to the direction", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("teamstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Team), Summary = "successful operation", Description = "successful operation", Example = typeof(DummyTeamExamples))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid ID supplied", Description = "Invalid ID supplied")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Team not found", Description = "Team not found")]
		public async Task<HttpResponseData> GetTeams([HttpTrigger(AuthorizationLevel.Function, "GET", Route = "team")] HttpRequestData req, FunctionContext executionContext)
		{
			// Generate output
			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			IEnumerable<Team> teams = await TeamsService.GetAllTeams();

			await response.WriteAsJsonAsync(teams);

			return response;
		}

		[Function(nameof(TeamHttpTrigger.DeleteTeam))]
		[OpenApiOperation(operationId: "deleteTeam", tags: new[] { "team" }, Summary = "Find team by ID", Description = "Returns a single team.", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("teamstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiParameter(name: "teamId", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID of team to return", Description = "ID of team to return", Visibility = OpenApiVisibilityType.Important)]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Team), Summary = "successful operation", Description = "successful operation", Example = typeof(DummyTeamExamples))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid ID supplied", Description = "Invalid ID supplied")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Team not found", Description = "Team not found")]
		public async Task<HttpResponseData> DeleteTeam([HttpTrigger(AuthorizationLevel.Function, "DELETE", Route = "team/{teamId}")] HttpRequestData req, int teamId, FunctionContext executionContext)
		{
			// Generate output
			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			await TeamsService.DeleteTeam(teamId);

			return response;
		}
	}
}
