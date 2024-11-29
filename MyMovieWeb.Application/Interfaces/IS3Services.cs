using Microsoft.AspNetCore.Http;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IS3Services
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task DeleteFileAsync(string fileUrl);
    }
}
