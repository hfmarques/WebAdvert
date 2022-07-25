using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WebAdvert.Web.Models;

namespace Web.ServiceClients
{
    public class SearchApiClient : ISearchApiClient
    {
        private readonly HttpClient client;
        private readonly string baseAddress;

        public SearchApiClient(HttpClient client, IConfiguration configuration)
        {
            this.client = client;
            baseAddress = configuration.GetSection("SearchApi").GetValue<string>("url");
        }

        public async Task<List<AdvertType>> Search(string keyword)
        {
            var result = new List<AdvertType>();
            var callUrl = $"{baseAddress}/search/v1/{keyword}";
            var httpResponse = await client.GetAsync(new Uri(callUrl)).ConfigureAwait(false);

            if (httpResponse.StatusCode != HttpStatusCode.OK) return result;
            var allAdverts =
                JsonSerializer.Deserialize<List<AdvertType>>(await httpResponse.Content.ReadAsStringAsync()
                    .ConfigureAwait(false));
            if (allAdverts != null) result.AddRange(allAdverts);

            return result;
        }
    }
}