using Microsoft.Extensions.Configuration;
using Nest;

namespace SearchWorker;

public class ElasticSearchHelper
{
    private static IElasticClient? client;

    public static ElasticClient GetInstance(IConfiguration configuration)
    {
        if (client != null) return (ElasticClient) client;

        var url = configuration["ES:Url"];
        var settings = new ConnectionSettings(new Uri(url)).DefaultIndex("adverts");
        client = new ElasticClient(settings);

        return (ElasticClient) client;
    }
}