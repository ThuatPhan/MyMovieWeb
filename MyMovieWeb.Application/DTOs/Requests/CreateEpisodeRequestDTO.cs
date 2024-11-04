using Microsoft.AspNetCore.Http;

namespace MyMovieWeb.Application.DTOs.Requests
{
    public class CreateEpisodeRequestDTO
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public int? EpisodeNumber { get; set; }
        public required IFormFile ThumbnailFile { get; set; }
        public required IFormFile VideoFile { get; set; }
        public required int MovieId { get; set; }
    }
}
