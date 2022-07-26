using Microsoft.AspNetCore.Mvc;
using SearchApi.Services;
using WebAdvert.Web.Models;

namespace SearchApi.Controllers
{
    [Route("search/v1")]
    [ApiController]
    [Produces("application/json")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService searchService;
        private readonly ILogger<SearchController> logger;

        public SearchController(ISearchService searchService, ILogger<SearchController> logger)
        {
            this.searchService = searchService;
            this.logger = logger;
        }

        [HttpGet]
        [Route("{keyword}")]
        public async Task<List<AdvertType>> Get(string keyword)
        {
            logger.LogInformation("Search method was called");
            return await searchService.Search(keyword);
        }
    }
}