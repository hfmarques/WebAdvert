using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using Amazon.Lambda.SNSEvents;

namespace SearchWorker;

[assembly: LambdaSerializer(typeof(JsonSerializer))]
public class SearchWorker
{
    public void Function(SNSEvent snsEvent, ILambdaContext context)
    {
        foreach (var record in snsEvent.Records)
        {
            context.Logger.LogLine(record.Sns.Message);
        }
    }
}