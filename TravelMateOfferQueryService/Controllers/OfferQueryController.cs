using Microsoft.AspNetCore.Mvc;
using TravelMateOfferQueryService.Services;

namespace TravelMateOfferQueryService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OfferQueryController(IOfferQueryService offerQueryService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOffer(Guid id)
    {
        try
        {
            var offer = await offerQueryService.GetOffer(id);
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
        var offers = await offerQueryService.GetOffers();
        return Ok(offers);
    }
}