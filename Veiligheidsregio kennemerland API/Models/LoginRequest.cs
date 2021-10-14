using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kennemerland.Models
{
	[OpenApiExample(typeof(LoginRequestExample))]
	public class LoginRequest
	{
		[OpenApiProperty(Description = "Username.")]
		[JsonRequired]
		public string Username { get; set; }

		[OpenApiProperty(Description = "Password.")]
		[JsonRequired]
		public string Password { get; set; }
	}

	public class LoginRequestExample : OpenApiExample<LoginRequest> {
		public override IOpenApiExample<LoginRequest> Build(NamingStrategy NamingStrategy = null) {
			Examples.Add(OpenApiExampleResolver.Resolve("LoginExample",
														new LoginRequest() {
															Username = "KjellPepping",
															Password = "Test101"
														},
														NamingStrategy));

			return this;
		}
	}

}
