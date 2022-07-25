using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Models.Advert;

namespace Web.ServiceClients
{
    public class AdvertApiClient : IAdvertApiClient
    {
        private readonly string baseAddress;
        private readonly HttpClient client;
        private readonly IMapper mapper;

        public AdvertApiClient(IConfiguration configuration, HttpClient client, IMapper mapper)
        {
            this.client = client;
            this.mapper = mapper;

            baseAddress = configuration.GetSection("AdvertApi").GetValue<string>("BaseUrl");
        }

        public async Task<AdvertResponse> CreateAsync(CreateAdvertModel model)
        {
            var advertApiModel = mapper.Map<AdvertModel>(model);

            var jsonModel = JsonSerializer.Serialize(advertApiModel);
            var response = await client.PostAsync(new Uri($"{baseAddress}/create"),
                new StringContent(jsonModel, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            var createAdvertResponse =
                JsonSerializer.Deserialize<CreateAdvertResponse>(await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false));
            var advertResponse = mapper.Map<AdvertResponse>(createAdvertResponse);

            return advertResponse;
        }

        public async Task<bool> ConfirmAsync(ConfirmAdvertRequest model)
        {
            var advertModel = mapper.Map<ConfirmAdvertModel>(model);
            var jsonModel = JsonSerializer.Serialize(advertModel);
            var response = await client
                .PutAsync(new Uri($"{baseAddress}/confirm"),
                    new StringContent(jsonModel, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<List<Advertisement>> GetAllAsync()
        {
            var apiCallResponse = await client.GetAsync(new Uri($"{baseAddress}/all")).ConfigureAwait(false);
            var allAdvertModels =
                JsonSerializer.Deserialize<List<AdvertModel>>(await apiCallResponse.Content.ReadAsStringAsync()
                    .ConfigureAwait(false));
            return allAdvertModels.Select(x => mapper.Map<Advertisement>(x)).ToList();
        }

        public async Task<Advertisement> GetAsync(string advertId)
        {
            var apiCallResponse = await client.GetAsync(new Uri($"{baseAddress}/{advertId}")).ConfigureAwait(false);
            var fullAdvert =
                JsonSerializer.Deserialize<AdvertModel>(await apiCallResponse.Content.ReadAsStringAsync()
                    .ConfigureAwait(false));
            return mapper.Map<Advertisement>(fullAdvert);
        }
    }
}