using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TravelMateBookingService.Services;

namespace TravelMateBookingService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PreferencesController(IBookingService bookingService) : ControllerBase
{

    [HttpGet("departure-preferences")]
    public async Task<IActionResult> GetDeparturePreferences()
    {
        var preferences = await bookingService.GetDeparturePreferences();
        return Ok(preferences);
    }
}

