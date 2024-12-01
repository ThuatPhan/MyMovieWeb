using Microsoft.AspNetCore.Http;

namespace MyMovieWeb.Application.DTOs.Requests
{
    public class FileUploadRequestDTO
    {
        public required IFormFile File { get; set; }
    }
}
