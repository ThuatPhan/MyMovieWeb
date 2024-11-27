namespace MyMovieWeb.Application.DTOs.Responses
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public bool IsPaid { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public required string Director { get; set; }
        public required List<string> Actors { get; set; }
        public required string PosterUrl { get; set; }
        public required string BannerUrl { get; set; }
        public required List<MovieGenreDTO> Genres { get; set; }
        public bool IsSeries { get; set; }
        public required List<EpisodeDTO> Episodes { get; set; }
        public bool? IsSeriesCompleted { get; set; }
        public string? VideoUrl { get; set; }
        public int View { get; set; }
        public int RateCount { get; set; }
        public int RateTotal { get; set; }
        public int CommentCount { get; set; }
        public bool IsShow { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
