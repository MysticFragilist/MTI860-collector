using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MTI860_collector.Models;
using Newtonsoft.Json;

namespace MTI860_collector
{
    public class CreateUser
    {
        private readonly ILogger<CreateUser> _logger;

        public CreateUser(ILogger<CreateUser> log)
        {
            _logger = log;
        }

        [FunctionName("createUser")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = "user/create")] HttpRequest req,
            [CosmosDB(databaseName: "vr-data", collectionName: "users",
            ConnectionStringSetting = "CosmosDbConnectionString"
            )]IAsyncCollector<dynamic> documentsOut,
            [CosmosDB(
                databaseName: "vr-data", collectionName: "users",
                ConnectionStringSetting = "CosmosDbConnectionString",
                SqlQuery = "SELECT * FROM c ORDER BY c._ts desc")]
                IEnumerable<dynamic> users)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            int nextUsername = 0;

            if (!users.Any())
            {
                nextUsername = 1;
            }
            else
            {
                var lastUser = users.OrderByDescending(x => x.createdBy).FirstOrDefault();
                nextUsername = lastUser.username + 1;
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            await documentsOut.AddAsync(new
            {
                id = Guid.NewGuid().ToString(),
                createdBy = DateTime.UtcNow,
                username = nextUsername,
            });


            return new OkObjectResult(nextUsername);
        }
    }
}

