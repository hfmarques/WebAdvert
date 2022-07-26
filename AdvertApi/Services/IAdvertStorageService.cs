using Models.Advert;

namespace AdvertApi.Services;

public interface IAdvertStorageService
{
    Task<string> AddAsync(AdvertModel model);
    Task<bool> CheckHealthAsync();
    Task ConfirmAsync(ConfirmAdvertModel model);
    Task<List<AdvertModel>> GetAllAsync();
    Task<AdvertModel> GetByIdAsync(string id);
}