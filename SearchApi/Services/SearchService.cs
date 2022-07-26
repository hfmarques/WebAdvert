using Nest;
using WebAdvert.Web.Models;

namespace SearchApi.Services
{
    public class SearchService : ISearchService
    {
        private readonly IElasticClient client;

        public SearchService(IElasticClient client)
        {
            this.client = client;
        }

        public async Task<List<AdvertType>> Search(string keyword)
        {
            var searchResponse = await client.SearchAsync<AdvertType>(search =>
                search.Query(query => query.Term(field => field.Title, keyword.ToLower())
                ));

            return searchResponse.Hits.Select(hit => hit.Source).ToList();
        }
    }
}