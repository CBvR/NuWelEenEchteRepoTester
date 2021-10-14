using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;

namespace Kennemerland.Attributes {
	public class KennemerlandAuthAttribute : OpenApiSecurityAttribute {
		public KennemerlandAuthAttribute() : base("KennemerlandAuth", SecuritySchemeType.Http) {
			Description = "JWT for authorization";
			In = OpenApiSecurityLocationType.Header;
			Scheme = OpenApiSecuritySchemeType.Bearer;
			BearerFormat = "JWT";
		}
	}
}
