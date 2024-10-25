using Microsoft.AspNetCore.Http;

namespace MyMovieWeb.Application.DTOs.Requests
{
    public class CreateEpisodeRequestDTO
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public int? EpisodeNumber { get; set; }
        public required IFormFile videoFile { get; set; }
        public required int MovieId { get; set; }
    }
}
