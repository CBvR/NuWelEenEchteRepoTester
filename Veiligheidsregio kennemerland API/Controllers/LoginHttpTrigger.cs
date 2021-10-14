using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Kennemerland.Attributes;
using Models;
using Services;

namespace Kennemerland.Controllers {
    public class LoginHttpTrigger {
        ILogger Logger { get; }
        ITokenService TokenService { get; }
        IUsersService UserService { get; }


        public LoginHttpTrigger(ITokenService TokenService, ILogger<LoginHttpTrigger> Logger, IUsersService usersService)
        {
            this.TokenService = TokenService;
            this.UserService = usersService;
            this.Logger = Logger;
            this.UserService = usersService;
        }

        [Function(nameof(LoginHttpTrigger.login))]
        [OpenApiOperation(operationId: "login", tags: new[] { "login" }, Summary = "Login for a user", Description = "This method logs in the user, and retrieves a JWT bearer token.")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(LoginRequest), Required = true, Description = "The user credentials")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(LoginResult), Description = "Login success")]
        public async Task<HttpResponseData> login([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req, FunctionContext executionContext)
        {
            LoginRequest login = JsonConvert.DeserializeObject<LoginRequest>(await new StreamReader(req.Body).ReadToEndAsync());

            login.Password = UserService.GetHashString(login.Password);
            LoginResult result = await TokenService.CreateToken(login);

            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);

            return response;
        }
    }
}