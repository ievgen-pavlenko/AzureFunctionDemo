using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureFunctionDemo;

public static class MessageHandlerInSecondQueue
{
    [FunctionName("NewMessageInSecondQueue")]
    public static async Task RunAsync(
        [QueueTrigger("%SecondQueueName%", Connection = "SecondStorage")] 
        MessageSecondQueue message,
        [Blob("%FirstContainerName%/{SourceFileName}", FileAccess.ReadWrite, Connection = "AzureWebJobsStorage")]
        TextReader textReader,
        [Blob("%SecondContainerName%/{TargetNameFileName}", FileAccess.ReadWrite, Connection = "SecondStorage")]
        TextWriter textWriter,
        ILogger log)
    {
        log.LogInformation($"Queue trigger in second queue have begin");
        
        var sb = new StringBuilder();
        sb.Append(await textReader.ReadToEndAsync());
        sb.Append("TimeStamp:");
        sb.Append(DateTime.Now.ToString(CultureInfo.InvariantCulture));
        
        await textWriter.WriteAsync(sb.ToString());
        
        log.LogInformation($"Queue trigger in second queue have finished");
    }
}