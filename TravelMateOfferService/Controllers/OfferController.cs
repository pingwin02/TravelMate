using Microsoft.AspNetCore.Mvc;
using TravelMateOfferService.Models;
using TravelMateOfferService.Models.DTO;
using TravelMateOfferService.Services;

namespace TravelMateOfferService.Controllers;

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
}