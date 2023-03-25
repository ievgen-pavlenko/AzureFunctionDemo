using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctionDemo;

public static class HttpInput
{
    [FunctionName("HttpInput")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
        HttpRequest req,
        [Queue("%FirstQueueName%", Connection = "AzureWebJobsStorage")]
        ICollector<Message> msg,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string name = req.Query["name"];
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        name ??= data?.name;
        name ??= "NoName";

        var blobName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.data";

        var message = new Message
        {
            Name = name,
            FileName = blobName
        };

        msg.Add(message);

        return new OkObjectResult($"Hello, {name}");
    }
}