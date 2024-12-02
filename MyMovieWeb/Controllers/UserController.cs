using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMovieWeb.Application;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Presentation.Response;
using System.Security.Claims;

namespace MyMovieWeb.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserServices _userServices;
        private readonly IOrderServices _orderServices;

        public UserController(ILogger<UserController> logger, IUserServices userServices,IOrderServices orderServices)
        {
            _logger = logger;
            _userServices = userServices;
            _orderServices = orderServices;
        }


        [HttpPost("follow-movie")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> FollowMovie([FromQuery] int movieId)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                Result<bool> result = await _userServices.FollowMovie(movieId, userId);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when following movie");
            }
        }

        [HttpPost("unfollow-movie")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> UnfollowMovie([FromQuery] int movieId)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                Result<bool> result = await _userServices.UnfollowMovie(movieId, userId);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when unfollow movie");
            }
        }

        [HttpGet("is-movie-followed")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> IsUserFollowedMovie([FromQuery] int movieId)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                Result<bool> result = await _userServices.IsUserFollowMovie(movieId, userId);
                if (!result.IsSuccess)
                {
                    return Ok(ApiResponse<bool>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when check followed movie");
            }
        }

        [HttpGet("count-followed-movies")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<FollowedMovieDTO>>>> GetFollowedMovies()
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                Result<int> result = await _userServices.CountFollowedMovie(userId);
                return Ok(ApiResponse<int>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving followed movie");
            }
        }

        [HttpGet("followed-movies")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<FollowedMovieDTO>>>> GetFollowedMovies([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                Result<List<FollowedMovieDTO>> result = await _userServices.GetFollowedMovies(userId, pageNumber, pageSize);

                return Ok(ApiResponse<List<FollowedMovieDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving followed movie");
            }
        }

        [HttpPost("rate-movie")]
        public async Task<ActionResult<ApiResponse<bool>>> RateMovie([FromBody] CreateRateMovieRequestDTO rateMovieRequestDTO)
        {
            try
            {
                Result<bool> result = await _userServices.RateMovie(rateMovieRequestDTO);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(result.Data, result.Message));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when rating movie");
            }
        }

        [HttpGet("notifications")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<NotificationDTO>>>> GetNotifications()
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                Result<List<NotificationDTO>> result = await _userServices.GetNotifications(userId);
                return Ok(ApiResponse<List<NotificationDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving notifcations");
            }
        }

        [HttpPut("notification/mark-as-read/{notificationId}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<NotificationDTO>>> MarkNotificationAsRead([FromRoute] int notificationId)
        {
            try
            {
                Result<NotificationDTO> result = await _userServices.MarkNotificationAsRead(notificationId);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<NotificationDTO>.FailureResponse(result.Message));
                }
                return Ok(ApiResponse<NotificationDTO>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving notifcations");
            }
        }

        [HttpDelete("notification/delete/{notificationId}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteNotification([FromRoute] int notificationId)
        {
            try
            {
                Result<bool> result = await _userServices.DeleteNotification(notificationId);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.FailureResponse(result.Message));
                }
                return Ok(ApiResponse<bool>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving notifcations");
            }
        }

        [HttpGet("is-bought-movie")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> CheckMovieBought([FromQuery] int movieId)
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var result = await _userServices.IsUserBoughtMovie(userId, movieId);

                return Ok(ApiResponse<bool>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when rating movie");
            }
        }

        [HttpGet("purchased-movies")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<MovieDTO>>>> GetPủchasedMovies([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var result = await _userServices.PurchasedMovies(userId, pageNumber, pageSize);
                return Ok(ApiResponse<List<MovieDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving bought movies");
            }
        }
        [HttpGet("count-movies-buy")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<FollowedMovieDTO>>>> CountMoviesBuybyUser()
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                Result<int> result = await _orderServices.CountMovieBoughtbyUser(userId);
                return Ok(ApiResponse<int>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving followed movie");
            }
        }


    }
}
