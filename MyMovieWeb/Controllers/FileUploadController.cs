using Microsoft.AspNetCore.Mvc;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.Interfaces;

namespace MyMovieWeb.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly ILogger<FileUploadController> _logger;
        private readonly IS3Services _s3Services;

        public FileUploadController(ILogger<FileUploadController> logger, IS3Services s3Services)
        {
            _logger = logger;
            _s3Services = s3Services;
        }

        [HttpPost("upload")]
        public async Task<ActionResult<string>> UploadFile([FromForm] FileUploadRequestDTO fileUploadRequest)
        {
            try
            {
                string fileUrl = await _s3Services.UploadFileAsync(fileUploadRequest.File);
                return Ok(new { location = fileUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when uploading file");
            }
        }
    }
}
