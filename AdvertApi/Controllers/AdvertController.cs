using AdvertApi.Services;
using Microsoft.AspNetCore.Mvc;
using Models.Advert;

namespace AdvertApi.Controllers;

[ApiController]
[Route("adverts/v1")]
public class AdvertController : ControllerBase
{
    private readonly IAdvertStorageService advertStorageService;

    public AdvertController(IAdvertStorageService advertStorageService)
    {
        this.advertStorageService = advertStorageService;
    }

    [HttpPost]
    [ProducesResponseType(404)]
    [ProducesResponseType(201, Type = typeof(CreateAdvertResponse))]
    public async Task<IActionResult> Create(AdvertModel model)
    {
        try
        {
            var recordId = await advertStorageService.Add(model);
            return Created("", new CreateAdvertResponse {Id = recordId});
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut]
    [ProducesResponseType(404)]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Confirm(ConfirmAdvertModel model)
    {
        try
        {
            await advertStorageService.Confirm(model);
            return Ok(model);
        }
        catch (KeyNotFoundException)
        {
            return new NotFoundResult();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}