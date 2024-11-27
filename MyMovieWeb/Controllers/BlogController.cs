using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Application.Services;
using MyMovieWeb.Presentation.Response;

namespace MyMovieWeb.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> _logger;
        private readonly IBlogPostService _blogServices;

        public BlogController(ILogger<BlogController> logger, IBlogPostService blogServices)
        {
            _blogServices = blogServices;
            _logger = logger;
        }

        [HttpPost]
        [DisableRequestSizeLimit]
       // [Authorize(Policy = "create:blog")]
        public async Task<ActionResult<ApiResponse<BlogPostDTO>>> CreateBlog([FromForm] CreateBlogRequestDTO blogRequestDTO)
        {
            try
            {
                Result<BlogPostDTO> result = await _blogServices.CreateBlogPost(blogRequestDTO);
                return CreatedAtAction(
                    nameof(GetBlogById),
                    new { id = result.Data.Id },
                    ApiResponse<BlogPostDTO>.SuccessResponse(result.Data, result.Message)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<BlogPostDTO>.FailureResponse("An error occurred when creating Blog post")
                );
            }
        }

        [HttpPut("{id}")]
        [DisableRequestSizeLimit]
       // [Authorize(Policy = "update:blog")]
        public async Task<ActionResult<ApiResponse<BlogPostDTO>>> UpdateBlogPost([FromRoute] int id, [FromForm] UpdateBlogRequestDTO blogRequestDTO)
        {
            try
            {
                Result<BlogPostDTO> result = await _blogServices.UpdateBlogPost(id, blogRequestDTO);
                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<BlogPostDTO>.FailureResponse(result.Message));
                }
                return Ok(ApiResponse<BlogPostDTO>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<BlogPostDTO>.FailureResponse("An error occurred when Updating Blog post")
                );
            }
        }

        [HttpDelete("{id}")]
        //[Authorize(Policy = "delete:blog")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteBLog([FromRoute] int id)
        {
            try
            {
                Result<bool> result = await _blogServices.DeleteBlogPost(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.FailureResponse(result.Message));
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<bool>.FailureResponse("An error occurred when deleting Blog post")
                );
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<BlogPostDTO>>> GetBlogById([FromRoute] int id)
        {
            try
            {
                Result<BlogPostDTO> result = await _blogServices.GetBlogPostById(id);
                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<BlogPostDTO>.FailureResponse(result.Message));
                }
                return Ok(ApiResponse<BlogPostDTO>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<BlogPostDTO>.FailureResponse("An error occurred when retrieving Blog")
                );
            }
        }

    }
}
