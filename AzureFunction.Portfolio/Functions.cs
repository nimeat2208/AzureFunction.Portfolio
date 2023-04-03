using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunction.Portfolio
{
    public class Functions
    {
        private readonly ILogger _logger;

        public Functions(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Functions>();
        }

        [Function("PostEmailFunction")]
        public async Task<HttpResponseData> PostEmailFunction([HttpTrigger(
            AuthorizationLevel.Function,
            "get", "post",
            Route = "postemail")]
        HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                Name = "Azure Function [PostEmailFunction]",
                CurrentTime = DateTime.UtcNow
            }
            );

            return response;
        }
    }
}
