using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Advert;
using Web.ServiceClients;
using Web.Services;

namespace Web.Controllers
{
    public class AdvertManagementController : Controller
    {
        private readonly IFileUploader fileUploader;
        private readonly IAdvertApiClient advertApiClient;
        private readonly IMapper mapper;

        public AdvertManagementController(IFileUploader fileUploader, IAdvertApiClient advertApiClient, IMapper mapper)
        {
            this.fileUploader = fileUploader;
            this.advertApiClient = advertApiClient;
            this.mapper = mapper;
        }

        public IActionResult Create(CreateAdvertViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdvertViewModel model, IFormFile imageFile)
        {
            if (!ModelState.IsValid) return View(model);

            var createAdvertModel = mapper.Map<CreateAdvertModel>(model);
            createAdvertModel.UserName = User.Identity.Name;

            var apiCallResponse = await advertApiClient.CreateAsync(createAdvertModel).ConfigureAwait(false);
            var id = apiCallResponse.Id;

            var isOkToConfirmAd = true;
            var filePath = string.Empty;
            if (imageFile != null)
            {
                var fileName = !string.IsNullOrEmpty(imageFile.FileName)
                    ? Path.GetFileName(imageFile.FileName)
                    : id;
                filePath = $"{id}/{fileName}";

                try
                {
                    await using var readStream = imageFile.OpenReadStream();
                    var result = await fileUploader.UploadFileAsync(filePath, readStream)
                        .ConfigureAwait(false);
                    if (!result)
                        throw new Exception(
                            "Could not upload the image to file repository. Please see the logs for details.");
                }
                catch (Exception e)
                {
                    isOkToConfirmAd = false;
                    var confirmModel = new ConfirmAdvertRequest()
                    {
                        Id = id,
                        FilePath = filePath,
                        Status = AdvertStatus.Pending
                    };
                    await advertApiClient.ConfirmAsync(confirmModel).ConfigureAwait(false);
                    Console.WriteLine(e);
                }
            }

            if (!isOkToConfirmAd) return RedirectToAction("Index", "Home");
            {
                var confirmModel = new ConfirmAdvertRequest()
                {
                    Id = id,
                    FilePath = filePath,
                    Status = AdvertStatus.Active
                };
                await advertApiClient.ConfirmAsync(confirmModel).ConfigureAwait(false);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}