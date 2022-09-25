using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using System.Net;
using MTI860_collector.Models;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace MTI860_collector
{
    public static class Collect
    {
        [FunctionName("collect")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "username" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Username** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "vr-data", collectionName: "collect-data",
            ConnectionStringSetting = "CosmosDbConnectionString"
            )]IAsyncCollector<dynamic> documentsOut,
            [CosmosDB(
                databaseName: "vr-data", collectionName: "users",
                ConnectionStringSetting = "CosmosDbConnectionString",
                SqlQuery = "SELECT * FROM c ORDER BY c._ts desc")]
                IEnumerable<dynamic> users,
            [CosmosDB(
                databaseName: "vr-data", collectionName: "collect-data",
                ConnectionStringSetting = "CosmosDbConnectionString",
                SqlQuery = "SELECT * FROM c ORDER BY c._ts desc")]
                IEnumerable<dynamic> collects,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CollectRequest cRequest = JsonConvert.DeserializeObject<CollectRequest>(requestBody);

            var userExists = users.Any(x => x.username == cRequest.Username);
            if (!userExists) return new BadRequestObjectResult($"username {cRequest.Username} non-existing");

            var collect = collects­.FirstOrDefault(x => x.username == cRequest.Username);
            if(collect != null) return new BadRequestObjectResult($"Collect already happened for username {cRequest.Username}");

            await documentsOut.AddAsync(new
            {
                id = System.Guid.NewGuid().ToString(),
                username = cRequest.Username,
                circles = new
                {
                    average = cRequest.Circles.Average(),
                    data = cRequest.Circles
                },
                triangles = new
                {
                    average = cRequest.Triangles.Average(),
                    data = cRequest.Triangles
                },
                stars = new
                {
                    average = cRequest.Stars.Average(),
                    data = cRequest.Stars
                }
            });

            return new OkResult();
        }
    }
}
