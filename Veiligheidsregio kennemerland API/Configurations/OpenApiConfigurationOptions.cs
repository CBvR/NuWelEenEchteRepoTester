using System;

using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace Kennemerland.Configurations {
	public class OpenApiConfigurationOptions : DefaultOpenApiConfigurationOptions {
		public override OpenApiInfo Info { get; set; } = new OpenApiInfo() {
			Version = "3.0.0",
			Title = "API for Safety region Kennemerland",
			Description = "This is the API that corresponds with the Cloud minor of year 4.1 of Computer Science Inholland",
			//TermsOfService = new Uri("https://github.com/Azure/azure-functions-openapi-extension"),
			Contact = new OpenApiContact() {
				Name = "Arthur Nijlant",
				Email = "627687@student.inholland.nl",
			},
			License = new OpenApiLicense() {
				Name = "MIT",
				Url = new Uri("http://opensource.org/licenses/MIT"),
			}
		};

		public override OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V3;
	}
}
