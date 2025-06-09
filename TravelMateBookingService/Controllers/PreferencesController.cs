using Microsoft.AspNetCore.Mvc;
using TravelMateBookingService.Services;

namespace TravelMateBookingService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PreferencesController(IBookingService bookingService) : ControllerBase
{
    [HttpGet("destination-preferences")]
    public async Task<IActionResult> GetDestinationPreferences()
    {
        var preferences = await bookingService.GetDestinationPreferences();
        return Ok(preferences);
    }

    [HttpGet("offer-preferences")]
    public async Task<IActionResult> GetOfferPreferences()
    {
        var preferences = await bookingService.GetOfferPreferences();
        return Ok(preferences);
    }
}