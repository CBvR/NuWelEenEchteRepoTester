using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Functions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;
using DAL;

namespace Kennemerland.Startup {
	public class Program {
		public static void Main() {
			IHost host = new HostBuilder()
				.ConfigureFunctionsWorkerDefaults((IFunctionsWorkerApplicationBuilder Builder) => {
					Builder.UseNewtonsoftJson().UseMiddleware<JwtMiddleware>();
				})
				.ConfigureOpenApi()
				.ConfigureServices(Configure)
				.Build();

			host.Run();
		}

		static void Configure(HostBuilderContext Builder, IServiceCollection Services) {
			Services.AddSingleton<ITeamsService, TeamService>();
			Services.AddSingleton<IUsersService, UserService>();
			Services.AddSingleton<IGoalsService, GoalService>();
			Services.AddSingleton<ITasksService, TaskSerivice>();
			Services.AddSingleton<ITokenService, TokenService>();
			Services.AddSingleton<ITeamRepository, TeamRepository>();
			Services.AddSingleton<IUserRepository, UserRepository>();
			Services.AddSingleton<IGoalRepository, GoalRepository>();
			Services.AddSingleton<ITaskRepository, TaskRepository>();
		}
	}
}


