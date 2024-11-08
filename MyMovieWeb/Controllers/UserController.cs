 using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMovieWeb.Application;
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

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
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

        [HttpGet("count-followed-movie")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> CountFollowedMovies()
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                Result<int> result = await _userServices.CountFollowedMovie(userId);

                return Ok(ApiResponse<int>.SuccessResponse(result.Data, userId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when counting followed movie");
            }
        }

        [HttpGet("followed-movies")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<FollowedMovieDTO>>>> GetFollowedMovies()
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                Result<List<FollowedMovieDTO>> result = await _userServices.GetFollowedMovies(userId);

                return Ok(ApiResponse<List<FollowedMovieDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving followed movie");
            }
        }
    }
}
