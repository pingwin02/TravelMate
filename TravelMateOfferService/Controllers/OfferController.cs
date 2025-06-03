using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TravelMateOfferService.Hubs;
using TravelMateOfferService.Models;
using TravelMateOfferService.Models.DTO;
using TravelMateOfferService.Services;

namespace TravelMateOfferService.Controllers;

[ApiController]
[Route("api/[controller]s")]
public class OfferController(IOfferService offerService, IHubContext<OfferHub> hubContext) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOffer(Guid id)
    {
        try
        {
            var offer = await offerService.GetOffer(id);
            return Ok(offer);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetOffers()
    {
        var offers = await offerService.GetOffers();
        return Ok(offers);
    }

    [HttpPost]
    public async Task<IActionResult> AddOffer([FromBody] OfferRequestDto offer)
    {
        try
        {
            var result = await offerService.AddOffer(offer);
            await hubContext.Clients.All.SendAsync("OfferAdded", offer);
            return Created($"/api/offers/{result}", result);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateOffer([FromBody] Offer offer)
    {
        try
        {
            var changeDto = new OfferChangeDto
            {
                OldOffer = await offerService.GetOffer(offer.Id),
                NewOffer = offer
            };
            await offerService.UpdateOffer(offer);
            await hubContext.Clients.All.SendAsync("OfferUpdated", changeDto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOffer(Guid id)
    {
        try
        {
            await offerService.DeleteOffer(id);
            await hubContext.Clients.All.SendAsync("OfferDeleted", id);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}