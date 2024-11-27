using Microsoft.AspNetCore.Http;

namespace MyMovieWeb.Application.DTOs.Requests
{
    public class CreateMovieRequestDTO
    {
        public required string Title { get; set; }
        public bool IsPaid { get; set; } = false;
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public required string Director { get; set; }
        public required List<string> Actors { get; set; }
        public required List<int> GenreIds { get; set; }
        public bool IsSeries { get; set; } = false;
        public bool? IsShow { get; set; } = true;
        public required IFormFile BannerFile { get; set; }
        public required IFormFile PosterFile { get; set; }
        public IFormFile? VideoFile { get; set; }
    }
}
