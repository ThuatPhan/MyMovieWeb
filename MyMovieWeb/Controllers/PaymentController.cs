using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Presentation.Response;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace MyMovieWeb.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentController> _logger;
        private readonly string _webHooksSecret;
        private readonly IOrderServices _orderServices;

        public PaymentController(
            ILogger<PaymentController> logger,
            IConfiguration configuration,
            IOrderServices orderServices
        )
        {
            _configuration = configuration;
            _logger = logger;
            _webHooksSecret = _configuration["Stripe:WebhookSecret"]!;
            _orderServices = orderServices;
        }

        [HttpPost("create-intent")]
        [Authorize]
        public IActionResult CreatePaymentIntent([FromBody] PaymentRequestDTO request)
        {
            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = request.Amount,
                    Currency = "vnd",
                    PaymentMethodTypes = new List<string> { "card" },
                    Metadata = new Dictionary<string, string>
                    {
                        { "movieId", request.Metadata["movieId"] },
                        { "userId", $"{User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value}" }
                    }
                };

                var service = new PaymentIntentService();
                var paymentIntent = service.Create(options);

                return Ok(new { paymentIntent.ClientSecret });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("create-checkout-session")]
        [Authorize]
        public ActionResult<ApiResponse<string>> CreateCheckoutSession([FromBody] PaymentRequestDTO paymentRequest)
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
                                UnitAmount = paymentRequest.Amount,
                                Currency = "vnd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = $"{paymentRequest.Metadata["movieTitle"]}",
                                    Images = [$"{paymentRequest.Metadata["moviePoster"]}"]
                                },
                            },
                            Quantity = 1,
                        },
                    },
                    Mode = "payment",
                    SuccessUrl = $"{_configuration["Stripe:SuccessUrl"]}?session_id={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = _configuration["Stripe:CancelUrl"],
                    Metadata = new Dictionary<string, string>
                    {
                        { "movieId", $"{paymentRequest.Metadata["movieId"]}" },
                        { "movieTitle", $"{paymentRequest.Metadata["movieTitle"]}" },
                        { "moviePoster", $"{paymentRequest.Metadata["moviePoster"]}" },
                        { "userId", $"{User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value}" }
                    }
                };

                var service = new SessionService();
                var session = service.Create(options);

                return Ok(ApiResponse<string>.SuccessResponse(session.Url, "Session created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> WebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _webHooksSecret);

                if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;
                    var order = new Order
                    {
                        SessionId = session.Id,
                        TotalAmount = session.AmountTotal,
                        MovieId = int.Parse(session.Metadata["movieId"]),
                        UserId = session.Metadata["userId"],
                        CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(
                            session.Created,
                            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
                        ),
                    };

                    await _orderServices.CreateOrder(order);
                } 
                else if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    var order = new Order
                    {
                        PaymentIntentId = paymentIntent.Id,
                        TotalAmount = paymentIntent.Amount,
                        MovieId = int.Parse(paymentIntent.Metadata["movieId"]),
                        UserId = paymentIntent.Metadata["userId"],
                        CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(
                            paymentIntent.Created,
                            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
                        )
                    };
                    await _orderServices.CreateOrder(order);
                }
                else
                {
                    _logger.LogWarning("Unhandled event type: {0}", stripeEvent.Type);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("session-details/{sessionId}")]
        [Authorize]
        public ActionResult<ApiResponse<object>> GetSessionDetails(string sessionId)
        {
            try
            {
                var service = new SessionService();
                var session = service.Get(sessionId);

                var sessionDetails = new
                {
                    SessionId = session.Id,
                    session.PaymentStatus,
                    session.AmountTotal,
                    session.Currency,
                    MovieId = session.Metadata["movieId"],
                    UserId = session.Metadata["userId"],
                    Title = session.Metadata["movieTitle"],
                    Poster = session.Metadata["moviePoster"],
                    session.Created
                };

                return Ok(ApiResponse<object>.SuccessResponse(sessionDetails, "Session details retrieved successfully"));
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ApiResponse<string>.FailureResponse("Failed to retrieve session details."));
            }
        }
    }
}
