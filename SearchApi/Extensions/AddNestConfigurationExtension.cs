using Nest;

namespace SearchApi.Extensions
{
    public static class AddNestConfigurationExtension
    {
        public static void AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
        {
            var elasticSearchUrl = configuration.GetSection("ES").GetValue<string>("url");

            var connectionSettings = new ConnectionSettings(new Uri(elasticSearchUrl))
                .DefaultIndex("adverts");

            var client = new ElasticClient(connectionSettings);

            services.AddSingleton<IElasticClient>(client);
        }
    }
}