using System.Threading.Tasks;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctionDemo;

public static class MessageHandlerInFirstQueue
{
    [FunctionName("NewMessage")]
    public static async Task RunAsync([QueueTrigger("%FirstQueueName%")] Message message,
        [Blob("%FirstContainerName%/{FileName}", FileAccess.ReadWrite, Connection = "AzureWebJobsStorage")]
        TextWriter textWriter,
        [Queue("%SecondQueueName%", Connection = "SecondStorage")]
        ICollector<MessageSecondQueue> secondQueue,
        ILogger log)
    {
        await textWriter.WriteAsync(JsonConvert.SerializeObject(message));
        
        secondQueue.Add(new MessageSecondQueue()
        {
            SourceFileName = message.FileName,
            TargetNameFileName = $"{Path.GetFileNameWithoutExtension(message.FileName)}-processed{Path.GetExtension(message.FileName)}" 
        });
        
        log.LogInformation($"C# Queue trigger function processed: {message}");
    }
}