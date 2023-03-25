using System.Collections.Generic;
using System.IO;
using System.Linq;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctionDemo;

public static class GetAllContainerObjects
{
    [FunctionName("GetAllContainerObjects")]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
        HttpRequest req,
        [Blob("%FirstContainerName%", FileAccess.Read, Connection = "AzureWebJobsStorage")]
        IEnumerable<BlobClient> blobs,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult(string.Join(',', blobs.Select(blob => blob.Name).ToArray()));
    }
}