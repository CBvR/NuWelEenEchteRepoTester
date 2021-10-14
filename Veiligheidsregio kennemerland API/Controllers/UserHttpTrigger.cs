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
using System.Collections;

namespace Kennemerland.Controllers
{
	public class UserHttpTrigger
	{
		ILogger Logger { get; }
		IUsersService UsersService { get; }

		public UserHttpTrigger(IUsersService UsersService, ILogger<UserHttpTrigger> Logger)
		{
			this.Logger = Logger;
			this.UsersService = UsersService;
		}

		[Function(nameof(UserHttpTrigger.AddUser))]
		[OpenApiOperation(operationId: "addUser", tags: new[] { "user" }, Summary = "Add a new user for the Kennemerland application to the system", Description = "To define the user type, add a: <br /> 1 for Gebruiker <br /> 2 for Directie <br /> 3 for Teameleader", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("userstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiRequestBody(contentType: "application/json", bodyType: typeof(User), Example = typeof(DummyUserExample), Required = true, Description = "User object that needs to be added to the store")]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(User), Summary = "New user details added", Description = "New user details added", Example = typeof(DummyUserExample))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Conflict, Summary = "User already exists", Description = "User with this userId already exists")]
		public async Task<HttpResponseData> AddUser([HttpTrigger(AuthorizationLevel.Function, "POST", Route = "user")] HttpRequestData req, FunctionContext executionContext)
		{
			// Parse input
			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			User user = JsonConvert.DeserializeObject<User>(requestBody);

			user = await UsersService.CreateUser(user);

			HttpResponseData response;
			// Generate output
			if (user != null) response = req.CreateResponse(HttpStatusCode.OK);
			else response = req.CreateResponse(HttpStatusCode.Conflict);

			await response.WriteAsJsonAsync(user);

			return response;
		}

		[Function(nameof(UserHttpTrigger.UpdateUser))]
		[OpenApiOperation(operationId: "updateUser", tags: new[] { "user" }, Summary = "Update an existing user", Description = "This updates an existing user.", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("userstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiRequestBody(contentType: "application/json", bodyType: typeof(User), Required = true, Description = "User object that needs to be updated to the store")]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(User), Summary = "User details updated", Description = "User details updated", Example = typeof(DummyUserExample))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid ID supplied", Description = "Invalid ID supplied")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "User not found", Description = "User not found")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.MethodNotAllowed, Summary = "Validation exception", Description = "Validation exception")]
		public async Task<HttpResponseData> UpdateUser([HttpTrigger(AuthorizationLevel.Function, "PUT", Route = "user")] HttpRequestData req, FunctionContext executionContext)
		{
			// Parse input
			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			User user = JsonConvert.DeserializeObject<User>(requestBody);

			user = await UsersService.UpdateUser(user);

			// Generate output
			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			await response.WriteAsJsonAsync(user);

			return response;
		}

		[Function(nameof(UserHttpTrigger.GetUserById))]
		[OpenApiOperation(operationId: "getUserById", tags: new[] { "user" }, Summary = "Find user by ID", Description = "Returns a single user.", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("userstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiParameter(name: "userId", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID of user to return", Description = "ID of user to return", Visibility = OpenApiVisibilityType.Important)]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(User), Summary = "successful operation", Description = "successful operation", Example = typeof(DummyUserExamples))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid ID supplied", Description = "Invalid ID supplied")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "User not found", Description = "User not found")]
		public async Task<HttpResponseData> GetUserById([HttpTrigger(AuthorizationLevel.Function, "GET", Route = "user/{userId}")] HttpRequestData req, int userId, FunctionContext executionContext)
		{
			// Generate output
			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			User user = await UsersService.GetUser(userId);

			await response.WriteAsJsonAsync(user);

			return response;
		}

		[Function(nameof(UserHttpTrigger.GetUserTasks))]
		[OpenApiOperation(operationId: "getUserTasks", tags: new[] { "user" }, Summary = "Retrieve all users tasks from userId", Description = "Returns a single user's tasks from teams.", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("userstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiParameter(name: "userId", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID of user to return", Description = "ID of user to return", Visibility = OpenApiVisibilityType.Important)]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(User), Summary = "successful operation", Description = "successful operation", Example = typeof(DummyUserExamples))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid ID supplied", Description = "Invalid ID supplied")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "User not found", Description = "User not found")]
		public async Task<HttpResponseData> GetUserTasks([HttpTrigger(AuthorizationLevel.Function, "GET", Route = "user/getTasks/{userId}")] HttpRequestData req, int userId, FunctionContext executionContext)
		{
			// Generate output
			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			IEnumerable<Models.Task> userTasks = await UsersService.GetUserTasks(userId);

			await response.WriteAsJsonAsync(userTasks);

			return response;
		}

		[Function(nameof(UserHttpTrigger.GetUsers))]
		[OpenApiOperation(operationId: "getUserById", tags: new[] { "user" }, Summary = "Gets all users from the system", Description = "Retrieves all the users from the system <br />This should be only available to the direction", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("userstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(User), Summary = "successful operation", Description = "successful operation", Example = typeof(DummyUserExamples))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid ID supplied", Description = "Invalid ID supplied")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "User not found", Description = "User not found")]
		public async Task<HttpResponseData> GetUsers([HttpTrigger(AuthorizationLevel.Function, "GET", Route = "user")] HttpRequestData req, FunctionContext executionContext)
		{
			// Generate output
			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            IEnumerable user = await UsersService.GetAllUsers();

            await response.WriteAsJsonAsync(user);

            return response;
		}

		[Function(nameof(UserHttpTrigger.DeleteUser))]
		[OpenApiOperation(operationId: "deleteUser", tags: new[] { "user" }, Summary = "Delete user by userid", Description = "Deletes a single user.", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("teamstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiParameter(name: "userId", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID of user to return", Description = "ID of user to delete", Visibility = OpenApiVisibilityType.Important)]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(User), Summary = "successful operation", Description = "successful operation", Example = typeof(DummyUserExample))]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid ID supplied", Description = "Invalid ID supplied")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "User not found", Description = "User not found")]
		public async Task<HttpResponseData> DeleteUser([HttpTrigger(AuthorizationLevel.Function, "DELETE", Route = "user/{userId}")] HttpRequestData req, int userId, FunctionContext executionContext)
		{
			// Generate output
			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			//await TeamsService.DeleteUser(userId);

			return response;
		}
	}
}