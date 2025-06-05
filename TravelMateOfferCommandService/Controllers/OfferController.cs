using Microsoft.AspNetCore.Mvc;
using TravelMateOfferCommandService.Models;
using TravelMateOfferCommandService.Models.DTO;
using TravelMateOfferCommandService.Services;

namespace TravelMateOfferCommandService.Controllers;

[ApiController]
[Route("api/[controller]s")]
public class OfferController(IOfferService offerService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddOffer([FromBody] OfferRequestDto offer)
    {
        try
        {
            var result = await offerService.AddOffer(offer);
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
            await offerService.UpdateOffer(offer);
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
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

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
}