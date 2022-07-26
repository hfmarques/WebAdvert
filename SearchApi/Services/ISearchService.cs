using WebAdvert.Web.Models;

namespace SearchApi.Services
{
    public interface ISearchService
    {
        Task<List<AdvertType>> Search(string keyword);
    }
}