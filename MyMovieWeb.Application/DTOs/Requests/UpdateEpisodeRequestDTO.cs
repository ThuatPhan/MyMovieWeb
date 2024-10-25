using Microsoft.AspNetCore.Http;

namespace MyMovieWeb.Application.DTOs.Requests
{
    public class UpdateEpisodeRequestDTO
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public int? EpisodeNumber { get; set; }
        public IFormFile? videoFile { get; set; }
        public required int MovieId { get; set; }

    }
}
