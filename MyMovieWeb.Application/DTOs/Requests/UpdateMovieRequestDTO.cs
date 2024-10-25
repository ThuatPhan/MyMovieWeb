using Microsoft.AspNetCore.Http;

namespace MyMovieWeb.Application.DTOs.Requests
{
    public class UpdateMovieRequestDTO
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required string Director { get; set; }
        public required List<string> Actors { get; set; }
        public required List<int> GenreIds { get; set; }
        public bool? IsSeries { get; set; } = false;
        public bool? IsSeriesCompleted { get; set; }
        public bool? IsShow { get; set; } = true;
        public IFormFile? PosterFile { get; set; }
        public IFormFile? BannerFile { get; set; }
        public IFormFile? VideoFile { get; set; }
    }
}
