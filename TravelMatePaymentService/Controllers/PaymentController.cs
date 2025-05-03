using Microsoft.AspNetCore.Mvc;
using TravelMatePaymentService.Services;

namespace TravelMatePaymentService.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class PaymentController(IPaymentService paymentService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentById(Guid id)
    {
        try
        {
            var payment = await paymentService.GetPaymentById(id);

            return Ok(payment);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpGet("{id}/pay")]
    public async Task<IActionResult> PayPayment(Guid id)
    {
        try
        {
            var isSuccess = await paymentService.FinalizePayment(id);

            if (isSuccess) return Ok("Payment finalized successfully");

            return BadRequest("Payment finalization failed");
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
    }
}