using System.Collections.Generic;
using System.Threading.Tasks;
using WebAdvert.Web.Models;

namespace Web.ServiceClients
{
    public interface ISearchApiClient
    {
        Task<List<AdvertType>> Search(string keyword);
    }
}
