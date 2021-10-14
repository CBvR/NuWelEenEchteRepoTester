using System.Net;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace Kennemerland.Attributes {
	public class ForbiddenResponseAttribute : OpenApiResponseWithBodyAttribute {
		public ForbiddenResponseAttribute() : base(HttpStatusCode.Forbidden, "text/plain", typeof(string)) {
			this.Description = "User login does not permit access to this function.";
		}
	}
}