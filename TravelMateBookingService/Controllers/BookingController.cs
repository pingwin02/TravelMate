using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelMateBookingService.Controllers.Exceptions;
using TravelMateBookingService.Models.Bookings.DTO;
using TravelMateBookingService.Services;

namespace TravelMateBookingService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingController(IBookingService bookingService) : ControllerBase
{
    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDto booking)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var createdBooking = await bookingService.CreateBooking(Guid.Parse(userId), booking);
            return Ok(createdBooking);
        }
        catch (SeatNotAvailableException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("user-info")]
    [Authorize]
    public IActionResult GetUserInfo()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        return Ok(new { UserId = userId, UserName = userName, Roles = userRoles });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetBookingsByUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return BadRequest("User not logged in");
        var bookings = await bookingService.GetBookingsByUserId(Guid.Parse(userId));
        return Ok(bookings);
    }
}