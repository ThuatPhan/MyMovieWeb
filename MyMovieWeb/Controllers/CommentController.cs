using AutoMapper;
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
    public class CommentController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CommentController> _logger;
        private readonly ICommentService _commentService;

        public CommentController(ILogger<CommentController> logger, ICommentService commentService, IMapper mapper)
        {
            _commentService = commentService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("create-comment")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<CommentDTO>>> CreateMovieComment([FromBody] CreateMovieCommentRequestDTO commentRequestDTO)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                Result<CommentDTO> result = await _commentService.CreateMovieComment(commentRequestDTO, userId);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<CommentDTO>.FailureResponse(result.Message));
                }

                return CreatedAtAction(
                    nameof(GetComment),
                    new { id = result.Data.Id },
                    ApiResponse<CommentDTO>.SuccessResponse(result.Data, result.Message)
                );

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when create comment");
            }
        }

        [HttpPost("create-episode-comment")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<CommentDTO>>> CreateEpisodeComment([FromBody] CreateEpisodeCommentRequestDTO commentRequestDTO)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                Result<CommentDTO> result = await _commentService.CreateEpisodeComment(commentRequestDTO, userId);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<CommentDTO>.FailureResponse(result.Message));
                }

                return CreatedAtAction(
                    nameof(GetComment),
                    new { id = result.Data.Id },
                    ApiResponse<CommentDTO>.SuccessResponse(result.Data, result.Message)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when create comment");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteComment(int id)
        {
            try
            {
                Result<bool> result = await _commentService.DeleteComment(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when delete comment");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CommentDTO>>> GetComment([FromRoute] int id)
        {
            try
            {
                Result<CommentDTO> result = await _commentService.GetCommentById(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<CommentDTO>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<CommentDTO>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving comment");
            }
        }

        [HttpGet("movie/{movieId}")]
        public async Task<ActionResult<ApiResponse<List<CommentDTO>>>> GetCommentsOfMovie([FromRoute] int movieId)
        {
            try
            {
                Result<List<CommentDTO>> result = await _commentService.GetCommentsOfMovie(movieId);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<CommentDTO>>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<List<CommentDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when get comments of movie");
            }
        }

        [HttpGet("movie/{movieId}/episode/{episodeId}")]
        public async Task<ActionResult<ApiResponse<List<CommentDTO>>>> GetCommentsEpisode([FromRoute] int movieId, [FromRoute] int episodeId)
        {
            try
            {
                Result<List<CommentDTO>> result = await _commentService.GetCommentsOfEpisode(movieId, episodeId);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<CommentDTO>>.FailureResponse(result.Message));
                }
                return Ok(ApiResponse<List<CommentDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when get comments of episode");
            }
        }
    }
}

