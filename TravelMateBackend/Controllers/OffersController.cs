using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TravelMateBackend.Models.Entities;
using TravelMateBackend.Services.Offers;

namespace TravelMateBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OffersController : ControllerBase
    {
        private readonly IOfferService _offerService;
        public OffersController(IOfferService offerService)
        {
            _offerService = offerService;
        }
        [HttpGet]
        public async Task<IActionResult> GetOffers()
        {
            var offers = await _offerService.GetOffers();
            return Ok(offers);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOffer(Guid id)
        {
            var offer = await _offerService.GetOffer(id);
            return Ok(offer);
        }
        [HttpPost]
        public async Task<IActionResult> AddOffer([FromForm] Offer offer)
        {
            var result = await _offerService.AddOffer(offer);
            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateOffer([FromForm] Offer offer)
        {
            var result = await _offerService.UpdateOffer(offer);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOffer(int id)
        {
            var result = await _offerService.DeleteOffer(id);
            return Ok(result);
        }
    }
}
