using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelMateBookingService.Controllers.Exceptions;
using TravelMateBookingService.Models.Bookings;
using TravelMateBookingService.Models.Bookings.DTO;
using TravelMateBookingService.Services;

namespace TravelMateBookingService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingController(IBookingService bookingService, IServiceProvider serviceProvider)
    : ControllerBase
{
    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDto booking)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var createdBooking = await bookingService.CreateBooking(Guid.Parse(userId), booking);
            return Created($"/api/booking/{createdBooking}", createdBooking);
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
        var bookings = await bookingService.GetBookingsByUserId(Guid.Parse(userId));
        return Ok(bookings);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetBookingById(Guid id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var booking = await bookingService.GetBookingById(Guid.Parse(userId), id);

            return Ok(booking);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> CancelBooking(Guid id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var booking = await bookingService.GetBookingById(Guid.Parse(userId), id);
            if (booking.Status is BookingStatus.Canceled or BookingStatus.Confirmed)
                return BadRequest("Booking is already canceled or confirmed");

            using var scope = serviceProvider.CreateScope();
            var expirationService = scope.ServiceProvider.GetRequiredService<BookingExpirationService>();
            await expirationService.CancelBooking(booking.Id, booking.SeatType, booking.OfferId);

            return Ok($"Booking {booking.Id} canceled");
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}