using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelMateBookingService.Models.Bookings;
using TravelMateBookingService.Models.Bookings.DTO;
using TravelMateBookingService.Services;

namespace TravelMateBookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDto booking)
        {
            if (booking == null) return BadRequest();
            var createdBooking = await _bookingService.CreateBooking(booking);
            return Ok(createdBooking);
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetBookingsByUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return BadRequest("User not logged");
            var bookings = await _bookingService.GetBookingsByUserId(Guid.Parse(userId));
            return Ok(bookings);
        }
    }
}
