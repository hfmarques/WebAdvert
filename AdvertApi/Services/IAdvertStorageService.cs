﻿using Models.Advert;

namespace AdvertApi.Services;

public interface IAdvertStorageService
{
    Task<string> Add(AdvertModel model);
    Task Confirm(ConfirmAdvertModel model);
}