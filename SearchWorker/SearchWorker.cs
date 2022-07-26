using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using Amazon.Lambda.SNSEvents;
using Models.Messages;
using Nest;
using WebAdvert.Web.Models;

[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace SearchWorker;

public class SearchWorker
{
    private readonly IElasticClient client;

    public SearchWorker(IElasticClient client)
    {
        this.client = client;
    }

    public SearchWorker() : this(ElasticSearchHelper.GetInstance(ConfigurationHelper.Instance))
    {
    }

    public async Task Function(SNSEvent snsEvent, ILambdaContext context)
    {
        foreach (var record in snsEvent.Records)
        {
            context.Logger.LogLine(record.Sns.Message);

            var message = System.Text.Json.JsonSerializer.Deserialize<AdvertConfirmedMessage>(record.Sns.Message);
            var advertDocument = new AdvertType
            {
                Id = message?.Id ?? "",
                Title = message?.Title ?? "",
                CreationDateTime = DateTime.UtcNow
            };

            await client.IndexDocumentAsync(advertDocument);
        }
    }
}