using Microsoft.AspNetCore.Mvc;
using MyMovieWeb.Application.DTOs.Requests;
using Stripe;
using Stripe.Checkout;

namespace MyMovieWeb.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("create-checkout-session")]
        public IActionResult CreateCheckoutSession([FromBody] CreatePaymentRequest paymentRequest)
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long?)paymentRequest.Amount,
                                Currency = "vnd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = paymentRequest.Metadata["movieTitle"],
                                    Images = [$"{paymentRequest.Metadata["moviePoster"]}"]
                                },
                            },
                            Quantity = 1,
                        },
                    },
                    Mode = "payment", // Kiểu giao dịch (payment, subscription, etc.)
                    SuccessUrl = $"{_configuration["Stripe:SuccessUrl"]}?session_id={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = _configuration["Stripe:CancelUrl"],
                };

                var service = new SessionService();
                var session = service.Create(options);

                return Ok(new { url = session.Url });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
