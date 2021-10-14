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
using Task = System.Threading.Tasks.Task;

namespace Services {
    // test
	public interface IReportService {
		Task CreateReport();
        Task GetReport(long reportId);
    }

    public class ReportService : IReportService
    {
        public Task CreateReport()
        {
            throw new System.NotImplementedException();
        }

        public Task GetReport(long reportId)
        {
            throw new System.NotImplementedException();
        }
    }
}
