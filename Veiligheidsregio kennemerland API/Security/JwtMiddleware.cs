using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Functions;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Services;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kennemerland.Startup {
    public class JwtMiddleware : IFunctionsWorkerMiddleware {
        ITokenService TokenService { get; }
        ILogger Logger { get; }

        public JwtMiddleware(ITokenService TokenService, ILogger<JwtMiddleware> Logger) {
            this.TokenService = TokenService;
            this.Logger = Logger;
        }

        public async Task Invoke(FunctionContext Context, FunctionExecutionDelegate Next) {
            string HeadersString = (string)Context.BindingContext.BindingData["Headers"];

            Dictionary<string, string> Headers = JsonConvert.DeserializeObject<Dictionary<string, string>>(HeadersString);

            if (Headers.TryGetValue("Authorization", out string AuthorizationHeader)) {
                try {
                    AuthenticationHeaderValue BearerHeader = AuthenticationHeaderValue.Parse(AuthorizationHeader);

                    ClaimsPrincipal User = await TokenService.GetByValue(BearerHeader.Parameter);
                    Console.WriteLine(User.IsInRole("Teamleader"));
                    Context.Items["User"] = User;
                }
                catch (Exception e) {
                    Logger.LogError(e.Message);
                }
            }

            await Next(Context);
        }

    }
}


