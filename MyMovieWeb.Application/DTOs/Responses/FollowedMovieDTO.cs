namespace MyMovieWeb.Application.DTOs.Responses
{
    public class FollowedMovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Director { get; set; }
        public List<string> Actors { get; set; }
        public string PosterUrl { get; set; }
        public string BannerUrl { get; set; }
        public List<MovieGenreDTO> Genres { get; set; }
        public bool IsSeries { get; set; }
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
