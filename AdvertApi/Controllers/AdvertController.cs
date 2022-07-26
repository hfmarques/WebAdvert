using System.Text.Json;
using AdvertApi.Services;
using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Mvc;
using Models.Advert;
using Models.Messages;

namespace AdvertApi.Controllers;

[ApiController]
[Route("adverts/v1")]
public class AdvertController : ControllerBase
{
    private readonly IAdvertStorageService advertStorageService;
    private readonly IConfiguration configuration;

    public AdvertController(IAdvertStorageService advertStorageService, IConfiguration configuration)
    {
        this.advertStorageService = advertStorageService;
        this.configuration = configuration;
    }

    [HttpPost]
    [ProducesResponseType(404)]
    [ProducesResponseType(201, Type = typeof(CreateAdvertResponse))]
    public async Task<IActionResult> Create(AdvertModel model)
    {
        try
        {
            var recordId = await advertStorageService.AddAsync(model);
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
            await advertStorageService.ConfirmAsync(model);
            await RaiseAdvertConfirmedMessage(model);
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

    private async Task RaiseAdvertConfirmedMessage(ConfirmAdvertModel model)
    {
        var topicArn = configuration["TopicArn"];

        using var client = new AmazonSimpleNotificationServiceClient();
        var dbModel = await advertStorageService.GetByIdAsync(model.Id);

        var message = new AdvertConfirmedMessage
        {
            Id = model.Id,
            Title = dbModel.Title,
        };
        await client.PublishAsync(topicArn, JsonSerializer.Serialize(message));
    }
}