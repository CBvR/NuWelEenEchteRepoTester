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
	public class ReportHttpTrigger {
		ILogger Logger { get; }
		IReportService ReportsService { get; }

		public ReportHttpTrigger(IReportService ReportsService, ILogger<ReportHttpTrigger> Logger) {
			this.Logger = Logger;
			//this.ReportsService = ReportsService;
		}

		public class reportFilter
        {
			[OpenApiProperty(Description = "Gets or sets the pet ID.")]
			[JsonRequired]
			public int Example { get; set; }

			[OpenApiProperty(Description = "Gets or sets the pet ID.")]
			[JsonRequired]
			public int Example2 { get; set; }
		}

		[Function(nameof(ReportHttpTrigger.AddReport))]
		[OpenApiOperation(operationId: "addReport", tags: new[] { "report" }, Summary = "Create goal progress report", Description = "This will start the creation of a direction report to see the progress of filtered tasks", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("reportstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiParameter(name: "reportFilter", In = ParameterLocation.Path, Required = true, Type = typeof(reportFilter), Summary = "ID of report to return", Description = "ID of report to return", Visibility = OpenApiVisibilityType.Important)]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.MethodNotAllowed, Summary = "Invalid input", Description = "Invalid input")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Summary = "Report created", Description = "The report is created. Get the report with the getReport function")]
		public async Task<HttpResponseData> AddReport([HttpTrigger(AuthorizationLevel.Function, "POST", Route = "report")] HttpRequestData req, FunctionContext executionContext) {
			// Parse input
			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

			await ReportsService.CreateReport();

			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			return response;
		}

		[Function(nameof(ReportHttpTrigger.GetReportById))]
		[OpenApiOperation(operationId: "getReportById", tags: new[] { "report" }, Summary = "Retrieve created report from the blob storage", Description = "Returns the created report from the blob storage if created forehand", Visibility = OpenApiVisibilityType.Important)]
		//[OpenApiSecurity("goalstore_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
		[OpenApiParameter(name: "reportId", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID of report to return", Description = "ID of report to return", Visibility = OpenApiVisibilityType.Important)]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid ID supplied", Description = "Invalid ID supplied")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Report not found", Description = "Goal not found")]
		[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Summary = "Report found", Description = "Report found")]
		public async Task<HttpResponseData> GetReportById([HttpTrigger(AuthorizationLevel.Function, "GET", Route = "report/{reportId}")] HttpRequestData req, int reportId, FunctionContext executionContext)
		{
			// Generate output
			HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

			await ReportsService.GetReport(reportId);

			//await response.WriteAsJsonAsync(goal);

			return response;
		}
	}
}
