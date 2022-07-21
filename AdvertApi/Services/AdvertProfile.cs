using AutoMapper;
using Models.Advert;

namespace AdvertApi.Services;

public class AdvertProfile : Profile
{
    public AdvertProfile()
    {
        CreateMap<AdvertModel, AdvertDbModel>();
    }
}